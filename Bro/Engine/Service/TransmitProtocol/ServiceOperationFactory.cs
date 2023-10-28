using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bro.Network.TransmitProtocol;
using Bro.Service;

namespace Bro.Network.Service
{
    public class ServiceOperationFactory
    {
        private static bool _assemblyInited = false;

        private delegate IServiceEvent CreateServiceEventDelegate();
        private delegate IServiceRequest CreateServiceRequestDelegate();
        private delegate IServiceResponse CreateServiceResponseDelegate();

        private static readonly Dictionary<byte, CreateServiceEventDelegate> _eventCreator = new Dictionary<byte, CreateServiceEventDelegate>();
        private static readonly Dictionary<byte, CreateServiceRequestDelegate> _requestCreator = new Dictionary<byte, CreateServiceRequestDelegate>();
        private static readonly Dictionary<byte, CreateServiceResponseDelegate> _responseCreator = new Dictionary<byte, CreateServiceResponseDelegate>();
        
        static ServiceOperationFactory()
        {
            Init(Assembly.GetExecutingAssembly());
        }
        
        private static void RegisterRequest(byte code, CreateServiceRequestDelegate dlg)
        {
            _requestCreator[code] = dlg;
        }

        private static void RegisterEvent(byte code, CreateServiceEventDelegate dlg)
        {
            _eventCreator[code] = dlg;
        }

        private static void RegisterResponse(byte code, CreateServiceResponseDelegate dlg)
        {
            _responseCreator[code] = dlg;
        }

        private static MethodInfo GetCreateMethodInfo(Type parentType)
        {
            string createMethodName = "Create";
            BindingFlags methodFlags = BindingFlags.Static | BindingFlags.Public;

            MethodInfo methodInfo = null;
            do
            {
                methodInfo = parentType.GetMethod(createMethodName, methodFlags);
                parentType = parentType.BaseType;
            } while (methodInfo == null && parentType != null);

            return methodInfo;
        }

        private static T GetCustomAttribute<T>(Type type) where T : Attribute
        {
            // Send inherit as false if you want the attribute to be searched only on the type. If you want to search the complete inheritance hierarchy, set the parameter to true.
            object[] attributes = type.GetCustomAttributes(false);
            return attributes.OfType<T>().FirstOrDefault();
        }

        public static void Init(Assembly assembly)
        {
            if (_assemblyInited)
            {
                return;
            }

            _assemblyInited = true;

            var eventInterfaceName = typeof(IServiceEvent).Name;
            var responseInterfaceName = typeof(IServiceResponse).Name;
            var requestInterfaceName = typeof(IServiceRequest).Name;
            var allTypes = assembly.GetTypes();
            Type parentType, currentType;
            for (int i = 0, max = allTypes.Length; i < max; ++i)
            {
                currentType = allTypes[i];
                parentType = currentType.BaseType;
                if (parentType != null && parentType.IsGenericType )
                {
                    if (parentType.GetInterface(eventInterfaceName) != null)
                    {
                        var methodInfo = GetCreateMethodInfo(parentType);
                        var createMethod = Delegate.CreateDelegate(typeof(CreateServiceEventDelegate), methodInfo) as CreateServiceEventDelegate;
                        RegisterEvent(createMethod().OperationCode, createMethod);
                    }
                    else if (parentType.GetInterface(responseInterfaceName) != null)
                    {
                        var methodInfo = GetCreateMethodInfo(parentType);
                        var createMethod = Delegate.CreateDelegate(typeof(CreateServiceResponseDelegate), methodInfo) as CreateServiceResponseDelegate;
                        RegisterResponse(createMethod().OperationCode, createMethod);
                    }
                    else if (parentType.GetInterface(requestInterfaceName) != null)
                    {
                        
                        var methodInfo = GetCreateMethodInfo(parentType);
                        var createMethod = Delegate.CreateDelegate(typeof(CreateServiceRequestDelegate), methodInfo) as CreateServiceRequestDelegate;
                        RegisterRequest(createMethod().OperationCode, createMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Serialize any net operation to writer in structure
        /// short BodySize - how long the following data of net operation
        /// byte OperationType - event/response/request
        /// byte OperationCode 
        /// bytes Params - the net operation data
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="operation">Operation.</param>
        public static void Serialize(IWriter writer, IServiceOperation operation)
        {
            // 32 bytes - length
            //  8 bytes - header ( 5 - checksum, 3 - type )
            //  8 bytes - code 
            // 16 bytes - counter

            var startPosition = writer.Position;

            writer.Position += 5; // length + header
            writer.Write((byte) operation.OperationCode);

            operation.Serialize(writer);

            var endPosition = writer.Position;
            var operationLength = (int) (endPosition - startPosition - 4);

            var checkSum = operationLength % 31;

            byte header = 0;
            header = (byte) (header | checkSum);
            header = (byte) (header << 3);
            header = (byte) (header | (byte) operation.Type);

            writer.Position = startPosition;
            writer.Write(operationLength);
            writer.Write(header);

            writer.Position = endPosition;
        }

        public static IServiceOperation Deserialize(IReader reader)
        {
            IServiceOperation result = null;

            // 32 bits - length
            //  8 bits - header ( 5 - checksum, 3 - type )
            //  8 bits - code 
            // 16 bits - counter

            int operationLength;
            reader.Read(out operationLength);
            var readerStartPosition = reader.Position;
            var readerEndPosition = readerStartPosition + operationLength;

            byte operationHeader;
            reader.Read(out operationHeader);

            // - - - - - * * * 7 = 00000111 ( operation type )
            var operationType = (ServiceOperationType) (operationHeader & 7);


            // * * * * * - - - 248 = 00011000 ( protocol version )
            var checkSum = ((byte) (operationHeader & 248) >> 3);

            if (operationLength % 31 != checkSum)
            {
                Bro.Log.Error( "NetworkOperationFactory: Invalid checksum when deserialing. Operation type ( possible ) = " +  operationType);
                return null;
            }

            byte operationCode;
            reader.Read(out operationCode);
            
            switch (operationType)
            {
                case ServiceOperationType.Event:
                {
                    CreateServiceEventDelegate createDelegate;
                    if (_eventCreator.TryGetValue(operationCode, out createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        Bro.Log.Info("Service event received with operation code = " + operationCode + ", but no handler found for it");
                    }
                }
                    break;

                case ServiceOperationType.Request:
                {
                    CreateServiceRequestDelegate createDelegate;
                    if (_requestCreator.TryGetValue(operationCode, out createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        Bro.Log.Info("Service request received with operation code = " + operationCode + ", but no handler found for it");
                    }
                }
                    break;

                case ServiceOperationType.Response:
                {
                    CreateServiceResponseDelegate createDelegate;
                    if (_responseCreator.TryGetValue(operationCode, out createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        // Bro.Log.Info($"service response received with operation code = {operationCode}, but no handler found for it");
                    }
                }
                    break;

                default:
                {
                    throw new System.SystemException("Unknown network operation type = " + operationType);
                }
            }

            if (result != null)
            {
                result.Deserialize(reader);
            }
            else
            {
                reader.Position = readerEndPosition;
                return null;
            }

            if (readerEndPosition != reader.Position)
            {
                Bro.Log.Error("NetworkOperationFactory: Operation length ( " + (reader.Position - readerStartPosition) + " ) " +
                              "is not equal to expected length ( " + operationLength + " ). " +
                              "Operation type ( possible ) = " + operationType +
                              "Operation code ( possible ) = " + operationCode);

                reader.Position = readerEndPosition;
                return null;
            }
            
            return result;
        }

        public static IServiceResponse CreateResponse(IServiceRequest request)
        {
            CreateServiceResponseDelegate createDelegate;
            if (_responseCreator.TryGetValue(request.OperationCode, out createDelegate))
            {
                var result = createDelegate();
                result.IsHolded = false;
                result.TemporaryIdentifier = request.TemporaryIdentifier;
                result.Channel = request.ResponseChannel;
                return result;
            }

            Bro.Log.Error("No response created for operation code " + request.OperationCode);
            return null;
        }
    }
}