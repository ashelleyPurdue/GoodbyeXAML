using System;
using System.Linq;
using System.Reflection;
using Xunit;
using CodeGenerator;

namespace CodeGeneratorTests
{
    public class ClassGeneratorTests
    {
        [Fact]
        public void Generates_Usings()
        {
            Type T = typeof(ClassWithExtraUsings);

            var properties = T.GetProperties();
            var generator = new ClassGenerator("CodeGeneratorTests", "ClassWithExtraUsingsExtensions");

            foreach (PropertyInfo p in properties)
                generator.AddProperty(p);

            string expected =
            @"
                using CodeGeneratorTests;
                using GoodbyeXAML.LambdaBinding;
                using System;
                using System.Collections.Generic;
                using System.ComponentModel;
                using System.Linq.Expressions;
            ".NormalizeWhitespace();

            string actual = generator
                .UsingsSection()
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_WithIntProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("IntProperty");

            string expected =
            @"
                public static TObject WithIntProperty<TObject>(this TObject obj, Int32 value)
                    where TObject : ClassWithProperties
                {
                    obj.IntProperty = value;
                    return obj;
                }
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleWith(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_WithGenericProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("GenericProperty");

            string expected =
            @"
                public static TObject WithGenericProperty<TObject>(this TObject obj, List<Int32> value)
                    where TObject : ClassWithProperties
                {
                    obj.GenericProperty = value;
                    return obj;
                }
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleWith(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_WithNestedGenericProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("NestedGenericProperty");

            string expected =
            @"
                public static TObject WithNestedGenericProperty<TObject>(this TObject obj, List<List<List<Int32>>> value)
                    where TObject : ClassWithProperties
                {
                    obj.NestedGenericProperty = value;
                    return obj;
                }
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleWith(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_WithDoubleGenericProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("DoubleGenericProperty");

            string expected =
            @"
                public static TObject WithDoubleGenericProperty<TObject>(this TObject obj, Dictionary<String, Int32> value)
                    where TObject : ClassWithProperties
                {
                    obj.DoubleGenericProperty = value;
                    return obj;
                }
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleWith(p)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Generates_BindIntProperty()
        {
            PropertyInfo p = typeof(ClassWithProperties)
                .GetProperty("IntProperty");

            string expected =
            @"
                public static TObject BindIntProperty<TObject>(this TObject obj, Expression<Func<Int32>> resultExpression)
                    where TObject : ClassWithProperties
                {
                    Utils.WhenExpressionChanges(obj, resultExpression, (o, result) =>
                    {
                        o.IntProperty = result;
                    });

                    return obj;
                }
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
            @"
                public static TObject HandleObjEvent<TObject>(this TObject obj, EventHandler<Object> handler)
                    where TObject : ClassWithEvents
                {
                    obj.ObjEvent += handler;
                    return obj;
                }
            ".NormalizeWhitespace();

            string actual = ClassGenerator
                .SingleHandle(e)
                .NormalizeWhitespace();

            Assert.Equal(expected, actual);
        }
    }
}
