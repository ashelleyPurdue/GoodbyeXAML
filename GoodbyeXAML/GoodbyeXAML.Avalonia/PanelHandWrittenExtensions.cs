using System;

using Avalonia;
using Avalonia.Controls;

namespace GoodbyeXAML.Avalonia.Shared
{
    public static class PanelHandwrittenExtensions
    {
        public static TPanel WithChildren<TPanel>(this TPanel panel, params IControl[] children)
            where TPanel : Panel
        {
            panel.Children.AddRange(children);
            return panel;
        }
    }
}