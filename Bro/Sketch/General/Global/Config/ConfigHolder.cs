using System;
using System.Collections.Generic;

namespace Bro.Sketch
{
    public class ConfigHolder
    {
        public IReadOnlyList<ConfigDetails> DetailsCatalog { get; private set; }
        private readonly Dictionary<Type, IConfigProvider> _storages = new Dictionary<Type, IConfigProvider>();
        private readonly Dictionary<Type,Type> _unionStorageHierarchy = new Dictionary<Type, Type>();
        
        public void AddUnionStorageType(Type unionStorageType, Type subStorageType)
        {
            _unionStorageHierarchy[subStorageType] = unionStorageType;
        }

        public void Initialize(List<ConfigDetails> detailsCatalog, ConfigStorageCollector collector)
        {
            DetailsCatalog = detailsCatalog;
            foreach (var details in detailsCatalog)
            {
                var unionStorageType =  GetUnionStorageType(details.StorageType);
                if(unionStorageType != null)
                {
                    var configProvider = GetConfigProvider(unionStorageType);
                    var subStorage = collector.GetStorage(details);
                    if (configProvider == null)
                    {
                        configProvider = Activator.CreateInstance(unionStorageType) as IConfigProvider;
                        AddConfigProvider(unionStorageType, configProvider);
                    }
                    ((IUnionConfigStorage)configProvider).AddConfigSubStorage(details,subStorage);
                }
                else
                {
                    var configProvider = collector.GetStorage(details);
                    AddConfigProvider(details.StorageType, configProvider);
                }
            }
        }

        private Type GetUnionStorageType(Type subStorageType)
        {
            _unionStorageHierarchy.TryGetValue(subStorageType, out var result);
            return result;
        }
        private IConfigProvider GetConfigProvider(Type type)
        {
            _storages.FastTryGetValue(type, out var result);
            return result;
        }

        private void AddConfigProvider(Type storageType, IConfigProvider configProvider)
        {
            _storages[storageType] = configProvider;
        }

        public T GetConfigStorage<T>() where T : class, IConfigProvider
        {
            var type = typeof(T);
            return GetConfigProvider(type) as T;
        }
    }
}