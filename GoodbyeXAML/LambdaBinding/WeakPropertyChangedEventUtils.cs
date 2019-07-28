using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GoodbyeXAML.LambdaBinding
{
    using HandlerList = List<PropertyChangedEventHandler>;
    using INPC = INotifyPropertyChanged;

    internal static class WeakPropertyChangedEventUtils
    {
        private static WeakKeyDictionary<INPC, WeakKeyDictionary<object, HandlerList>> dispatchTable
            = new WeakKeyDictionary<INPC, WeakKeyDictionary<object, HandlerList>>();

        /// <summary>
        /// Subscribes to the PropertyChanged event in a manner that avoids
        /// memory leaks.
        /// </summary>
        /// <param name="vm"> The INPC that we want to subscribe to.</param>
        /// <param name="targetObject"> The object whose property we'll be updating.</param>
        /// <param name="handler"> The action to be performed when the event triggers. </param>
        public static void WeakSubscribe
        (
            INPC vm,
            object targetObject,
            PropertyChangedEventHandler handler
        )
        {
            // If this vm is not in the weak subscription system yet, add it.
            WeakKeyDictionary<object, HandlerList> vmTableEntry;
            if (!dispatchTable.TryGetValue(vm, out vmTableEntry))
            {
                vmTableEntry = new WeakKeyDictionary<object, HandlerList>();
                dispatchTable.Add(vm, vmTableEntry);
                vm.PropertyChanged += DispatchWeakEvent;
            }

            // If a handler list for this object doesn't exist yet, make one.
            HandlerList handlerList;
            if (!vmTableEntry.TryGetValue(targetObject, out handlerList))
            {
                handlerList = new HandlerList();
                vmTableEntry.Add(targetObject, handlerList);
            }

            // Add the handler to the list.
            handlerList.Add(handler);
        }

        private static void DispatchWeakEvent(object sender, PropertyChangedEventArgs args)
        {
            // Look up this vm in the dispatch table
            var vm = (INPC)sender;
            var vmTableEntry = dispatchTable[vm];

            // Call all the handlers
            foreach (var targetObject in vmTableEntry.Keys)
            {
                foreach (var handler in vmTableEntry[targetObject])
                    handler(sender, args);
            }
        }
    }
}
