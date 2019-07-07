using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneratorTests
{
    public static class Utils
    {
        /// <summary> Normalizes whitespace in a C# code string to enable comparison. </summary>
        public static string NormalizeWhitespace(this string code)
        {
            // The only whitespace we permit are spaces
            char[] charsToRemove = new[]
            {
                '\n',
                '\r',
                '\t'
            };
            foreach (char c in charsToRemove)
                code = code.Replace(c, ' ');

            // Reduce it so there's never more than one consecutive space.
            int prevLen = -1;
            while (code.Length != prevLen)   // Keep going until it stops changing.
            {
                prevLen = code.Length;
                code = code.Replace("  ", " ");
            }

            return code;
        }
    }
}