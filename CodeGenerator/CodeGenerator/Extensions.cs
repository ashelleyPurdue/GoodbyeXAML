using System;
using System.Linq;

namespace CodeGenerator
{
    public static class Extensions
    {
        public static string GenericFullName(this Type T)
        {
            string fullName = T.FullName;
            fullName = fullName.Replace('+', '.');     // Undo the mangling of nested types
            fullName = "global::" + fullName;          // Prevent edge cases where the current namespace is the same as one of GoodbyeXAML's namespaces.

            if (!T.IsGenericType)
                return fullName;

            // The compiler mangles T's name to something like EventHandler`1.
            // We need to turn it back into something like EventHandler<Object>.

            // Chop off the stuff after the tilde.
            string beforeTilde = new string
            (
                fullName
                    .TakeWhile(c => c != '`')
                    .ToArray()
            );

            var commaSeparatedArgs = T.GenericTypeArguments
                 .Select(a => a.GenericFullName())
                 .Aggregate("", (c, n) => c + n + ", ")
                 .TrimEnd(',', ' ');    // Remove that stupid comma at the end.  Don't you LOVE printing lists?

            return $"{beforeTilde}<{commaSeparatedArgs}>";
        }
    }
}
