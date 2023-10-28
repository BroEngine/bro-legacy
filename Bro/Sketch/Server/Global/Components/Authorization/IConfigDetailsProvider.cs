using System.Collections.Generic;

namespace Bro.Sketch.Server
{
    public interface IConfigDetailsProvider
    {
        List<ConfigDetails> GetConfigDetailsCatalog(Profile profile);
        List<ConfigDetails> GetDefaultConfigDetailsCatalog();
    }
}