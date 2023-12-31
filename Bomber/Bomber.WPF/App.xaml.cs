﻿using Bomber.Game;
using Bomber.WPF.Factories;
using Bomber.WPF.ViewModels;
using Bomber.WPF.Views;
using GameFramework.Impl.Core;
using GameFramework.UI.WPF.Core;
using GameFramework.Visuals.Factories;
using Implementation.Module;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Windows;

namespace Bomber.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override IServiceProvider LoadModules(ServiceCollection collection)
        {
            // NOTE: Add your own modules here
            var source = new CancellationTokenSource();
            var core = new CoreModule(collection, source);

            // NOTE: Change the namespace to your own namespace
            core.RegisterServices("joshika39.MyGame");
            core.RegisterOtherServices(new GameFrameworkCore(collection, source));
            core.RegisterOtherServices(new WpfGameFramework(collection, source));
            core.RegisterOtherServices(new GameModule(collection));

            // NOTE: Add your own services here
            return collection
                .AddScoped<IMainWindow, MainWindow>()
                .AddScoped<IMapViewFactory2D, WpfGameMapViewFactory>()
                .AddScoped<IMainWindowViewModel, MainWindowViewModel>()
                .BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                Gameplay.Application2D = Current;

                var mainWindow = provider.GetRequiredService<IMainWindow>();
                var mainWindowViewModel = provider.GetRequiredService<IMainWindowViewModel>();

                if (mainWindow is MainWindow window)
                {
                    window.DataContext = mainWindowViewModel;
                    window.Show();
                }
            }
        }
    }
}
