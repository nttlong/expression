using System;

namespace DynamicExpr
{
    public class NExpr : FXExpr
    {
        
        public string Value
        {
            get
            {
                return base.token;
            }
        }
        public NExpr(string token)
        {
            this.token = token;
        }
        public ConstTypeEnum ValueType
        {
            //[System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (this.token == "'") return ConstTypeEnum.Text;
                if (this.token[0] == "'".ToCharArray()[0] &&
                    this.token[this.token.Length - 1] == "'".ToCharArray()[0])
                {
                    var val = this.token.Substring(1, this.token.Length - 2);
                    DateTime od = new DateTime();
                    if (DateTime.TryParse(val, out od))
                    {
                        return ConstTypeEnum.Date;
                    }
                    return ConstTypeEnum.Text;
                }
                else
                {
                    long x = 0;
                    if(long.TryParse(this.token,out x))
                    {
                        return ConstTypeEnum.Int;
                    }
                    decimal d = 0;
                    if (decimal.TryParse(this.token, out d))
                    {
                        return ConstTypeEnum.Numeric;
                    }
                    bool b = false;
                    if (bool.TryParse(this.token, out b))
                    {
                        return ConstTypeEnum.Bool;
                    }
                }
                return ConstTypeEnum.None;
            }
        }
        public override string ToString()
        {
            return "Fields.["+this.token+"]";
        }

        internal object GetValue()
        {
            if (this.ValueType == ConstTypeEnum.Bool)
            {
                return bool.Parse(this.token);
            }
            if (this.ValueType == ConstTypeEnum.Int)
            {
                return long.Parse(this.token);
            }
            if (this.ValueType == ConstTypeEnum.Text)
            {
                return this.token;
            }
            if (this.ValueType == ConstTypeEnum.Date)
            {
                return DateTime.Parse(this.token);
            }
            if (this.ValueType == ConstTypeEnum.Numeric)
            {
                return Decimal.Parse(this.token);
            }
            throw new NotImplementedException();
        }
    }
}