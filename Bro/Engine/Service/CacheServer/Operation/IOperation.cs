using Sider;

namespace Bro.Service.Cache
{
    public interface IOperation
    {
        OperationType OperationType { get; }
       
        void Process(RedisClient redisClient);

        void InvokeCallback(bool result);
    }
}