using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public static class TypeExtensions
    {
        public static string GenericName(this Type T)
        {
            if (!T.IsGenericType)
                return T.Name;

            // The compiler mangles T's name to something like EventHandler`1.
            // We need to turn it back into something like EventHandler<Object>.

            // Chop off the stuff after the tilde.
            string beforeTilde = new string
            (
                T.Name
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

                builder.Append(genericArg.GenericName());   // Lookin' at YOU, Mr. IEnumerable<Dictionary<Func<string>, List<int>>>
            }
            builder.Append(">");

            return builder.ToString();
        }
    }
}
