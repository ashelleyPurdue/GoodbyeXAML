using System;
using System.Linq;
using System.Text;
using System.Reflection;
using CodeGenerator;

public static class ExtensionClassGenerator
{
    public static string GenerateExtensionClassFor(string namespaceName, Type T)
    {
        return 
        $@"
            namespace {namespaceName}
            {{
                public static class {T.Name}Extensions
                {{
                    {GeneratePropertyExtensions()}
                    {GenerateEventExtensions()}
                }}
            }}
        ";

        string GeneratePropertyExtensions()
        {
            var builder = new StringBuilder();
            var settableProperties = T
                .GetProperties()
                .Where(p => p.DeclaringType == T)   // Skip properties added by parent class
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);

            foreach (PropertyInfo p in settableProperties)
                builder.AppendLine(GenerateSinglePropertyExtension(p));

            return builder.ToString();
        }

        string GenerateEventExtensions()
        {
            var builder = new StringBuilder();
            var events = T
                .GetEvents()
                .Where(e => e.DeclaringType == T);

            foreach (EventInfo e in events)
                builder.AppendLine(GenerateSingleEventExtension(e));

            return builder.ToString();
        }

        string GenerateSinglePropertyExtension(PropertyInfo p) =>
        $@"
            public static TObject With{p.Name}<TObject>(this TObject obj, {p.PropertyType.GenericName()} value)
                where TObject : {T.Name}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        string GenerateSingleEventExtension(EventInfo e) =>
        $@"
            public static TObject Handle{e.Name}<TObject>(this TObject obj, {e.EventHandlerType.GenericName()} handler)
                where TObject : {T.Name}
            {{
                obj.{e.Name} += handler;
                return obj;
            }}
        ";
    }
}