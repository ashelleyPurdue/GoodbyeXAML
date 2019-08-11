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
        private void ExpectGeneratedClassContainsExtension<TMemberInfo>
        (
            Type targetType,
            Func<Type, string, BindingFlags, TMemberInfo> memberGetter,
            Func<TMemberInfo, string> extensionGenerator,
            string memberName,
            bool shouldContain = true
        )
        {
            var bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance;

            TMemberInfo memberInfo = memberGetter(targetType, memberName, bindingFlags);

            string expectedMethod = extensionGenerator(memberInfo)
                .NormalizeWhitespace();

            string generatedClass = ClassGenerator
                .Generate("dontcare", targetType)
                .NormalizeWhitespace();

            Assert.Equal(generatedClass.Contains(expectedMethod), shouldContain);
        }

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
        [InlineData("IntProperty")]
        [InlineData("StringProperty")]
        [InlineData("GenericProperty")]
        [InlineData("ReadOnlyList", false)]
        [InlineData("PrivateProperty", false)]
        [InlineData("PrivateSetter", false)]
        [InlineData("NoSetter", false)]
        [InlineData("Item", false)]     // "Item" is the name of a hidden property that indexers get compiled to.
        public void Generates_Setter_Extensions_For(string propertyName, bool shouldGenerate = true) =>
            ExpectGeneratedClassContainsExtension
            (
                typeof(ClassWithProperties),
                (T, name, flags) => T.GetProperty(name, flags),
                ClassGenerator.SingleSet,
                propertyName,
                shouldGenerate
            );

        [Theory]
        [InlineData("IntProperty")]
        [InlineData("StringProperty")]
        [InlineData("GenericProperty")]
        [InlineData("ReadOnlyList", false)]
        [InlineData("PrivateProperty", false)]
        [InlineData("PrivateSetter", false)]
        [InlineData("NoSetter", false)]
        [InlineData("Item", false)]     // "Item" is the name of a hidden property that indexers get compiled to.
        public void Generates_Binding_Extensions_For(string propertyName, bool shouldGenerate = true) =>
            ExpectGeneratedClassContainsExtension
            (
                typeof(ClassWithProperties),
                (T, name, flags) => T.GetProperty(name, flags),
                ClassGenerator.SingleBind,
                propertyName,
                shouldGenerate
            );

        [Theory]
        [InlineData("ObjEvent")]
        [InlineData("ActionEvent")]
        [InlineData("LocalDelegateEvent")]
        [InlineData("PrivateEvent", false)]
        [InlineData("ProtectedEvent", false)]
        public void Generates_Event_Extensions_For(string eventName, bool shouldGenerate = true) =>
            ExpectGeneratedClassContainsExtension
            (
                typeof(ClassWithEvents),
                (T, name, flags) => T.GetEvent(name, flags),
                ClassGenerator.SingleHandle,
                eventName,
                shouldGenerate
            );

        [Theory]
        [InlineData("ReadOnlyList")]
        [InlineData("DirectIList")]
        public void Generates_IList_Extensions_For(string propertyName, bool shouldGenerate = true) =>
            ExpectGeneratedClassContainsExtension
            (
                typeof(ClassWithProperties),
                (T, name, flags) => T.GetProperty(name, flags),
                ClassGenerator.SingleListAdd,
                propertyName,
                shouldGenerate
            );

        [Fact]
        public void Generates_AddItem_Extensions_For_ILists()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("ReadOnlyList");

            string expected =
            $@"
                public static TObject _ReadOnlyList<TObject>(this TObject obj, params {INT32}[] items)
                    where TObject : {CLASS_WITH_PROPERTIES}
                {{
                    foreach (var i in items)
                        obj.ReadOnlyList.Add(i);
                    return obj;
                }}
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleListAdd(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }
    }
}
