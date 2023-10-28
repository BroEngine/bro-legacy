using Bro.Network.TransmitProtocol;

namespace Bro.Ecs
{
    public interface IComponentSerializer
    {
        void Serialize<T>(IWriter writer, ref T component) where T : struct;
        void Deserialize<T>(IReader reader, out T component) where T : struct;
    }
}