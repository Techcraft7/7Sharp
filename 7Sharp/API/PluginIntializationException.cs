using System;
using System.Runtime.Serialization;

namespace _7Sharp
{
    [Serializable]
    internal class PluginIntializationException : Exception
    {
        public PluginIntializationException()
        {
        }

        public PluginIntializationException(string message) : base(message)
        {
        }

        public PluginIntializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PluginIntializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}