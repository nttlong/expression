namespace DynamicExpr
{
    internal class VExpr
    {
        private string token;
        public string Value
        {
            get
            {
                return token;
            }
        }
        public VExpr(string token)
        {
            this.token = token;
        }
        public bool IsConst
        {
            get
            {
                return this.token[0] == "'".ToCharArray()[0] &&
                    this.token[this.token.Length - 1] == "'".ToCharArray()[0];
            }
        }
        public override string ToString()
        {
            return "val("+this.token+")";
        }
    }
}