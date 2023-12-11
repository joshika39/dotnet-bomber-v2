using Bomber.Game.Visuals.Views;
using GameFramework.Core.Position;

namespace Bomber.Game.Factories
{
    public interface ITileFactory
    {
        IBomberMapTileView CreateGround(IPosition2D position);
    }
}
