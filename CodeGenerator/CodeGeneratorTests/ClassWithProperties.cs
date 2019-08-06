using System;
using System.Collections.Generic;

namespace CodeGeneratorTests
{
    public class ClassWithProperties
    {
        // Should get extension methods
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }

        public List<int> GenericProperty { get; set; }
        public List<List<List<int>>> NestedGenericProperty { get; set; }
        public Dictionary<string, int> DoubleGenericProperty { get; set; }

        // Should only get an extenion method for adding items
        public List<int> ReadOnlyList { get; private set; }

        // Should NOT get extension methods
        public int NoSetter { get; }
        public int PrivateSetter { get; private set; }
        private int PrivateProperty { get; set; }

        // Indexers get compiled to a property called "Item".
        // This "Item" property should NOT be given an extension.
        public int this[int index]
        {
            get => 0;
            set { }
        }
    }
}