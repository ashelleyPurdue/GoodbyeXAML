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
            string withCharsRemoved = new string
            (
                code
                    .Where(c => c != '\n')
                    .Where(c => c != '\r')
                    .Where(c => c != '\t')
                    .ToArray()
            );

            // Reduce it so there's never more than one consecutive space.
            int prevLen = -1;
            string reduced = withCharsRemoved;
            while (reduced.Length != prevLen)   // Keep going until it stops changing.
            {
                prevLen = reduced.Length;
                reduced = reduced.Replace("  ", " ");
            }

            return reduced;
        }
    }
}