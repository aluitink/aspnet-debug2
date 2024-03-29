﻿using System.Runtime.Serialization;

namespace aspnet_debug.Shared
{
    [DataContract]
    public class MessageBase
    {
        [DataMember]
        public Command Command { get; set; }

        [DataMember]
        public object Payload { get; set; }
    }
}