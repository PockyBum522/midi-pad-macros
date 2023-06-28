using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Serilog;
using MidiPadMacros.Core;
using MidiPadMacros.Core.Logic.WindowsHelpers;
using MidiPadMacros.UI.Interfaces;
using MidiPadMacros.UI.WindowResources.MainWindow;
using MidiPadMacros.UI.WindowResources.SettingsWindow;
using WindowsInput;
using WindowsInput.Native;

namespace MidiPadMacros.UI.TrayIcon;

/// <summary>
/// ViewModel for Tray Icon
/// </summary>
public partial class TrayIconViewModel : ObservableObject, ITrayIconViewModel
{
    private readonly ILogger _logger;

    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly MainWindow _mainWindow;
    
    //private readonly ISettingsApplicationLocal _settingsAppLocal;
    
    private readonly SettingsWindow _settingsWindow;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected logger to use</param>
    /// <param name="mainWindow">Main window view for the application</param>
    /// <param name="settingsViewModel">Injected settings window viewmodel to use</param>
    /// <param name="mainWindowViewModel">Main window ViewModel for the application</param>
    // /// <param name="settingsAppLocal">Injected application local settings to use</param>
    public TrayIconViewModel(ILogger logger,
        MainWindowViewModel mainWindowViewModel,
        MainWindow mainWindow,
        SettingsViewModel settingsViewModel) //, ISettingsApplicationLocal settingsAppLocal)
    {
        _logger = logger;
        
        _mainWindow = mainWindow;
        _mainWindowViewModel = mainWindowViewModel;
        _mainWindow.DataContext = _mainWindowViewModel;
        _mainWindow.Closing += _mainWindowViewModel.OnWindowClosing;

        _logger.Information("Initializing Tray Icon View");
        
        //_settingsAppLocal = settingsAppLocal;

        _settingsWindow = new()
        {
            DataContext = settingsViewModel
        };

        _settingsWindow.Hide();
        
        _logger.Information("Hid settings window, tray icon init finished");

        LogAllMidiDevices();

        SetupMidiFighterEvents();
    }
    
    private static IInputDevice _inputDevice;

    private void LogAllMidiDevices()
    {
        var allDevices = InputDevice.GetAll();

        foreach (var midiDevice in allDevices)
        {
            _logger.Information("Found device: {DeviceName}", midiDevice.Name);
        }
    }

    private async Task SetupMidiFighterEvents()
    {
        _inputDevice = InputDevice.GetByName("Midi Fighter Spectra");
        _inputDevice.EventReceived += OnEventReceived;
        _inputDevice.StartEventsListening();
        
        Console.WriteLine("Input device is listening for events. Press any key to exit...");

        while (true)
        {
            await Task.Delay(1000);
        }
        
        (_inputDevice as IDisposable)?.Dispose();
    }

    
    public static string DeviceName { get; set; }

    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        
        _logger.Information("Event received from '{DeviceName}' at {TimeStamp}: {EventId}", midiDevice.Name, DateTime.Now, e.Event);

        var midiEvent = e.Event;
        
        if (midiEvent.EventType != MidiEventType.NoteOn) return;

        var noteOnEvent = (NoteOnEvent)midiEvent;
        
        // Otherwise
        switch (noteOnEvent.NoteNumber)
        {
            // Second row from bottom, far left
            case 40:
                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LCONTROL);

                new InputSimulator().Mouse.LeftButtonClick();
                
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
                
                break;
            
            // Absolute bottom left button
            case 36:
                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LMENU);
                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LSHIFT);
                
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VK_E);
                
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LMENU);
                
                break;
            
            // Last row, second from left
            case 37:

                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LMENU);
                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LSHIFT);
                
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VK_H);
                
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LMENU);
                
                break;
            
            // Last row, third from left
            case 38:

                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LMENU);
                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LSHIFT);
                
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VK_H);
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VK_E);
                
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LMENU);
                
                break;
            
            // Last row, last in row
            case 39:

                new InputSimulator().Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
                
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VK_Z);
                
                new InputSimulator().Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
                
                break;
        }
    }
    
    /// <summary>
    /// Command to show the settings window
    /// </summary>
    [RelayCommand]
    private void OpenMainWindow() => _mainWindow.Show();
    
    /// <summary>
    /// Command to exit the application completely
    /// </summary>
    [RelayCommand]
    private void ExitApplication() => Environment.Exit(0);
    
    /// <summary>
    /// Command to show the settings window
    /// </summary>
    [RelayCommand]
    private void OpenSettingsWindow() => _settingsWindow.Show();
    
    // Example of some commonly used things that might be needed for a menu entry
    // private void EnableDevice()
    // {
    //     var deviceCategoryGuid = new Guid(_settingsAppLocal.DeviceClassGuid);
    //
    //     var instancePath = _settingsAppLocal.DeviceInstancePath;
    //
    //     DeviceHelper.SetDeviceEnabled(deviceCategoryGuid, instancePath, true);
    //     
    //     _logger.Information("Enabled device at path: {DevicePath}",
    //         _settingsAppLocal.DeviceInstancePath);
    // }
    
    /// <summary>
    /// Command to open the log file in VSCode
    /// </summary>
    [RelayCommand]
    private void OpenLogfileInVscode()
    {
        // This is just to parent the messageboxes
        var temporaryWindow = new Window()
        {
            Visibility = Visibility.Hidden,
            // Just hiding the window is not sufficient, as it still temporarily pops up the first time.
            // Therefore, make it transparent.
            AllowsTransparency = true,
            Background = System.Windows.Media.Brushes.Transparent,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false
        };

        temporaryWindow.Show();
        
        var vscodeProcess = new Process();

        var newestFileName = FolderHelpers.GetNewestFileNameIn(ApplicationPaths.LogAppBasePath);
        
        const string vsCodePath = @"C:\Program Files\Microsoft VS Code\Code.exe";

        _logger.Information("Looking for VSCode at path: {VscodePath}", vsCodePath);
        
        if (!File.Exists(vsCodePath))
        {
            MessageBox.Show(temporaryWindow, 
                $"Please install Visual Studio Code to {vsCodePath} before trying to open log");
            
            return;
        }

        vscodeProcess.StartInfo.FileName = vsCodePath;
        vscodeProcess.StartInfo.Arguments = "\"" + Path.Combine(ApplicationPaths.LogAppBasePath, newestFileName) + "\"";
        
        vscodeProcess.Start();
    }
}