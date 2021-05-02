using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DynamicExpr
{
    public static class ExprBuilders
    {
        public static Expression<Func<T,bool>> Logical<T>(string StrLogicalExpr,object ParamsObject)
        {
            var expr = ToLambdaExpression(typeof(T), StrLogicalExpr, ParamsObject);
            return expr as Expression<Func<T, bool>>;
        }
        public static LambdaExpression ToLambdaExpression(
            Type type, string StrExpr,
            object ParamsObject)
        {
            var Pr = Expression.Parameter(type,"p");
            var Expr = StrExpr.FixBracket().FixSpace().ToFxExpression();
            Expression x = null;
            if(Expr is BExpr)
            {
                x= ToOperandExpression(Pr, Expr as BExpr, ParamsObject);
                return Expression.Lambda(x,Pr);
            }
            if(Expr is FExpr)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public static Expression<Func<T, dynamic>> CreateSelector<T>(string SelectorExpression, object ParamsObject)
        {
            var exprs = SelectorExpression.Replace("  "," ").SplitExp();
            var Dic = new Dictionary<string, string>();
            foreach(var x in exprs)
            {
                var fx = x.ExtractAlais();
                Dic.Add(fx[1], fx[0]);
            }
            return CreateSelector<T>(Dic, ParamsObject);
        }

        public static Expression<Func<T, dynamic>> CreateSelector<T>(Dictionary<string, string> dict, object ParamsObject)
        {
            ParameterExpression Pr = Expression.Parameter(typeof(T), "p");
            var ret = CreateNewExpr(Pr, dict, ParamsObject);
            var retX= Expression.Lambda<Func<T, dynamic>>(ret.Body, ret.Parameters[0]);
            return retX;
        }

        public static LambdaExpression CreateNewExpr(ParameterExpression Pr, Dictionary<string, string> Dict,object ParamsObject)
        {
            ParamsObject = ParamsObject ?? new { };
            var dict = new Dictionary<string, Type>();
            var argsDict = new Dictionary<string,Expression>();
            foreach(var x in Dict)
            {
                var FX= x.Value.FixBracket().FixSpace().ToFxExpression();
                var fx = ExprBuilders.ToExpression(Pr, FX, ParamsObject);
                argsDict.Add(x.Key, fx);
                dict.Add(x.Key, fx.Type);
            }
            var t = CreateNewType(dict);
            var args = new List<Expression>();
            var outputParam = Expression.Parameter(t, "n");
            var mbxs = new List<MemberInfo>();
            foreach(var x in argsDict)
            {
                //var mx = Expression.MakeMemberAccess(outputParam, t.GetProperty(x.Key));
                //var ax = Expression.Assign(mx, x.Value);
                args.Add(x.Value);
                mbxs.Add(t.GetProperty(x.Key));
            }
            var nx = Expression.New(t.GetConstructors()[0],
                    args.ToArray(),
                    mbxs.ToArray()
                );
         
            return Expression.Lambda(nx, Pr);
        }

        internal static LambdaExpression Assign(Type type, string Expr)
        {
            var Pr = Expression.Parameter(type, "p");
            var fx = Assign(Pr, Expr);
            return Expression.Lambda(fx, Pr);
        }

        public static BinaryExpression Assign(ParameterExpression Pr, string Expr)
        {
            var ms = Expr.ExtractAlais();
            var P = Pr.Type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == ms[1].ToLower());
            if(P==null)
            {
                throw new Exception($"'{ms[1]}' was not found in {Pr.Type.FullName}");
            }
            var fx = ToExpression(Pr, ms[0].FixBracket().FixSpace().ToFxExpression(), null); ;
            var mbx = Expression.MakeMemberAccess(Pr, P);
            var ret = Expression.Assign(mbx, fx);
            return ret;
        }

        private static Expression ToOperandExpression(ParameterExpression Pr, BExpr bExpr, object paramsObject)
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
                if (left.Type == typeof(string) && right.Type==typeof(string))
                {

                    var mc = BuilinFuncsMap.Concat;
                    var arr = Expression.NewArrayInit(typeof(string), left, right);
                    return Expression.Call(mc, arr);
                }
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
                return ToOperandExpression(Pr, Expr as BExpr,paramsObject);
            }
            if (Expr is PExpr)
            {
                return ToConstExpression(Expr as PExpr, paramsObject);
            }
            if(Expr is FExpr)
            {
                return ToCallExpression(Pr,Expr as FExpr, paramsObject);
            }
            throw new NotImplementedException();
        }

        public static Expression ToCallExpression(ParameterExpression Pr, FExpr fx, object paramsObject)
        {
            if(fx.Name==nameof(BuilinFuncsMap.Left))
            {
                if(fx.Args.Count<2)
                {
                    throw new ArgumentException($"'{nameof(BuilinFuncs.Left)} require 2 params'");
                }
                var mc = BuilinFuncsMap.Left;
                var mx = ToExpression(Pr, fx.Args[0], paramsObject);
                var len = ToExpression(Pr, fx.Args[1], paramsObject);
                if(len is UnaryExpression)
                {
                    if(((UnaryExpression)len).Operand is ConstantExpression)
                    {
                        var val = ((ConstantExpression)(((UnaryExpression)len).Operand)).Value;
                        val = Convert.ChangeType(val, typeof(int));
                        len = Expression.Constant(val);
                    }
                }
                return Expression.Call(mc, mx, len);
            }
            if (fx.Name == nameof(BuilinFuncsMap.Concat))
            {
                var args = new List<Expression>();
                foreach(var x in fx.Args)
                {
                    var FX = ToExpression(Pr, x, paramsObject);
                    if (FX is UnaryExpression && ((UnaryExpression)FX).Operand is ConstantExpression)
                    {
                        args.Add(((UnaryExpression)FX).Operand);
                    }
                    else
                    {
                        args.Add(FX);
                    }
                }
                var axs = Expression.NewArrayInit(typeof(string), args);
                var mc = BuilinFuncsMap.Concat;
                return Expression.Call(mc, axs);
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
            //var ret = Expression.Convert(constVal, pExpr.ReturnType);
            return constVal;
        }

        internal static object GetVal(Type type, string value, object paramsObject)
        {
            if (paramsObject == null) return null;
            var P = paramsObject.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == value.ToLower());
            if (P == null) return null;
            return P.GetValue(paramsObject);
        }
        public static Type CreateNewType(Dictionary<string,Type> Dict)
        {
            // Let's start by creating a new assembly
            AssemblyName dynamicAssemblyName = new AssemblyName("#$DynamicAsm");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("#$DynamicAsm");
            
            // Now let's build a new type
            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType(
                $"#{Guid.NewGuid().ToString()}", TypeAttributes.Public);
            foreach(var x in Dict)
            {
               var custNamePropBldr= dynamicAnonymousType.DefineProperty(x.Key,PropertyAttributes.HasDefault, x.Value,null);
                // The property set and property get methods require a special
                // set of attributes.
                MethodAttributes getSetAttr =
                    MethodAttributes.Public | MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig;
                var fKey = x.Key[0].ToString().ToLower() + x.Key.Substring(1, x.Key.Length - 1);
                FieldBuilder customerNameBldr = dynamicAnonymousType.DefineField($"{fKey}",
                                                        x.Value,
                                                        FieldAttributes.Public);

                // Define the "get" accessor method for CustomerName.
                MethodBuilder custNameGetPropMthdBldr =
                    dynamicAnonymousType.DefineMethod($"get_{x.Key}",
                                               getSetAttr,
                                               x.Value,
                                               Type.EmptyTypes);
                ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);
                MethodBuilder custNameSetPropMthdBldr =
           dynamicAnonymousType.DefineMethod($"set_{x.Key}",
                                      getSetAttr,
                                      null,
                                      new Type[] { x.Value });
                ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);
                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
                dynamicAnonymousType.DefineField(x.Key, x.Value, FieldAttributes.Public);
            }
            Type[] constructorArgs = Dict.Select(p=>p.Value).ToArray();
            var ctor = dynamicAnonymousType.DefineConstructor(MethodAttributes.Public,
                      CallingConventions.Standard, constructorArgs);
            
            // Generate IL for the method. The constructor stores its argument in the private field.
            ILGenerator myConstructorIL = ctor.GetILGenerator();
            myConstructorIL.Emit(OpCodes.Ldarg_0);
            myConstructorIL.Emit(OpCodes.Ldarg_1);
            //myConstructorIL.Emit(OpCodes.Stfld, myGreetingField);
            myConstructorIL.Emit(OpCodes.Ret);
            var ret = dynamicAnonymousType.CreateType();
            //var ret=ctor.DeclaringType;
                return ret;
        }
    }
}
