using Bomber.Game.Game.Tiles;

namespace Bomber.Game.Visuals.Views
{
    public interface IBombWatcher
    {
        void BombExploded(Bomb bomb);
    }
}
