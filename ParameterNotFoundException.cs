using System;
using System.Runtime.Serialization;

namespace DynamicExpr
{
    [Serializable]
    internal class ParameterNotFoundException : Exception
    {
        public ParameterNotFoundException()
        {
        }

        public ParameterNotFoundException(string message) : base(message)
        {
        }

        public ParameterNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParameterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string ParamName { get; internal set; }
        public object ParamRef { get; internal set; }
    }
}