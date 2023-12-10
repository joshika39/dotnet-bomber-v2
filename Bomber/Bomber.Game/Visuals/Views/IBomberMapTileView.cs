using GameFramework.Visuals.Views;

namespace Bomber.Game.Visuals.Views
{
    public interface IBomberMapTileView : IStaticObjectView2D
    {
        void IndicateBomb(double waitTime);
    }
}
