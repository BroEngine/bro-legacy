namespace Bro.Sketch
{
    public class VersionStorageContainer
    {
        public RemoteConfigVersionStorage RemoteConfigVersionStorage;
        public LocalConfigVersionStorage LocalConfigVersionStorage;
        public PersistentConfigVersionStorage PersistentConfigVersionStorage;
        public string VersionFileName;
    }
}