using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodbyeXAML.LambdaBinding
{
    internal class WeakKeyDictionary<TKey, TValue>
    {
        private Dictionary<WeakReference<TKey>, TValue> innerDict = new Dictionary<WeakReference<TKey>, TValue>();

        public TValue this[TKey key]
        {
            get => innerDict[Weak(key)];
            set => innerDict[Weak(key)] = value;
        }

        public ICollection<TKey> Keys => innerDict
            .Keys
            .Where(k => k.IsAlive)
            .Select(k => k.Target)
            .ToArray();

        public void Add(TKey key, TValue value)
        {
            CleanDeadKeys();
            innerDict.Add(Weak(key), value);
        }

        public bool TryGetValue(TKey key, out TValue value)
            => innerDict.TryGetValue(Weak(key), out value);

        /// <summary>
        /// Deletes all dead keys
        /// </summary>
        private void CleanDeadKeys()
        {
            // I really wanted to use LINQ here, but this routine will be
            // called every time we access the dictionary, so it actually
            // needs to be efficient :(

            var deadKeys = new List<WeakReference<TKey>>();
            foreach (var key in innerDict.Keys)
            {
                if (!key.IsAlive)
                    deadKeys.Add(key);
            }

            foreach (var key in deadKeys)
                innerDict.Remove(key);
        }

        private WeakReference<TKey> Weak(TKey key) => new WeakReference<TKey>(key);
    }

    internal class WeakReference<T> : WeakReference
    {
        private int originalHashCode;

        public WeakReference(T target) : base(target)
            => originalHashCode = target.GetHashCode();

        public new T Target
        {
            get => (T)base.Target;
            set => base.Target = value;
        }

        public override bool Equals(object obj)
        {
            var other = (WeakReference<T>)obj;

            if (!IsAlive || !other.IsAlive)
                return false;

            return Target.Equals(other.Target);
        }

        public override int GetHashCode() => originalHashCode;
    }
}
