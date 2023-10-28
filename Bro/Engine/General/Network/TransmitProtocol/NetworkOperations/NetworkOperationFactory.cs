using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public static class NetworkOperationFactory 
    {
        #region Registrations

        private delegate INetworkEvent CreateEventDelegate();
        private delegate INetworkRequest CreateRequestDelegate();
        private delegate INetworkResponse CreateResponseDelegate();

        /* static paradigm */
        private static readonly Dictionary<byte, CreateEventDelegate> _eventCreator = new Dictionary<byte, CreateEventDelegate>();
        private static readonly Dictionary<byte, CreateRequestDelegate> _requestCreator = new Dictionary<byte, CreateRequestDelegate>();
        private static readonly Dictionary<byte, CreateResponseDelegate> _responseCreator = new Dictionary<byte, CreateResponseDelegate>();
        /* static paradigm */
        
        private static void RegisterRequest(byte code, CreateRequestDelegate dlg)
        {
            CheckExistingRequest(code, dlg);
            _requestCreator[code] = dlg;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void CheckExistingRequest(byte code, CreateRequestDelegate dlg)
        {
            if (_requestCreator.ContainsKey(code))
            {
                Bro.Log.Error("Already exists with code " + code + " " + dlg);
            }
        }

        private static void RegisterEvent(byte code, CreateEventDelegate dlg)
        {
            CheckExistingEvent(code, dlg);
            _eventCreator[code] = dlg;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void CheckExistingEvent(byte code, CreateEventDelegate dlg)
        {
            if (_eventCreator.ContainsKey(code))
            {
                Bro.Log.Error("Already exists with code " + code + " " + dlg);
            }
        }

        private static void RegisterResponse(byte code, CreateResponseDelegate dlg)
        {
            CheckExistingResponse(code, dlg);
            _responseCreator[code] = dlg;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void CheckExistingResponse(byte code, CreateResponseDelegate dlg)
        {
            if (_responseCreator.ContainsKey(code))
            {
                Bro.Log.Error("Already exists with code " + code + " " + dlg);
            }
        }

        private static MethodInfo GetOperationMethodInfo(Type curType)
        {
            return typeof(NetworkPool).GetMethod("GetOperation").MakeGenericMethod(curType);
        }

        private static MethodInfo GetParamMethodInfo(Type curType)
        {
            return typeof(NetworkPool).GetMethod("GetParam").MakeGenericMethod(curType);
        }
        
        static NetworkOperationFactory()
        {
            RegisterClasses(Assembly.GetExecutingAssembly());
        }

        public static void Initialize()
        {
            // its call static constructor, if it not called before
        }

        private static void RegisterClasses(Assembly assembly)
        {
            var eventInterfaceName = typeof(INetworkEvent).Name;
            var responseInterfaceName = typeof(INetworkResponse).Name;
            var requestInterfaceName = typeof(INetworkRequest).Name;
            var allTypes = assembly.GetTypes();
            Type parentType, currentType;
            for (int i = 0, max = allTypes.Length; i < max; ++i)
            {   
                currentType = allTypes[i];
               
                if (currentType.IsClass)
                {
                    parentType = currentType.BaseType;
                    if (!currentType.IsAbstract && parentType != null && parentType.IsGenericType)
                    {
                        if (parentType.GetInterface(eventInterfaceName) != null)
                        {
                            var methodInfo = GetOperationMethodInfo(currentType);
                            var createMethod = Delegate.CreateDelegate(typeof(CreateEventDelegate), methodInfo) as CreateEventDelegate;
                            var sampleEvent = createMethod();
                        
                            RegisterEvent(sampleEvent.OperationCode, createMethod);
                            sampleEvent.Retain();
                            sampleEvent.Release();
                        }
                        else if (parentType.GetInterface(responseInterfaceName) != null)
                        {
                            var methodInfo = GetOperationMethodInfo(currentType);
                            var createMethod = Delegate.CreateDelegate(typeof(CreateResponseDelegate), methodInfo) as CreateResponseDelegate;
                            var sampleResponse = createMethod();
                        
                            RegisterResponse(sampleResponse.OperationCode, createMethod);
                            sampleResponse.Retain();
                            sampleResponse.Release();
                        }
                        else if (parentType.GetInterface(requestInterfaceName) != null)
                        {
                            var methodInfo = GetOperationMethodInfo(currentType);
                            var createMethod = Delegate.CreateDelegate(typeof(CreateRequestDelegate), methodInfo) as CreateRequestDelegate;
                            var sampleRequest = createMethod();
                        
                            RegisterRequest(sampleRequest.OperationCode, createMethod);
                            sampleRequest.Retain();
                            sampleRequest.Release();
                        }
                    }
                    RegisterUniversalParam(currentType);

                    if (!currentType.IsAbstract && !currentType.IsInterface)
                    {
                        RegisterFeatureType(currentType);
                    }
                }
            }
        }
        
        private static void RegisterUniversalParam(Type currentType)
        {  
            if (Attribute.GetCustomAttribute(currentType, typeof(UniversalParamRegistrationAttribute), false) is UniversalParamRegistrationAttribute universalParamAttribute)
            {
                var objectType = universalParamAttribute.Type;
                var paramType = currentType;
                var paramInfo = GetParamMethodInfo(paramType);
                var createMethod = Delegate.CreateDelegate(typeof(ParamsRegistry.CreateParamFunc), paramInfo) as ParamsRegistry.CreateParamFunc;
                ParamsRegistry.Register(objectType, createMethod, universalParamAttribute.ParamTypeIndex);
            }
        }

        private static void RegisterFeatureType(Type currentType)
        {
            if (Attribute.GetCustomAttribute(currentType, typeof(TypeBinderAttribute), false) is TypeBinderAttribute classNameAttribute)
            {
                ClassTypeBinder.Register(currentType, classNameAttribute.TypeKey);
            }
        }
        #endregion

        /// <summary>
        /// Serialize any net operation to writer in structure
        /// short BodySize - how long the following data of net operation
        /// byte OperationType - event/response/request
        /// byte OperationCode 
        /// bytes Params - the net operation data
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="operation">Operation.</param>
        public static void Serialize(IWriter writer, INetworkOperation operation)
        {
            // 32 bytes - length
            //  8 bytes - header ( 5 - checksum, 3 - type )
            //  8 bytes - code 
            // 16 bytes - counter

            var startPosition = writer.Position;

            writer.Position += 5; // length + header
            writer.Write((byte) operation.OperationCode);
            writer.Write((short) operation.OperationCounter);
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

        public static INetworkOperation Deserialize(IReader reader)
        {
            // 32 bytes - length
            //  8 bytes - header ( 5 - checksum, 3 - type )
            //  8 bytes - code 
            // 16 bytes - counter

            reader.Read(out int operationLength);
            var readerStartPosition = reader.Position;
            var readerEndPosition = readerStartPosition + operationLength;
            reader.Read(out byte operationHeader);

            // - - - - - * * * 7 = 00000111 ( operation type )
            var operationType = (NetworkOperationType) (operationHeader & 7);

            // * * * * * - - - 248 = 00011000 ( protocol version )
            var checkSum = ((byte) (operationHeader & 248) >> 3);

            if (operationLength % 31 != checkSum)
            {
                Bro.Log.Error(  "NetworkOperationFactory: Invalid checksum when deserialing. Operation type ( possible ) = " + operationType);
                return null;
            }

            reader.Read(out byte operationCode);
            reader.Read(out short operationCounter);

            var result = CreateNetworkOperation(operationType, operationCode);

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
                Bro.Log.Error( $"network operation factory :: operation length = {(reader.Position - readerStartPosition)} is not equal to expected length = {operationLength}; operation type = {operationType}; operation code = {operationCode}");
                reader.Position = readerEndPosition;
                return null;
            }

            result.OperationCounter = operationCounter;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static INetworkOperation CreateNetworkOperation(NetworkOperationType operationType, byte operationCode)
        {
            INetworkOperation result = null;

            switch (operationType)
            {
                case NetworkOperationType.Event:
                {
                    if (_eventCreator.TryGetValue(operationCode, out var createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        Bro.Log.Error(
                            $"network operation factory :: event received with operation code = {operationCode} but no handler found for it");
                    }
                }
                    break;

                case NetworkOperationType.Request:
                {
                    if (_requestCreator.TryGetValue(operationCode, out var createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        Bro.Log.Error(
                            $"network operation factory :: request received with operation code = {operationCode} but no handler found for it");
                    }
                }
                    break;

                case NetworkOperationType.Response:
                {
                    if (_responseCreator.TryGetValue(operationCode, out var createDelegate))
                    {
                        result = createDelegate();
                    }
                    else
                    {
                        Bro.Log.Error(
                            $"network operation factory :: response received with operation code = {operationCode} but no handler found for it");
                    }
                }
                    break;

                case NetworkOperationType.Ping:
                    result = new PingOperation();
                    break;

                case NetworkOperationType.Encryption:
                    result = new LetsEncryptOperation();
                    break;

                case NetworkOperationType.Handshake:
                    result = new HandShakeOperation();
                    break;

                default:
                    throw new System.SystemException(
                        $"network operation factory :: unknown network operation type = {operationType}");
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static INetworkResponse CreateResponse(INetworkRequest request)
        {
            if (_responseCreator.TryGetValue(request.OperationCode, out var createDelegate))
            {
                var result = createDelegate();
                result.IsHeld = false;
                result.TemporaryIdentifier = request.TemporaryIdentifier;
                return result;
            }
            else
            {
                Bro.Log.Error("No response created for operation code " + request.OperationCode);
            }

            return null;
        }
    }
}