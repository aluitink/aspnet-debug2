using System.Runtime.Serialization;

namespace aspnet_debug.Shared
{
    [DataContract]
    public class StartDebuggingMessage
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public ApplicationType AppType { get; set; }

        [DataMember]
        public byte[] DebugContent { get; set; }
    }
}