using System;
using Xunit;
using CodeGenerator;

namespace CodeGeneratorTests
{
    public class ExtensionMethodGeneratorTests
    {
        [Fact]
        public void ClassWithProperties_Works()
        {
            string expected = 
            @"
                public static class ClassWithPropertiesExtensions
                {
                    public static TObject WithStringProperty<TObject>(this TObject obj, String value)
                        where TObject : ClassWithProperties
                    {
                        obj.StringProperty = value;
                        return obj;
                    }

                    public static TObject WithIntProperty<TObject>(this TObject obj, Int32 value)
                        where TObject : ClassWithProperties
                    {
                        obj.IntProperty = value;
                        return obj;
                    }
                }
            ".NormalizeWhitespace();

            string actual = ExtensionMethodGenerator.GenerateExtensionClassFor(typeof(ClassWithProperties))
                .NormalizeWhitespace();

            Assert.Equal
            (
                expected,
                actual
            );
        }
    }
}
