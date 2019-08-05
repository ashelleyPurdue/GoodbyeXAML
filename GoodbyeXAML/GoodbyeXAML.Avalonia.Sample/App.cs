using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;

public class App : Application
{
    public override void Initialize()
    {
        Styles.AddRange(new[]
        {
            IncludeStyle("avares://Avalonia.Themes.Default/DefaultTheme.xaml"),
            IncludeStyle("avares://Avalonia.Themes.Default/Accents/BaseLight.xaml")
        });
    }

    private StyleInclude IncludeStyle(string uri) 
    {
        var u = new Uri(uri);
        var style = new StyleInclude(u);
        style.Source = u;
        return style;
    }
}