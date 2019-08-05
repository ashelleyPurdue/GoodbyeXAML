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
        public static TextBlock Text(string text) => new TextBlock()
            .WithText(text);

        public static TextBlock Text(Expression<Func<string>> textBinding) => new TextBlock()
            .BindText(textBinding);
    }
}
