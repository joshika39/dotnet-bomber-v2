using Bomber.Game.Game.Map;
using Bomber.Game.Visuals.Views;
using GameFramework.UI.Maui.Map;
using GameFramework.Visuals.Views;

namespace Bomber.Maui.Map
{
    internal class GameMapView : MauiMapControl, IGameMapView
    {
        public void PlantBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }

        public void DeleteBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }
    }
}