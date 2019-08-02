using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using CodeGenerator;
using static CodeGeneratorTests.FullNames;

namespace CodeGeneratorTests
{
    public class GenericFullNameTests
    {
        [Theory]
        [InlineData(typeof(int), INT32)]
        [InlineData(typeof(string), STRING)]
        [InlineData(typeof(List<int>), LIST+"<"+INT32+">")]
        [InlineData(typeof(List<List<int>>), LIST+"<"+LIST+"<"+INT32+">"+">")]
        [InlineData(typeof(Dictionary<string, int>), DICTIONARY+"<"+STRING+", "+INT32+">")]
        [InlineData(typeof(ClassWithProperties), CLASS_WITH_PROPERTIES)]
        [InlineData(typeof(ClassWithNestedTypes.ExampleEnum), CLASS_WITH_NESTED_TYPES+".ExampleEnum")]
        [InlineData(typeof(ClassWithNestedTypes.NestedClass), NESTED_CLASS)]
        [InlineData(typeof(ClassWithNestedTypes.NestedGenericClass<int>), NESTED_GENERIC_CLASS+"<"+INT32+">")]
        public void Test_GenericFullName(Type T, string expected)
        {
            string actual = T.GenericFullName();
            Assert.Equal(expected, actual);
        }
    }
}
