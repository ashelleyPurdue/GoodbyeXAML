using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoodbyeXAML.LambdaBinding
{
    internal static class ExpressionUtils
    {
        /// <summary>
        /// Allows easy enumeration of an expresison tree.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<Expression> TraverseExpressionTree(Expression root)
        {
            var visitor = new Visitor();
            visitor.Visit(root);
            return visitor.nodes;
        }

        public static IEnumerable<(object owner, string memberName)> GetAllPropertyAccesses(Expression expression)
        {
            return TraverseExpressionTree(expression)
                .Where(e => e is MemberExpression)
                .Select(e => (MemberExpression)e)
                .Where(me => me.Expression is MemberExpression)
                .Select(me => (GetMemberExpressionObject(me), me.Member.Name));
        }

        public static object GetMemberExpressionObject(MemberExpression me)
        {
            // Borrowed from https://stackoverflow.com/questions/1613239/getting-the-object-out-of-a-memberexpression
            // Thanks @Denis Shishkanov!
            return Expression.Lambda((MemberExpression)me.Expression)
                .Compile()
                .DynamicInvoke();
        }

        private class Visitor : ExpressionVisitor
        {
            public List<Expression> nodes = new List<Expression>();
            public override Expression Visit(Expression node)
            {
                nodes.Add(node);
                return base.Visit(node);
            }
        }
    }
}
