using Bomber.Game.Visuals.Views;
using GameFramework.Core.Position;

namespace Bomber.Game.Factories
{
    public interface ITileFactory
    {
        IBomberMapTile CreateGround(IPosition2D position);
    }
}
