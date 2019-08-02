using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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

            // Add all the arguments.
            var builder = new StringBuilder();
            builder.Append(beforeTilde + "<");

            for (int i = 0; i < T.GenericTypeArguments.Length; i++)
            {
                Type genericArg = T.GenericTypeArguments[i];

                if (i != 0)
                    builder.Append(", ");

                builder.Append(genericArg.GenericFullName());   // Gotta go recursive.  Lookin' at YOU, Mr. IEnumerable<Dictionary<Func<string>, List<int>>>
            }
            builder.Append(">");

            return builder.ToString();
        }
    }
}
