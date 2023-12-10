using Bomber.Game.Visuals.Views;

namespace Bomber.WPF.ViewModels
{
    public interface IMainWindowViewModel
    {
        IGameMapView MapView { get; }
    }
}
