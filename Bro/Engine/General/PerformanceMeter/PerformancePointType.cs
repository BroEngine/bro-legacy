using System.ComponentModel;

namespace Bro
{
    public enum PerformancePointType : byte 
    {
        [Description("udp_kernel_read")] UdpKernelRead = 0,
        [Description("udp_kernel_write")] UdpKernelWrite,
        
        [Description("udp_engine_events")] UdpEngineEvents, 
        [Description("udp_engine_packets_read")] UdpEnginePacketsRead,
        [Description("udp_engine_packets_send")] UdpEnginePacketsSend,
        
        [Description("network_peer_send_data")] NetworkPeerSendData,
        
        [Description("client_peer_receive_data")] ClientPeerReceiveData,
        [Description("client_peer_send_data")] ClientPeerSendData,
        
        [Description("context_request_handler")] ContextRequestHandler,
        
        [Description("broker_read_full")] BrokenReadFull,
        [Description("broker_read_handler")] BrokenReadHandler,
        
        [Description("broker_write")] BrokenWrite,
        
        //test
        //[Description("thread_pool")] ThreadPool,
        //[Description("thread_thread_scheduler")] ThreadScheduler
    }
}