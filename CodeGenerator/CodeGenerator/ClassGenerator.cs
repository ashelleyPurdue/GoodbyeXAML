using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var properties = T
                .GetProperties()
                .Where(p => p.DeclaringType == T)   // Skip properties added by parent class
                .Where(p => p.GetIndexParameters().Length == 0);    // Skip indexers


            var settableProperties = properties
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);

            var iListProperties = properties    // Get all properties that implement IList<something>
                .Where(p => GetIListType(p.PropertyType) != null)
                .Where(p => !IsReadOnlyCollection(p.PropertyType));    // HACK: Exclude ReadOnlyCollection
                                                                       // For some reason it implements IList,
                                                                       // even though it can't be added to -.-

            var events = T
                .GetEvents()
                .Where(e => e.DeclaringType == T);

            return new string[0]
                .Concat(settableProperties.Select(SingleSet))
                .Concat(settableProperties.Select(SingleBind))
                .Concat(iListProperties.Select(SingleListAdd))
                .Concat(events.Select(SingleHandle))
                .Aggregate("", (current, next) => current + next + '\n');

            bool IsReadOnlyCollection(Type T)
            {
                if (T == null)
                    return false;

                Type readOnlyCollectionDef = typeof(ReadOnlyCollection<int>).GetGenericTypeDefinition();
                if (T.IsGenericType && T.GetGenericTypeDefinition() == readOnlyCollectionDef)
                    return true;

                return IsReadOnlyCollection(T.BaseType);
            }
        }

        public static string SingleSet(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"_{p.Name}", p.PropertyType.GenericFullName(), "value", p.DeclaringType)}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        public static string SingleBind(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"_{p.Name}", $"{EXPRESSION}<{FUNC}<{p.PropertyType.GenericFullName()}>>", "resultExpression", p.DeclaringType)}
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
            public static {FunctionSignature($"_{e.Name}", e.EventHandlerType.GenericFullName(), "handler", e.DeclaringType)}
            {{
                obj.{e.Name} += handler;
                return obj;
            }}
        ";

        /// <summary>
        /// Gets the generic version of IList<something> that T implements.
        /// If it doens't implement something deriving from IList<something>,
        /// returns null.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        private static Type GetIListType(Type T)
        {
            Type iList = typeof(IList<int>)
                .GetGenericTypeDefinition();

            if (T.IsGenericType && T.GetGenericTypeDefinition() == iList)
                return T;

            return T
                .GetInterfaces()
                .Select(GetIListType)
                .FirstOrDefault(i => i != null);
        }

        public static string SingleListAdd(PropertyInfo p)
        {
            // Get the name of 'T' in IList<T>
            string genericTypeParam = GetIListType(p.PropertyType)
                .GetGenericArguments()
                .First()
                .GenericFullName();

            return
            $@"
                public static {FunctionSignature($"_{p.Name}", $"params {genericTypeParam}[]", "items", p.DeclaringType)}
                {{
                    foreach (var i in items)
                        obj.{p.Name}.Add(i);
                    return obj;
                }}
            ";
        }

        public static string FunctionSignature(string funcName, string paramType, string paramName, Type T) => IsValidGenericConstraint(T)
            ? $"TObject {funcName}<TObject>(this TObject obj, {paramType} {paramName}) where TObject : {T.GenericFullName()}"
            : $"{T.GenericFullName()} {funcName}(this {T.GenericFullName()} obj, {paramType} {paramName})";

        private static bool IsValidGenericConstraint(Type t) =>
            (t.IsInterface) ||
            (t.IsGenericTypeParameter) ||
            (!t.IsSealed);
    }
}
