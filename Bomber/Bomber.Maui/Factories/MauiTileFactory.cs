using System;
using Bomber.Game.Factories;
using Bomber.Game.Visuals.Views;
using Bomber.Maui.Map;
using GameFramework.Configuration;
using GameFramework.Core.Position;
using Color = System.Drawing.Color;
namespace Bomber.Maui.Factories;

internal class MauiTileFactory : ITileFactory
{
    private readonly IConfigurationService2D _configurationService2D;

    public MauiTileFactory(IConfigurationService2D configurationService2D)
    {
        _configurationService2D = configurationService2D ?? throw new ArgumentNullException(nameof(configurationService2D));
    }
    
    public IBomberMapTileView CreateGround(IPosition2D position)
    {
        return new ExplosionIndicatorTileView(position, _configurationService2D, Color.Green,
            Color.Coral);
    }
}