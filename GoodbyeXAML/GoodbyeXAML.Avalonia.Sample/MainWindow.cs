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
        Content = new Panel().WithChildren
        (
            LetterCountForm()
                .WithHorizontalAlignment(HorizontalAlignment.Center)
                .WithVerticalAlignment(VerticalAlignment.Top),

            new Button()
                .OnWindowEdge(hor: HorizontalAlignment.Right, vert: VerticalAlignment.Bottom)
                .WithContent("Click Me!")
                .HandleClick((s, e) => ShowMessage("You can handle events inline using lambdas."))
        );
    }

    private StackPanel LetterCountForm()
    {
        var letterCountTextbox = new TextBox()
            .WithText("The letter count in this textbox is updated using lambda bindings.");

        return new StackPanel().WithChildren
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
            .WithSizeToContent(SizeToContent.WidthAndHeight)
            .WithMinHeight(70)
            .WithContent(new Panel().WithChildren
        (
            Text(message)
                .OnWindowEdge(vert: VerticalAlignment.Top),

            new Button()
                .OnWindowEdge(vert: VerticalAlignment.Bottom)
                .WithContent("OK")
                .HandleClick((s, e) => dialog.Close())
        ));
        
        dialog.Show();
    }
}