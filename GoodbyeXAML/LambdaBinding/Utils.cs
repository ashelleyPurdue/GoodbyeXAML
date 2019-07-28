using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoodbyeXAML.LambdaBinding
{
    public delegate void ExpressionChangedHandler<TObserver, TResult>(TObserver observer, TResult newValue);

    public static class Utils
    {
        /// <summary>
        /// Executes the given action on the observer when
        /// the value of resultExpression changes.
        /// 
        /// This is a weak event subscription; the subscription
        /// will not prevent observer from being garbage collected.
        /// </summary>
        /// <typeparam name="TObserver"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="observer"></param>
        /// <param name="resultExpression"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TObserver WhenExpressionChanges<TObserver, TResult>
        (
            TObserver observer,
            Expression<Func<TResult>> resultExpression,
            ExpressionChangedHandler<TObserver, TResult> handler
        )
        {
            // Compile the result expression to a delegate so it can be
            // evaluated.
            Func<TResult> resultFunction = resultExpression.Compile();

            // Do an initial call of the handler now, since PropertyChanged
            // probably hasn't fired yet.
            handler(observer, resultFunction());

            // Traverse the expression tree, find any
            // INPC accesses, and subscribe to their
            // PropertyChanged event.
            var sourcePropAccesses = ExpressionUtils.GetAllPropertyAccesses(resultExpression.Body)
                .Where(access => access.owner is INotifyPropertyChanged)
                .Distinct();

            foreach ((object sourcePropOwner, string sourcePropName) in sourcePropAccesses)
            {
                WeakPropertyChangedEventUtils.WeakSubscribe(sourcePropOwner as INotifyPropertyChanged, observer, (s, e) =>
                {
                    if (e.PropertyName == sourcePropName)
                        handler(observer, resultFunction());
                });
            }

            return observer;
        }
    }
}
