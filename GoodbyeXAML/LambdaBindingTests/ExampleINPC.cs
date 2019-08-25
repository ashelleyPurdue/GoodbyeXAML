using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using GoodbyeXAML.LambdaBinding;
using Xunit;

using AutoPropertyChanged;

namespace LambdaBindingTests
{
    public class ExampleINPC : INotifyPropertyChanged
    {
        [NotifyChanged] public int IntProperty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [Fact]
        public void WhenExpressionChanges_Updates_When_Vm_And_Observer_Are_This()
        {
            int runCount = 0;

            Utils.WhenExpressionChanges
            (
                this,
                () => this.IntProperty,
                (observer, result) => runCount++
            );

            this.IntProperty = 1337;

            Assert.Equal(2, runCount);
        }

        [Fact]
        public void WhenExpressionChanges_Updates_When_Vm_Is_This_But_Not_Observer()
        {
            int runCount = 0;
            string dummyObserver = "foo bar baz";

            Utils.WhenExpressionChanges
            (
                dummyObserver,
                () => this.IntProperty,
                (observer, result) => runCount++
            );

            this.IntProperty = 1337;

            Assert.Equal(2, runCount);
        }
    }
}
