﻿using System.Runtime.Serialization;

namespace aspnet_debug.Shared
{
    [DataContract]
    public enum ApplicationType
    {
        [EnumMember] Desktopapplication,
        [EnumMember] Webapplication
    }
}