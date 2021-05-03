using System;
using System.Linq;

namespace DynamicExpr
{
    public class PExpr : FXExpr
    {
        public PExpr(string token)
        {
            this.token = token;
        }

        public string Name
        {
            get
            {
                return this.token.Substring(1, this.token.Length - 2);
            }
        }

        

        public override string ToString()
        {
            return "Params.[" + this.token.Substring(1, this.token.Length - 2)+"]";
        }
        public override string ToCSharpLamdaExpr(object ParamsObject)
        {
            if (ParamsObject == null) return "null";
            var P = ParamsObject.GetType().GetProperties()
                .FirstOrDefault(p=>p.Name.ToLower()==this.Name.ToLower());
            if(P==null)
            {
                return "null";
            }
            var val = P.GetValue(ParamsObject);
            if (val == null) return "null";
            var pType = Nullable.GetUnderlyingType(P.PropertyType) ?? P.PropertyType;
            if (pType == typeof(string))
            {
                return @""""+val.ToString()+@"""";
            }
            throw new NotImplementedException();
        }
    }
}