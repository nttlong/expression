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
    }
}