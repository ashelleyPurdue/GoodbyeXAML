using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorTests
{
    public class ClassWithNestedTypes
    {
        public enum ExampleEnum
        {
            foo,
            bar,
            baz,
            fizz,
            buzz
        }

        public class NestedClass { }
        public class NestedGenericClass<T> { }
    }
}
