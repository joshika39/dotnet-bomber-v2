using GameFramework.Core.Position;
using GameFramework.Objects.Static;

namespace Bomber.Game.Factories
{
    public interface ITileFactory
    {
        IStaticObject2D CreateGround(IPosition2D position);
        IStaticObject2D CreateWall(IPosition2D position);
        IStaticObject2D CreateHole(IPosition2D position);
    }
}
