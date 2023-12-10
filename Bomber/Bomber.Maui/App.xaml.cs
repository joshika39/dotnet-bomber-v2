using Bomber.Game;
using Bomber.Maui.Factories;
using GameFramework.Impl.Core;
using GameFramework.UI.Maui.Core;
using GameFramework.Visuals.Factories;
using Implementation.Module;

namespace Bomber.Maui
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Gameplay.Application2D = Current;
        }

        protected override IServiceProvider LoadModules(ServiceCollection collection)
        {
            // NOTE: Add your own modules here
            var source = new CancellationTokenSource();
            var core = new CoreModule(collection, source);

            // NOTE: Change the namespace to your own namespace
            core.RegisterServices("joshika39.MyGame");
            core.RegisterOtherServices(new GameFrameworkCore(collection, source));
            core.RegisterOtherServices(new MauiGameFramework(collection, source));
            core.RegisterOtherServices(new GameModule(collection));

            // NOTE: Add your own services here
            return collection
                .AddScoped<IMapViewFactory2D, MauiGameMapViewFactory>()
                .BuildServiceProvider();
        }
    }
}
