using System;

namespace DynamicExpr
{
    public class FXExpr
    {
        internal string token;

        public virtual string ToCSharpLamdaExpr(object ParamsObject)
        {
            if(this is BExpr)
            {
                return ((BExpr)this).ToCSharpLamdaExpr(ParamsObject);
            }
            if(this is NExpr)
            {
                return ((NExpr)this).ToCSharpLamdaExpr(ParamsObject);
            }
            if (this is PExpr)
            {
                return ((PExpr)this).ToCSharpLamdaExpr(ParamsObject);
            }
            if (this is FExpr)
            {
                return ((FExpr)this).ToCSharpLamdaExpr(ParamsObject);
            }
            throw new NotImplementedException();
        }
    }
}