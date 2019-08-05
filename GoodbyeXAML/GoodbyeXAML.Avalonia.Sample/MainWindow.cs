using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Layout;

using GoodbyeXAML.Avalonia.Shared;
using System.ComponentModel;
using static GoodbyeXAML.Avalonia.Sample.ControlFactories;
using static GoodbyeXAML.Avalonia.Sample.PropertyMixins;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Content = new Panel()._Children
        (
            LetterCountForm()
                ._HorizontalAlignment(HorizontalAlignment.Center)
                ._VerticalAlignment(VerticalAlignment.Top),

            new Button()
                .OnWindowEdge(hor: HorizontalAlignment.Right, vert: VerticalAlignment.Bottom)
                ._Content("Click Me!")
                ._Click((s, e) => ShowMessage("You can handle events inline using lambdas."))
        );
    }

    private StackPanel LetterCountForm()
    {
        var letterCountTextbox = new TextBox()
            ._Text("The letter count in this textbox is updated using lambda bindings.");

        return new StackPanel()._Children
        (
            Text("You can bind properties to lambda expressions."),
            letterCountTextbox,
            Text(() => $"There are {letterCountTextbox.Text.Length} letters in that sentence.")
        );
    }

    private void ShowMessage(string message)
    {
        Window dialog = new Window();   // We must declare and assign dialog before we can
                                        // use it in the button's HandleClick closure.

        dialog
            ._SizeToContent(SizeToContent.WidthAndHeight)
            ._MinHeight(70)
            ._Content(new Panel()._Children
        (
            Text(message)
                .OnWindowEdge(vert: VerticalAlignment.Top),

            new Button()
                .OnWindowEdge(vert: VerticalAlignment.Bottom)
                ._Content("OK")
                ._Click((s, e) => dialog.Close())
        ));
        
        dialog.Show();
    }
}