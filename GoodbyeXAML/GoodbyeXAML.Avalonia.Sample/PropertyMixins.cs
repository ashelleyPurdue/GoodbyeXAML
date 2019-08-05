using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Layout;
using GoodbyeXAML.Avalonia.Shared;

namespace GoodbyeXAML.Avalonia.Sample
{
    public static class PropertyMixins
    {
        public static TLayoutable OnWindowEdge<TLayoutable>
        (
            this TLayoutable obj,
            HorizontalAlignment hor = HorizontalAlignment.Center,
            VerticalAlignment vert = VerticalAlignment.Center,
            double margin = 8
        ) where TLayoutable : Layoutable => obj
            .WithHorizontalAlignment(hor)
            .WithVerticalAlignment(vert)
            .WithMargin(new Thickness(margin));
    }
}
