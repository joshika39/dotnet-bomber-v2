using Bomber.Game;
using Bomber.Game.Game.Map;
using Bomber.Game.Visuals.Views;
using Bomber.Maui.Map;
using GameFramework.UI.Maui.Core;

namespace Bomber.Maui.ViewModels
{
    public class MainPageViewModel
    {
        public IGameMapView MapControl { get; set; }

        public MainPageViewModel()
        {
            var mapControl = new GameMapView();
            var gameplay = new Gameplay();
            MapControl = mapControl;
        }
    }
}
