using System;

namespace Alfred.Utils.Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public class WebSocketAuthorize : Attribute
    {
    }
}
