﻿using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Config.Net;
using JetBrains.Annotations;
using Serilog;
using MidiPadMacros.Core;
using MidiPadMacros.Core.Interfaces;
using MidiPadMacros.Core.Logic.Application;
using MidiPadMacros.UI.Interfaces;
using MidiPadMacros.UI.TrayIcon;
using MidiPadMacros.UI.WindowResources.MainWindow;
using MidiPadMacros.UI.WindowResources.SettingsWindow;

namespace MidiPadMacros.Main;

/// <summary>
/// Builds the container for the local application dependencies. This is then passed to TeakTools.Common
/// dependency injection for library dependencies to get added
/// </summary>
[PublicAPI]
public class DiContainerBuilder
{
    private readonly ContainerBuilder _builder = new ();
    private ILogger? _logger;
    private ISettingsApplicationLocal? _settingsApplicationLocal;

    //private ISettingsApplicationLocal _settingsApplicationLocal;

    /// <summary>
    /// Gets a built container with all local application and TeakTools.Common dependencies in it
    /// </summary>
    /// <returns>A built container with all local application and TeakTools.Common dependencies in it</returns>
    [SupportedOSPlatform("Windows7.0")]
    public IContainer GetBuiltContainer(ILogger? testingLogger = null)
    {
        // Takes testing logger or if that's null builds the normal one and adds to DI container
        RegisterLogger(testingLogger);

        // This is not injected, kind of. It adds the dictionary to Application.Current.Resources.MergedDictionaries
        AddThemeResourceMergedDictionary();
        
        // All of these methods set up and initialize all necessary resources and dependencies,
        // then register the thing for Dependency Injection
        
        RegisterApplicationConfiguration();

        RegisterMainDependencies();
        
        RegisterUiDependencies();

        var container = _builder.Build();
        
        return container;
    }

    [SupportedOSPlatform("Windows7.0")]
    private void AddThemeResourceMergedDictionary()
    {
        var resourcePath = ApplicationPaths.DarkThemePath;
        var currentResource = new Uri(resourcePath, UriKind.RelativeOrAbsolute);
        
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = currentResource });
    }

    [SupportedOSPlatform("Windows7.0")]
    private void RegisterLogger(ILogger? testingLogger)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ApplicationPaths.LogPath) ?? "");

        if (testingLogger is not null)
        {
            _builder.RegisterInstance(testingLogger).As<ILogger>().SingleInstance();
            return;
        }

        // Otherwise, if it is null, make new logger:
        _logger = new LoggerConfiguration()
            .Enrich.WithProperty("MidiPadMacrosApplication", "MidiPadMacrosSerilogContext")
            .MinimumLevel.Debug()
            .WriteTo.File(ApplicationPaths.LogPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();
        
        _builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
    }
    private void RegisterApplicationConfiguration()
    {
        _settingsApplicationLocal = 
            new ConfigurationBuilder<ISettingsApplicationLocal>()
                .UseIniFile(ApplicationPaths.PathSettingsApplicationLocalIniFile)
                .Build();
        
        _builder.RegisterInstance(_settingsApplicationLocal).As<ISettingsApplicationLocal>().SingleInstance();
        
        SetupAnyBlankConfigurationPropertiesAsDefaults();
    }

    private void SetupAnyBlankConfigurationPropertiesAsDefaults()
    {
        if (_settingsApplicationLocal is null) throw new NullReferenceException($"{nameof(_settingsApplicationLocal)} was null");
        
        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialSelectionSettings.LastComPort)) _settingsApplicationLocal.SerialSelectionSettings.LastComPort = "COM1";
        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialSelectionSettings.LastBaud)) _settingsApplicationLocal.SerialSelectionSettings.LastBaud = "115200";
        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialSelectionSettings.LastParity)) _settingsApplicationLocal.SerialSelectionSettings.LastParity = "1";
        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialSelectionSettings.LastDataBit)) _settingsApplicationLocal.SerialSelectionSettings.LastDataBit = "8";
        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialSelectionSettings.LastStopBit)) _settingsApplicationLocal.SerialSelectionSettings.LastStopBit = "1";

        if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.SerialLogSettings.LastDirectory)) _settingsApplicationLocal.SerialLogSettings.LastDirectory = 
            Path.Join(
                ApplicationPaths.LogAppBasePath,
                "..",
                "Serial Data Logs");

        Directory.CreateDirectory(_settingsApplicationLocal.SerialLogSettings.LastDirectory);
    }

    [SupportedOSPlatform("Windows7.0")]
    private void RegisterMainDependencies()
    {
        _builder.RegisterType<ExceptionHandler>().AsSelf().SingleInstance();
    }
    
    [SupportedOSPlatform("Windows7.0")]
    private void RegisterUiDependencies()
    {
        _builder.RegisterInstance(Dispatcher.CurrentDispatcher).AsSelf().SingleInstance();
        
        _builder.RegisterType<SettingsViewModel>().AsSelf().SingleInstance();
        _builder.RegisterType<SettingsWindow>().AsSelf().SingleInstance();

        _builder.RegisterType<TrayIconViewModel>().As<ITrayIconViewModel>().SingleInstance();
        _builder.RegisterType<TrayIconMain>().As<ITrayIcon>().SingleInstance();
        
        _builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
        _builder.RegisterType<MainWindow>().AsSelf().SingleInstance();
    }
}