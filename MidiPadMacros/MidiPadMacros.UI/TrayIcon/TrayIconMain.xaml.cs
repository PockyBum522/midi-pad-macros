using CommunityToolkit.Mvvm.ComponentModel;
using MidiPadMacros.UI.Interfaces;

namespace MidiPadMacros.UI.TrayIcon;

/// <summary>
/// TrayIconMain class for setting up  
/// </summary>
[ObservableObject]
public partial class TrayIconMain : ITrayIcon
{
    /// <summary>
    /// Constructor that sets up ViewModel, icon resource
    /// </summary>
    /// <param name="trayIconViewModel">Injected TrayIconViewModel</param>
    public TrayIconMain(ITrayIconViewModel trayIconViewModel)
    {
        Resources.Add("MainTrayIcon", new System.Drawing.Icon("Icons/TrayIcon.ico"));

        DataContext = trayIconViewModel;
        
        InitializeComponent();
    }
}