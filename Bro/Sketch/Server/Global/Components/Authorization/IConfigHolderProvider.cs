namespace Bro.Sketch.Server
{
    public interface IConfigHolderProvider
    {
        ConfigHolder CreateConfigHolder(Profile profile);
        ConfigHolder CreateDefaultConfigHolder();
    }
}