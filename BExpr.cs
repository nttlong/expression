using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpr
{
    public class BExpr:FXExpr
    {

        public string Operator { get; internal set; }
        public FXExpr Left { get; internal set; }
        public FXExpr Right { get; internal set; }
        public override string ToString()
        {
            return "("+Left.ToString()+")"+Operator+"("+Right.ToString()+")";
        }
        public override string ToCSharpLamdaExpr(object ParamsObject)
        {
            var op = Operator;
            if ("+,-,*,/,%,^".IndexOf(op) == -1)
            {
                if (op == "=") op = "==";
                else if (op == "and") op = "&&";
                else if (op == "or") op = "||";
                else
                {
                    throw new NotImplementedException();
                }
            }
            return "(" + Left.ToCSharpLamdaExpr(ParamsObject) + ")" + op + "(" + Right.ToCSharpLamdaExpr(ParamsObject) + ")";
        }
    }
}
