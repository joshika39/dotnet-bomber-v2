using GameFramework.Configuration;
using GameFramework.Core.Position;
using GameFramework.Impl.Tiles.Static;
using GameFramework.Objects.Static;
using GameFramework.Tiles.Factories;
using GameFramework.Visuals.Factories;
using System.Drawing;
using Bomber.Game.Factories;

namespace Bomber.Game.Game.Tiles
{
    internal class StaticTileConverter : IStaticObject2DConverter
    {
        private readonly ITileFactory2D _tileFactory2D;
        private readonly ITileViewFactory2D _tileViewFactory2D;
        private readonly IConfigurationService2D _configurationService2D;
        private readonly ITileFactory _tileFactory;

        public StaticTileConverter(ITileFactory2D tileFactory2D, ITileViewFactory2D tileViewFactory2D,
            IConfigurationService2D configurationService2D, ITileFactory tileFactory)
        {
            _tileFactory2D = tileFactory2D ?? throw new ArgumentNullException(nameof(tileFactory2D));
            _tileViewFactory2D = tileViewFactory2D ?? throw new ArgumentNullException(nameof(tileViewFactory2D));
            _configurationService2D =
                configurationService2D ?? throw new ArgumentNullException(nameof(configurationService2D));
            _tileFactory = tileFactory ?? throw new ArgumentNullException(nameof(tileFactory));
        }

        public IStaticObject2D FromEnum<T>(T tileType, IPosition2D position) where T : Enum
        {
            if (tileType is not TileTypes)
            {
                throw new ArgumentException($"tileType must be of type {nameof(TileTypes)}");
            }

            return tileType switch
            {
                TileTypes.GroundTile => new GeneralStaticTile(position, _configurationService2D, 
                    _tileFactory.CreateGround(position)),
                TileTypes.WallTile => new GeneralStaticTile(position, _configurationService2D,
                    _tileViewFactory2D.CreateTileView2D(position, Color.Gray), true),
                TileTypes.HoleTile => _tileFactory2D.CreateStaticTile2D(position, Color.Black),
                _ => throw new ArgumentException($"Unknown tile type: {tileType}")
            };
        }
    }
}