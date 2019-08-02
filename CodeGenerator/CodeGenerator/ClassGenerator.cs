using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGenerator
{
    public static class ClassGenerator
    {
        const string EXPRESSION = "global::System.Linq.Expressions.Expression";
        const string FUNC = "global::System.Func";

        public static string Generate(string namespaceName, Type targetType) =>
        $@"
            namespace {namespaceName}
            {{
                public static class {targetType.Name}Extensions
                {{
                    {ClassBody(targetType)}
                }}
            }}
        ";

        private static string ClassBody(Type T)
        {
            var settableProperties = T
                .GetProperties()
                .Where(p => p.DeclaringType == T)   // Skip properties added by parent class
                .Where(p => p.CanWrite && p.SetMethod.IsPublic)
                .Where(p => p.GetIndexParameters().Length == 0);    // Skip indexers

            var events = T
                .GetEvents()
                .Where(e => e.DeclaringType == T);

            return new string[0]
                .Concat(settableProperties.Select(SingleWith))
                .Concat(settableProperties.Select(SingleBind))
                .Concat(events.Select(SingleHandle))
                .Aggregate("", (current, next) => current + next + '\n');
        }

        public static string SingleWith(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"With{p.Name}", p.PropertyType.GenericFullName(), "value", p.DeclaringType)}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        public static string SingleBind(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"Bind{p.Name}", $"{EXPRESSION}<{FUNC}<{p.PropertyType.GenericFullName()}>>", "resultExpression", p.DeclaringType)}
            {{
                GoodbyeXAML.LambdaBinding.Utils.WhenExpressionChanges(obj, resultExpression, (o, result) =>
                {{
                    o.{p.Name} = result;
                }});

                return obj;
            }}
        ";

        public static string SingleHandle(EventInfo e) =>
        $@"
            public static {FunctionSignature($"Handle{e.Name}", e.EventHandlerType.GenericFullName(), "handler", e.DeclaringType)}
            {{
                obj.{e.Name} += handler;
                return obj;
            }}
        ";

        public static string FunctionSignature(string funcName, string paramType, string paramName, Type T) => IsValidGenericConstraint(T)
            ? $"TObject {funcName}<TObject>(this TObject obj, {paramType} {paramName}) where TObject : {T.GenericFullName()}"
            : $"{T.GenericFullName()} {funcName}(this {T.GenericFullName()} obj, {paramType} {paramName})";

        private static bool IsValidGenericConstraint(Type t) =>
            (t.IsInterface) ||
            (t.IsGenericTypeParameter) ||
            (!t.IsSealed);
    }
}
