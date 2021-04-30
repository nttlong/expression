using System.Collections.Generic;
using System.Linq;

namespace DynamicExpr
{
    internal class FExpr
    {
        public FExpr()
        {
        }

        public string Name { get; internal set; }
        public object Args { get; internal set; }
        public override string ToString()
        {
            var _args = Args as List<object>;
            return "fn."+Name+"("+string.Join(',', _args.Select(p=>p.ToString()))+")";
        }
    }
}