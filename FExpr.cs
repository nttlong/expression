using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicExpr
{
    public class FExpr:FXExpr
    {
        public FExpr()
        {
        }

        public string Name { get; internal set; }
        public List<FXExpr> Args { get; internal set; }
        public MethodInfo Method { get; internal set; }

        public override string ToString()
        {
            var _args = Args as List<FXExpr>;
            return "fn."+Name+"("+string.Join(',', _args.Select(p=>p.ToString()))+")";
        }
    }
}