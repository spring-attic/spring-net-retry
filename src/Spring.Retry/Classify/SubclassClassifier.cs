// -----------------------------------------------------------------------
// <copyright file="SubclassClassifier.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Spring.Retry.Classify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubclassClassifier<T, C> : IClassifier<T, C>
    {
        private ConcurrentDictionary<Type, C> classified = new ConcurrentDictionary<Type, C>();

        private C defaultValue = default(C);

        public SubclassClassifier() : this(default(C)) { }

        public SubclassClassifier(C defaultValue) : this(new Dictionary<Type, C>(), defaultValue) { }

        public SubclassClassifier(IDictionary<Type, C> typeMap, C defaultValue)
            : base()
        {
            this.classified = new ConcurrentDictionary<Type, C>(typeMap);
            this.defaultValue = defaultValue;
        }

        public void SetDefaultValue(C defaultValue) { this.defaultValue = defaultValue; }

        public void SetTypeMap(IDictionary<Type, C> map) { this.classified = new ConcurrentDictionary<Type, C>(map); }

        public C Classify(T classifiable)
        {
            if (classifiable == null)
            {
                return defaultValue;
            }

            var exceptionClass = (Type)classifiable.GetType();
            if (classified.ContainsKey(exceptionClass))
            {
                C outValue;
                classified.TryGetValue(exceptionClass, out outValue);
                return outValue;
            }

            // check for subclasses
            var classes = new SortedSet<Type>(classified.Keys, new ClassComparator());

            foreach (var cls in classes)
            {
                if (cls.IsAssignableFrom(exceptionClass))
                {
                    C value = classified[cls];
                    this.classified.TryAdd(exceptionClass, value);
                    return value;
                }
            }

            return defaultValue;
        }

        public C GetDefault() { return defaultValue; }

        private class ClassComparator : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                if (x.IsAssignableFrom(y))
                {
                    return 1;
                }

                return -1;
            }
        }
    }
}
