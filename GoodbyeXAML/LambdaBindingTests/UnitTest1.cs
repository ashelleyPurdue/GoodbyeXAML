using System;
using GoodbyeXAML.LambdaBinding;
using Xunit;

namespace LambdaBindingTests
{
    public class UnitTest1
    {
        [Fact]
        public void WhenExpressionChanges_Updates_Immediately_When_Called()
        {
            var vm = new ExampleINPC();
            bool ranOnce = false;

            Utils.WhenExpressionChanges
            (
                vm,
                () => vm.IntProperty,
                (observer, result) => ranOnce = true
            );

            Assert.True(ranOnce);
        }

        [Fact]
        public void WhenExpressionChanges_Updates_When_Single_Observed_PropertyChanges()
        {
            var vm = new ExampleINPC();
            int runCount = 0;

            Utils.WhenExpressionChanges
            (
                vm,
                () => vm.IntProperty,
                (observer, result) => runCount++
            );

            vm.IntProperty = 1337;

            Assert.Equal(2, runCount);
        }
    }
}
