using System;

namespace Kekiri.Reporting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SuppressOutputAttribute : Attribute
    {
        public string Reason { get; set; }
    }
}