using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DynamicExpr
{
    public class BuilinFuncsMap
    {
        public static MethodInfo Left => typeof(BuilinFuncs).GetMethods().FirstOrDefault(p => p.Name == nameof(BuilinFuncs.Left));

        public static MethodInfo Concat =>
            typeof(string).GetMethods()
            .FirstOrDefault(p => p.GetParameters().Any(x => x.ParameterType==typeof(string[])));
    }
    public class BuilinFuncs
    {
        public static string Left(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
        
    }
}
