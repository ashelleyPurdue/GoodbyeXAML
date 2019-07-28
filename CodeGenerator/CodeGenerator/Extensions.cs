using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public static class Extensions
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

        /// <summary>
        /// Returns all namespaces you would need to import to represent this type
        /// without fully-qualifying it.
        /// 
        /// EG: EventHandler<DragStartedEventArgs> would yield:
        ///     * System
        ///     * System.Windows.Controls.Primitives
        /// because EventHandler belongs to System and DragStartedEventArgs belongs
        /// to System.Windows.Controls.Primitives
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static IEnumerable<string> AllReferencedNamespaces(this Type T)
        {
            yield return T.Namespace;

            Type[] genericTypeArgs = T.GetGenericArguments();
            foreach (Type genericArg in genericTypeArgs)
            {
                foreach (string ns in genericArg.AllReferencedNamespaces())
                    yield return ns;
            }
        }

        /// <summary>
        /// Why does this *not* exist by default?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="contents"></param>
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> contents)
        {
            foreach (T item in contents)
                set.Add(item);
        }

        public static void AddRange<T>(this HashSet<T> set, params T[] contents) 
            => set.AddRange((IEnumerable<T>)contents);
    }
}
