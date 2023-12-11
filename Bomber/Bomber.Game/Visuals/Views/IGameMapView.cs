using GameFramework.Visuals.Views;

namespace Bomber.Game.Visuals.Views
{
    public interface IGameMapView : IMapView2D
    {
        void PlantBomb(IMovingObjectView bombView);
        void DeleteBomb(IMovingObjectView bombView);
    }
}
