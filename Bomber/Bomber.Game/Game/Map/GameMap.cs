using Bomber.Game.Game.Tiles;
using Bomber.Game.Visuals.Views;
using GameFramework.Configuration;
using GameFramework.Core.Factories;
using GameFramework.Core.Position;
using GameFramework.Impl.Map;
using Infrastructure.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Bomber.Game.Game.Map
{
    public class GameMap : AMap2D<GameMapSource, IGameMapView>
    {
        private readonly ILifeCycleManager _lifeCycleManager;

        public GameMap(GameMapSource mapSource, IGameMapView view, IPositionFactory positionFactory,
            IConfigurationService2D configurationService2D) : base(mapSource, view, positionFactory,
            configurationService2D)
        {
            _lifeCycleManager = Gameplay.Application2D?.Services.GetRequiredService<ILifeCycleManager>() ?? throw new InvalidOperationException("Application2D must be set!");
            FillEntities(mapSource);
        }

        public bool HasEnemy(IPosition2D position)
        {
            foreach (var entity in Interactables)
            {
                if (entity is not Enemy npc)
                {
                    continue;
                }

                if (npc.Position == position)
                {
                    return true;
                }
            }
            return false;
        }

        private void FillEntities(GameMapSource mapSource)
        {
            foreach (var entity in mapSource.Enemies)
            {
                var pos = PositionFactory.CreatePosition(entity.X, entity.Y);
                Interactables.Add(new Enemy(pos, _lifeCycleManager));
            }
        }

    }
}
