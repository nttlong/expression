using System.Collections.Generic;
using System.Linq;

namespace DynamicExpr
{
    internal class FExpr: FxExpr
    {
        public FExpr()
        {
        }

        public string Name { get; internal set; }
        public List<FxExpr> Args { get; internal set; }
        public override string ToString()
        {
            var _args = Args as List<FxExpr>;
            return "fn."+Name+"("+string.Join(',', _args.Select(p=>p.ToString()))+")";
        }
    }
}