using Bomber.Forms.Map;
using Bomber.Game.Factories;
using Bomber.Game.Visuals.Views;
using GameFramework.Configuration;
using GameFramework.Core.Position;

namespace Bomber.Forms.Factories;

internal class FormsTileFactory : ITileFactory
{
    private readonly IConfigurationService2D _configurationService2D;

    public FormsTileFactory(IConfigurationService2D configurationService2D)
    {
        _configurationService2D = configurationService2D ?? throw new ArgumentNullException(nameof(configurationService2D));
    }
    
    public IBomberMapTileView CreateGround(IPosition2D position)
    {
        return new ExplosionIndicatorTile(position, (double)_configurationService2D.Dimension, Color.Green,
            Color.Coral);
    }
}