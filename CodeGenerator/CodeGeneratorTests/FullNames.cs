using System;
using global::System.Collections.Generic;
using global::System.Text;

namespace CodeGeneratorTests
{
    class FullNames
    {
        public const string LIST = "global::System.Collections.Generic.List";
        public const string DICTIONARY = "global::System.Collections.Generic.Dictionary";
        public const string OBJECT = "global::System.Object";
        public const string STRING = "global::System.String";
        public const string INT32 = "global::System.Int32";
        public const string EXPRESSION = "global::System.Linq.Expressions.Expression";
        public const string FUNC = "global::System.Func";
        public const string EVENT_HANDLER = "global::System.EventHandler";

        public const string CLASS_WITH_PROPERTIES = "global::CodeGeneratorTests.ClassWithProperties";
        public const string CLASS_WITH_EXTRA_USINGS = "global::CodeGeneratorTests.ClassWithExtraUsings";
        public const string CLASS_WITH_EVENTS = "global::CodeGeneratorTests.ClassWithEvents";
        public const string CLASS_WITH_NESTED_TYPES = "global::CodeGeneratorTests.ClassWithNestedTypes";

        public const string NESTED_CLASS = CLASS_WITH_NESTED_TYPES + ".NestedClass";
        public const string NESTED_GENERIC_CLASS = CLASS_WITH_NESTED_TYPES + ".NestedGenericClass";
    }
}
