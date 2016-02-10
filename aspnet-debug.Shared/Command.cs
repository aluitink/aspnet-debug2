using System.Runtime.Serialization;

namespace aspnet_debug.Shared
{
    [DataContract]
    public enum Command : byte
    {
        [EnumMember]
        DebugContent,
        [EnumMember]
        StartedMono,
        [EnumMember] 
        Shutdown
    }
}