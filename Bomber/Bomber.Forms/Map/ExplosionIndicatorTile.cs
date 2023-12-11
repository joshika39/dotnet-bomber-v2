using Bomber.Game.Visuals.Views;
using GameFramework.Core.Position;
using GameFramework.UI.Forms.Tiles.Static;

namespace Bomber.Forms.Map
{
    public class ExplosionIndicatorTile : AStaticTileView, IBomberMapTileView
    {
        private readonly Color _fillColor;
        private readonly Color _indicatorColor;

        public ExplosionIndicatorTile(IPosition2D position2D, double size, Color fillColor, Color indicatorColor, bool hasBorder = false) : base(position2D, size, fillColor, hasBorder)
        {
            _fillColor = fillColor;
            _indicatorColor = indicatorColor;
        }

        public void IndicateBomb(double waitTime)
        {
            BackColor = _indicatorColor;
            Invalidate();
            Task.Delay(TimeSpan.FromSeconds(waitTime)).ContinueWith(_ => BackColor = _fillColor);
        }
    }
}
