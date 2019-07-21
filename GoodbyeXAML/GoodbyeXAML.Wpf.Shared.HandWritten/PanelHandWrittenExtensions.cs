using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoodbyeXAML.Wpf.Shared
{
    public static class PanelHandWrittenExtensions
    {
        public static TObject WithChildren<TObject>(this TObject panel, params UIElement[] children)
            where TObject : Panel
        {
            foreach (var child in children)
                panel.Children.Add(child);

            return panel;
        }
    }
}
