namespace Bro.Service.Context
{
    public interface IContextInitializer
    {
        void InitializeContext(IServiceContext context);
    }
}