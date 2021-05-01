using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpr
{
    public static class ExprBuilders
    {

        public static LambdaExpression ToLambdaExpression(
            Type type, string StrExpr,
            object ParamsObject)
        {
            var Pr = Expression.Parameter(type,"p");
            var Expr = StrExpr.FixBracket().FixSpace().ToFxExpression();
            Expression x = null;
            if(Expr is BExpr)
            {
                x= ToConditionlExpression(Pr, Expr as BExpr, ParamsObject);
                return Expression.Lambda(x,Pr);
            }
            if(Expr is FExpr)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private static Expression ToConditionlExpression(ParameterExpression Pr, BExpr bExpr, object paramsObject)
        {
            if(bExpr.Operator=="="||
               bExpr.Operator=="==")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                return Expression.Equal(left, right);
            }
            if (bExpr.Operator == "+" )
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                return Expression.Add(left, right);
            }
            if (bExpr.Operator == "-")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                return Expression.Subtract(left, right);
            }
            if (bExpr.Operator == "*")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                return Expression.Multiply(left, right);
            }
            if (bExpr.Operator == "/")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                return Expression.Divide(left, right);
            }
            if (bExpr.Operator == "%")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                return Expression.Modulo(left, right);
            }
            if (bExpr.Operator == "^")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                left = Expression.Convert(left, typeof(double));

                MakeSureLeftAndRighIsTheSameType(ref left, ref right);
                var mc = typeof(Math).GetMethod("Pow");

                return Expression.Call(mc,left, right);
            }
            if (bExpr.Operator == "&")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                var mci = typeof(String).GetMethods().FirstOrDefault(p => p.Name == "Concat");
                return Expression.Call(mci, left, right);
            }
            if (bExpr.Operator.ToLower() == "and"||
                bExpr.Operator=="&&")
            {
                Expression left = ToExpression(Pr, bExpr.Left, paramsObject);
                Expression right = ToExpression(Pr, bExpr.Right, paramsObject);
                return Expression.AndAlso(left, right);
            }
            throw new NotImplementedException();
        }

        private static void MakeSureLeftAndRighIsTheSameType(ref Expression left, ref Expression right)
        {
            if (right is UnaryExpression)
            {
                var ux = right as UnaryExpression;
                if (ux.Operand is ConstantExpression)
                {
                    var val = Convert.ChangeType(((ConstantExpression)ux.Operand).Value, left.Type);
                    right = Expression.Constant(val);
                }
                return;
            }
            if (left is MemberExpression)
            {
                if (right is UnaryExpression)
                {
                    var ux = right as UnaryExpression;
                    if (ux.Operand is ConstantExpression)
                    {
                        var val = Convert.ChangeType(((ConstantExpression)ux.Operand).Value, left.Type);
                        right = Expression.Constant(val);
                    }
                    return;
                }

                return;
            }
            if(left is UnaryExpression)
            {
                var rType = right.Type;
                var ux = left as UnaryExpression;
                if (ux.Operand is ConstantExpression)
                {
                    var val = Convert.ChangeType(((ConstantExpression)ux.Operand).Value, right.Type);
                    left = Expression.Constant(val);
                }
                return;
            }
            if (left is ConstantExpression)
            {
                var val = Convert.ChangeType(((ConstantExpression)left).Value, left.Type);
                left = Expression.Constant(val);
                return;
            }
            throw new NotImplementedException();
        }

        internal static Expression ToExpression(ParameterExpression Pr, FXExpr Expr,object paramsObject)
        {
            if(Expr is NExpr)
            {
               
                var nExpr = Expr as NExpr;
                if (nExpr.ValueType!=ConstTypeEnum.None)
                {
                    var Cx = Expression.Constant(nExpr.GetValue());
                    return Expression.Convert(Cx, typeof(object));
                }
                else
                {
                    var mb = Pr.Type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == nExpr.Value.ToLower());
                    if (mb == null)
                    {
                        throw new Exception($"'{nExpr.Value}' was not found in ${Pr.Type.FullName}");
                    }
                    var ret = Expression.MakeMemberAccess(Pr, mb);
                    return ret;
                }
            }
            if(Expr is BExpr)
            {
                return ToConditionlExpression(Pr, Expr as BExpr,paramsObject);
            }
            if (Expr is PExpr)
            {
                return ToConstExpression(Expr as PExpr, paramsObject);
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// Covert to const
        /// </summary>
        /// <param name="pExpr"></param>
        /// <param name="paramsObject"></param>
        /// <exception cref="ParameterNotFoundException"/>
        /// <returns></returns>
        public static Expression ToConstExpression(PExpr pExpr, object paramsObject)
        {
            if (paramsObject == null) return null;
            var P = paramsObject.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == pExpr.Name.ToLower());
            if (P == null)
            {
                throw new ParameterNotFoundException($"'{pExpr.Name}' was not found") { 
                
                       ParamName=pExpr.Name,
                       ParamRef=paramsObject
                };
            }
            var constVal = Expression.Constant(P.GetValue(paramsObject));
            var ret = Expression.Convert(constVal, typeof(object));
            return ret;
        }

        internal static object GetVal(Type type, string value, object paramsObject)
        {
            if (paramsObject == null) return null;
            var P = paramsObject.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == value.ToLower());
            if (P == null) return null;
            return P.GetValue(paramsObject);
        }
    }
}
