namespace Bro.Network
{
    public interface IWebContext
    {
        IWebClient GetWebClient(bool keepAlive = false, long timeout = 5000);
    }
}