using Bomber.Game.Visuals.Views;
using GameFramework.UI.WPF.Map;
using GameFramework.Visuals.Views;

namespace Bomber.WPF.Map
{
    internal class GameMapView : WpfMapControl, IGameMapView
    {
        public void PlantBomb(IMovingObjectView bombView)
        {

        }

        public void DeleteBomb(IMovingObjectView bombView)
        {

        }

        public void Clear()
        {
            Children.Clear();
        }
    }
}
