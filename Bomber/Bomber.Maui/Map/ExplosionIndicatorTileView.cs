using Bomber.Game.Visuals.Views;
using GameFramework.Configuration;
using GameFramework.Core.Position;
using GameFramework.UI.Maui.Tiles.Static;
using Color = System.Drawing.Color;

namespace Bomber.Maui.Map;

internal class ExplosionIndicatorTileView : AStaticTileView, IBomberMapTileView
{
    private readonly Color _indicatorColor;

    public ExplosionIndicatorTileView(IPosition2D position, IConfigurationService2D configurationService,
        Color fillColor, Color indicatorColor) : base(position, configurationService, fillColor,
        false)
    {
        _indicatorColor = indicatorColor;
    }

    public void IndicateBomb(double waitTime)
    {
        Dispatcher.Dispatch(async () =>
        {
            Background = new SolidColorBrush(ConvertColor(_indicatorColor));
            await Task.Delay(TimeSpan.FromSeconds(waitTime));
            Background = new SolidColorBrush(ConvertColor(FillColor));
        });

    }
}