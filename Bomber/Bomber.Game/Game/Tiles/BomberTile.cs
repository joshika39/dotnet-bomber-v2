using System.Drawing;
using Bomber.Game.Visuals.Views;
using GameFramework.Board;
using GameFramework.Core.Position;
using GameFramework.Impl.Tiles.Static;

namespace Bomber.Game.Game.Tiles;

public class BomberTile : StaticTile, IBomberMapTile
{
    private readonly Color _fillColor;
    private readonly Color _indicatorColor;

    public BomberTile(IPosition2D position, IBoardService boardService, bool isObstacle = false,
        bool hasBorder = false) : base(position, boardService, Color.Green, isObstacle, hasBorder)
    {
        _fillColor = Color.Green;
        _indicatorColor = Color.Yellow;
    }

    public void IndicateBomb(double waitTime)
    {
        View.FillColor = _indicatorColor;
        Task.Delay(TimeSpan.FromSeconds(waitTime)).ContinueWith(_ => View.FillColor = _fillColor);
    }
}