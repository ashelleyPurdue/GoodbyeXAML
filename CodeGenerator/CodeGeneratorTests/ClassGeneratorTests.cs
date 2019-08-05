using System;
using System.Linq;
using System.Reflection;
using Xunit;
using CodeGenerator;
using static CodeGeneratorTests.FullNames;

namespace CodeGeneratorTests
{
    public class ClassGeneratorTests
    {

        [Fact]
        public void Generates_WithIntProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("IntProperty");

            string expected =
            @$"
                public static TObject _IntProperty<TObject>(this TObject obj, {INT32} value)
                    where TObject : {CLASS_WITH_PROPERTIES}
                {{
                    obj.IntProperty = value;
                    return obj;
                }}
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleSet(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_BindIntProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("IntProperty");

            string expected =
            @$"
                public static TObject _IntProperty<TObject>(this TObject obj, {EXPRESSION}<{FUNC}<{INT32}>> resultExpression)
                    where TObject : {CLASS_WITH_PROPERTIES}
                {{
                    GoodbyeXAML.LambdaBinding.Utils.WhenExpressionChanges(obj, resultExpression, (o, result) =>
                    {{
                        o.IntProperty = result;
                    }});

                    return obj;
                }}
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleBind(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_HandleObjEvent()
        {
            EventInfo e = typeof(ClassWithEvents)
                .GetEvent("ObjEvent");

            string expected =
            @$"
                public static TObject _ObjEvent<TObject>(this TObject obj, {EVENT_HANDLER}<{OBJECT}> handler)
                    where TObject : {CLASS_WITH_EVENTS}
                {{
                    obj.ObjEvent += handler;
                    return obj;
                }}
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleHandle(e)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("PrivateProperty")]
        [InlineData("PrivateSetter")]
        [InlineData("NoSetter")]
        [InlineData("Item")]    // "Item" is the name of a hidden property that indexers get compiled to.
        public void Doesnt_Generate_Extensions_For_Private_Properties(string propertyName)
        {
            string generatedCode = ClassGenerator.Generate("dontcare", typeof(ClassWithProperties));
            Assert.DoesNotContain("_" + propertyName, generatedCode);
        }
    }
}
