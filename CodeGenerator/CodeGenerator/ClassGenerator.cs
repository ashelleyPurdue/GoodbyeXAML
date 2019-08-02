using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGenerator
{
    public class ClassGenerator
    {
        const string EXPRESSION = "global::System.Linq.Expressions.Expression";
        const string FUNC = "global::System.Func";

        private string namespaceName;

        private List<PropertyInfo> properties = new List<PropertyInfo>();
        private List<EventInfo> events = new List<EventInfo>();
        private string className;

        /// <summary>
        /// </summary>
        /// <param name="namespaceName">The namespace of the generated class</param>
        /// <param name="targetType">The type we are generating extension methods for</param>
        public ClassGenerator(string namespaceName, Type targetType)
        {
            this.namespaceName = namespaceName;
            this.className = targetType.Name + "Extensions";
        }

        public void AddProperty(PropertyInfo p)
        {
            properties.Add(p);
        }

        public void AddEvent(EventInfo e)
        {
            events.Add(e);
        }

        public string Generate() =>
        $@"
            namespace {namespaceName}
            {{
                public static class {className}
                {{
                    {ClassBody()}
                }}
            }}
        ";

        private string ClassBody()
        {
            var builder = new StringBuilder();

            // With extensions
            foreach (PropertyInfo p in properties)
                builder.AppendLine(SingleWith(p));

            // Bind extensions
            foreach (PropertyInfo p in properties)
                builder.AppendLine(SingleBind(p));

            // Handle extensions
            foreach (EventInfo e in events)
                builder.AppendLine(SingleHandle(e));

            return builder.ToString();
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
