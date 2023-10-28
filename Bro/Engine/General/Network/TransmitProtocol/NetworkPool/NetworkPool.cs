using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro
{
    public static class NetworkPool
    {
        private static PoolContainer<INetworkOperation> NetworkOperations { get; }
        private static PoolContainer<BaseParam> NetworkParams { get; }

        private static bool IsLoggingEnabled = false;

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
        public static bool IsEnabled => true;
#elif UNITY_EDITOR
        public static bool IsEnabled => true;
#else
        public static bool IsEnabled => false;
#endif

        static NetworkPool()
        {
            NetworkOperations = new PoolContainer<INetworkOperation>(IsLoggingEnabled);
            NetworkParams = new PoolContainer<BaseParam>(IsLoggingEnabled, new Dictionary<Type, int>()
            {
                {typeof(IntParam), 256},
                {typeof(UniversalParam), 256}
            });
        }

        public static void SetPoolSizeForOperation<T>(int size) where T : INetworkOperation
        {
            NetworkOperations.SetPoolSize<T>(size);
        }

        public static void SetPoolSizeForParam<T>(int size) where T : BaseParam
        {
            NetworkParams.SetPoolSize<T>(size);
        }

        public static T GetOperation<T>() where T : class, INetworkOperation, new()
        {
            if (IsEnabled)
            {
                var op = NetworkOperations.GetValue<T>();

                if (!op.IsPoolable)
                {
                    op.Retain();
                    op.Release();
                    return new T();
                }

                op.Cleanup();
                
                return op;
            }

            return new T();
        }

        public static void PutBackOperation(INetworkOperation op)
        {
            if (IsEnabled)
            {
                NetworkOperations.PutBack(op);
            }
        }

        public static T GetParam<T>() where T : BaseParam, new()
        {
            if (IsEnabled)
            {
                return NetworkParams.GetValue<T>();
            }
            return new T();
        }
        
        // public static T GetParam<T>(System.Func<BaseParam> createDelegate) where T : BaseParam, new()
        // {
        //     if (IsEnabled)
        //     {
        //         return NetworkParams.GetValue<T>(createDelegate);
        //     }
        //     return (T)createDelegate();
        // }
        

        public static void PutBackParam(BaseParam p)
        {
            if (IsEnabled)
            {
                NetworkParams.PutBack(p);
            }
        }

        public static void Reset()
        {
            NetworkOperations.Reset();
            NetworkParams.Reset();
        } 

        public static void StateChanged(string savePath)
        {
            // void SaveFileInDirectory(string fileDirectory, string fileText)
            // {
            //     if (!File.Exists(fileDirectory))
            //     {
            //         File.Create(fileDirectory).Close();
            //     }
            //
            //     File.WriteAllText(fileDirectory, fileText);
            // }
            //
            // if (!IsLoggingEnabled)
            // {
            // }
            //
            // const string saveDirectory = "PoolManagersLogs";
            // const string operationsPool = "operationsPool.txt";
            // const string paramsPool = "paramsPool.txt";
            //
            // var savesFullPath = Path.Combine(savePath, saveDirectory);
            //
            // if (!Directory.Exists(savesFullPath))
            // {
            //     Directory.CreateDirectory(Path.GetFullPath(savesFullPath));
            // }
            //
            // SaveFileInDirectory(Path.GetFullPath(Path.Combine(savesFullPath, operationsPool)), NetworkOperations.PoolContainerDebugger.ToString());
            // SaveFileInDirectory(Path.GetFullPath(Path.Combine(savesFullPath, paramsPool)), NetworkParams.PoolContainerDebugger.ToString());

        }


     

    }
}