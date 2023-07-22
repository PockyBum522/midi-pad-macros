using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MidiPadMacros.UI.WindowResources.UserControls;

[ObservableObject]
public partial class MidiPadCircularButton : UserControl
{
    /// <summary>
    /// Displays the macro text. Example "Ctrl + Shift + T"
    /// </summary>
    public static readonly DependencyProperty MacroTextProperty = DependencyProperty.Register(
        nameof(MacroText), typeof(string), typeof(MidiPadCircularButton), new PropertyMetadata(default(string)));

    /// <summary>
    /// Displays the macro text. Example "Ctrl + Shift + T"
    /// </summary>
    public string MacroText
    {
        get { return (string)GetValue(MacroTextProperty); }
        set { SetValue(MacroTextProperty, value); }
    }

    /// <summary>
    /// Displays the MIDI id for this button
    /// </summary>
    public static readonly DependencyProperty ButtonIdTextProperty = DependencyProperty.Register(
        nameof(ButtonIdText), typeof(string), typeof(MidiPadCircularButton), new PropertyMetadata(default(string)));

    /// <summary>
    /// Displays the MIDI id for the button
    /// </summary>
    public string ButtonIdText
    {
        get { return (string)GetValue(ButtonIdTextProperty); }
        set { SetValue(ButtonIdTextProperty, value); }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public MidiPadCircularButton()
    {
        InitializeComponent();
    }
}