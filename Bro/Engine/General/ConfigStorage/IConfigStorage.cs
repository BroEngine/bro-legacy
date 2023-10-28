namespace Bro
{
    public interface IConfigProvider
    {
    }

    public interface IUnionConfigStorage
    {
        void AddConfigSubStorage(ConfigDetails details, IConfigProvider configProvider);
    }
    
    public interface IConfigStorage : IConfigProvider
    {
        int Version { get; }
        
        bool IsInitialized { get; }
        
        void Initialize(string configData, int version);

        void CopyFrom(IConfigStorage source);

        string Dump();
    }
    
    public interface IConfigProvider<TKey,TValue> : IConfigProvider
    {
        TValue GetConfig(TKey key);
    }
    
    public interface IConfigProvider<TValue>  :IConfigProvider
    {
        TValue GetConfig();
    }
}