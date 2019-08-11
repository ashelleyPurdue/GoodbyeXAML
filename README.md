# Installing
Add one of the following nuget packages to your project:
* `GoodbyeXAML.Wpf.Framework` for the WPF .Net Framework version
* `GoodbyeXAML.Wpf.Core` for the WPF .Net Core version
* `GoodbyeXAML.Avalonia` for the Avalonia version

# What is GoodbyeXAML?

GoodbyeXAML is a collection of extension methods that lets helps you lay out WPF
or Avalonia UIs using declarative C#, rather than XAML.

For example, here's a simple "Hello world" window in XAML:

```XML
    <Window x:Class="MyApp.MainWindow"
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                xmlns:local="clr-namespace:MyApp"
                mc:Ignorable="d">
        <TextBlock Text="Hello world" />
    </Window>
```

With GoodbyeXAML, we can create the same window in pure C#:

```C#
    using System.Windows;
    using System.Windows.Controls;
    using GoodbyeXAML.Wpf.Shared;

    namespace MyApp
    {
        public class MainWindow : Window
        {
            public MainWindow()
            {
                this.Content = new TextBlock()
                    ._Text("Hello world!");
            }
        }
    }
```

`_Text` is an extension method that sets TextBlock's `Text` property.
Every property on every Control has a corresponding extension method that sets
its value and returns the Control you called it on.  That means you can chain
multiple calls like this:

```C#
new TextBlock()
    ._Text("Hello world!")
    ._Background(Brushes.Black)
    ._Foreground(Brushes.White);
```

Using this technique, you can create trees of Controls just like you can in XAML:

```C#
new StackPanel()
    ._Orientation(Orientation.Vertical)
    ._HorizontalAlignment(HorizontalAlignment.Center)
    ._VerticalAlignment(VerticalAlignment.Center)
    ._Children
    (
        new TextBlock()._Text("Hello"),
        new TextBlock()._Text("World"),
        new StackPanel()
            .Orientation(Orientation.Horizontal)
            ._Children
            (
                new TextBlock()._Text("I'm "),
                new TextBlock()._Text("a "),
                new TextBlock()._Text("nested "),
                new TextBlock()._Text("StackPanel!")
            )
    );
```

Unlike with XAML, though, you have access to all of the language constructs
built into C#.  For example, we can extract `new TextBlock()._Text("Blah")` into
a separate function, since we keep repeating it so much:

```C#
public static TextBlock Label(string text)
{
    return new TextBlock()
        ._Text(text);
}
```

Or preferrably, we can use C#'s alternate syntax:
```C#
public static TextBlock Label(string text) => new TextBlock()
    ._Text(text);
```

Now our earlier code can be refactored to look like:
```C#
new StackPanel()
    ._Orientation(Orientation.Vertical)
    ._HorizontalAlignment(HorizontalAlignment.Center)
    ._VerticalAlignment(VerticalAlignment.Center)
    ._Children
    (
        Label("Hello"),
        Label("World"),
        new StackPanel()
            .Orientation(Orientation.Horizontal)
            ._Children
            (
                Label("I'm "),
                Label("a "),
                Label("nested "),
                Label("StackPanel!")
            )
    );
```

If we later want to change all of those TextBlocks (for example, to give them
a dark theme), we only need to change the `Label` function.  It is, after all,
just a function!

```C#
public static TextBlock Label(string text) => new TextBlock()
    ._Text(text)
    ._Background(Brushes.Black)
    ._Foreground(Brushes.White);
```

Or maybe we don't want them to be TextBlocks after all-- maybe we want them
to be buttons!  Why?  Because I said so, that's why!

```C#
public static TextBlock Label(string text) => new Button()
    ._Content(text)
    ._Click((s, e) => MessageBox.Show(text))
```

Properties aren't the only things that have these extenion methods-- all public
facing events do too!  And because we're using C#, we can use a lambda expression 
as our event handler.  That way we don't need to write all of the usual
boilerplate just for a one-line event handler.

Of course, you can still use a delegate instead of a lambda expression, just like 
you would in XAML:

```C#
public static TextBlock Label(string text) => new Button()
    ._Content(text)
    ._Click(Label_Click)

private static void Label_Click(object sender, RoutedEventArgs args)
{
    var btn = (Button)sender;
    var text = (string)(btn.Content);

    MessageBox.Show(text);
}
```

I hope now you can see just how flexible this technique is---far moreso than
XAML!


# Lambda binding
TODO: Explain what lambda binding is, and how cool it is.


# Building
Before you can build any of the projects in the `GoodbyeXAML` folder, you 
first need to run CodeGenerator.  Open `CodeGenerator/CodeGenerator.sln` in
Visual Studio and click "run".  This will generate a bunch of shared projects 
and dump them into the folder `GeneratedExtensionMethods`.

Now you can open `GoodbyeXAML/GoodbyeXAML.sln` in Visual Studio and build the
individual projects at your leisure.

At some point, I will write a script or something to automated this.