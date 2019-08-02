using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGenerator
{
    public class ClassGenerator
    {
        private HashSet<string> namespaces = new HashSet<string>();
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

            namespaces.AddRange
            (
                "GoodbyeXAML.LambdaBinding",
                "System.Linq.Expressions",
                "System",
                targetType.Namespace
            );
        }

        public void AddProperty(PropertyInfo p)
        {
            namespaces.AddRange(p.PropertyType.AllReferencedNamespaces());
            namespaces.Add(p.DeclaringType.Namespace);
            properties.Add(p);
        }

        public void AddEvent(EventInfo e)
        {
            namespaces.AddRange(e.EventHandlerType.AllReferencedNamespaces());
            events.Add(e);
        }

        public string Generate() =>
        $@"
            {UsingsSection()}

            namespace {namespaceName}
            {{
                public static class {className}
                {{
                    {ClassBody()}
                }}
            }}
        ";

        public string UsingsSection()
        {
            var builder = new StringBuilder();
            var sortedNamespaces = namespaces
                .OrderBy(s => s);   // Sort it alphabetically so the order is deterministic for the unit tests.

            foreach (string ns in sortedNamespaces)
                builder.AppendLine($"using {ns};");

            return builder.ToString();
        }

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
            public static {FunctionSignature($"With{p.Name}", p.PropertyType.GenericName(), "value", p.DeclaringType)}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        public static string SingleBind(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"Bind{p.Name}", $"Expression<Func<{p.PropertyType.GenericName()}>>", "resultExpression", p.DeclaringType)}
            {{
                Utils.WhenExpressionChanges(obj, resultExpression, (o, result) =>
                {{
                    o.{p.Name} = result;
                }});

                return obj;
            }}
        ";

        public static string SingleHandle(EventInfo e) =>
        $@"
            public static {FunctionSignature($"Handle{e.Name}", e.EventHandlerType.GenericName(), "handler", e.DeclaringType)}
            {{
                obj.{e.Name} += handler;
                return obj;
            }}
        ";

        public static string FunctionSignature(string funcName, string paramType, string paramName, Type T) => IsValidGenericConstraint(T)
            ? $"TObject {funcName}<TObject>(this TObject obj, {paramType} {paramName}) where TObject : {T.Name}"
            : $"{T.Name} {funcName}(this {T.Name} obj, {paramType} {paramName})";

        private static bool IsValidGenericConstraint(Type t) =>
            (t.IsInterface) ||
            (t.IsGenericTypeParameter) ||
            (!t.IsSealed);
    }
}
