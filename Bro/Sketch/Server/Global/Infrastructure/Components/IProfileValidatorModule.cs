using Bro.Server.Context;

namespace Bro.Sketch.Server.Infrastructure
{
    public interface IProfileValidatorModule : IServerContextModule
    {
        string Validate(string json);
        string Default();
    }
}