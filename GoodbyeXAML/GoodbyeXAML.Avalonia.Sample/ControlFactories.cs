using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Avalonia.Controls;
using GoodbyeXAML.Avalonia.Shared;

namespace GoodbyeXAML.Avalonia.Sample
{
    public static class ControlFactories
    {
        public static TextBlock Label(string text) => new TextBlock()
            ._Text(text);

        public static TextBlock Label(Expression<Func<string>> textBinding) => new TextBlock()
            ._Text(textBinding);
    }
}
