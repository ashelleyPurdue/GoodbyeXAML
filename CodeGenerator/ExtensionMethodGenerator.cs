using System;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeGenerator
{
    public static class ExtensionMethodGenerator
    {
        public static string GenerateExtensionClassFor(Type T) => 
        $@"
            public static class {T.Name}Extensions
            {{
                {GeneratePropertyExtensions(T)}
                {GenerateEventExtensions(T)}
            }}
        ";

        private static string GeneratePropertyExtensions(Type T)
        {
            var builder = new StringBuilder();
            var settableProperties = T
                .GetProperties()
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);

            foreach (PropertyInfo p in settableProperties)
                builder.AppendLine(GenerateSinglePropertyExtension(T, p));

            return builder.ToString();
        }

        private static string GenerateEventExtensions(Type T)
        {
            var builder = new StringBuilder();

            foreach (EventInfo e in T.GetEvents())
                builder.AppendLine(GenerateSingleEventExtension(T, e));

            return builder.ToString();
        }

        private static string GenerateSinglePropertyExtension(Type T, PropertyInfo p) =>
        $@"
            public static TObject With{p.Name}<TObject>(this TObject obj, {p.PropertyType.GenericName()} value)
                where TObject : {T.Name}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        private static string GenerateSingleEventExtension(Type T, EventInfo e) =>
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
