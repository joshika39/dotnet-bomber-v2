using Bomber.Game.Game.Map;
using Bomber.Game.Visuals.Views;
using GameFramework.UI.Forms.Map;
using GameFramework.Visuals.Views;

namespace Bomber.Forms.Map
{
    internal class GameMapView : FormsMapControl, IGameMapView
    {
        public void PlantBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }

        public void DeleteBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
