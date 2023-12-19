using Bomber.Game.Game.Map;
using Bomber.Game.Visuals.Views;
using GameFramework.Core.Position;
using GameFramework.Manager;
using GameFramework.Objects;
using GameFramework.Objects.Static;
using GameFramework.Visuals.Tiles;
using Infrastructure.Application;
using Infrastructure.Time.Listeners;
using System.Drawing;
using GameFramework.Board;
using GameFramework.Impl.Tiles.Interactable;

namespace Bomber.Game.Game.Tiles
{
    public sealed class Bomb : InteractableTile, ITickListener, IViewLoadedSubscriber
    {
        private readonly ICollection<IBombWatcher> _bombWatchers = new List<IBombWatcher>();
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IStaticObject2D> _affectedObjects = new List<IStaticObject2D>();
        private bool _disposed;
        private readonly CancellationToken _stoppingToken;

        public IScreenSpacePosition ScreenSpacePosition { get; }
        public int Radius { get; }
        public int RemainingTime { get; private set; }
        private bool _isDetonated;


        public Bomb(IPosition2D position, int radius, ILifeCycleManager lifeCycleManager, IBoardService boardService, int timeToExplosion = 2000) : base(position, boardService, Color.Black)
        {
            RemainingTime = timeToExplosion;
            if (Gameplay.Application2D is null)
            {
                throw new InvalidOperationException("ApplicationID must be set!");
            }
            _gameManager = Gameplay.Application2D.Manager;
            var boardService1 = Gameplay.Application2D.BoardService;
            lifeCycleManager = lifeCycleManager ?? throw new ArgumentNullException(nameof(lifeCycleManager));
            _stoppingToken = lifeCycleManager.Token;
            if (radius <= 0)
            {
                throw new InvalidOperationException("Radius cannot be zero or negative");
            }

            ScreenSpacePosition = View.ScreenSpacePosition;

            Radius = radius;

            if (_gameManager.State != GameState.InProgress)
            {
                Dispose();
            }

            var map = boardService1.GetActiveMap<GameMap>();
            if (map is not null)
            {
                _affectedObjects = map.MapPortion(position, radius);
            }
        }

        public async Task Detonate()
        {
            if (_disposed || _isDetonated)
            {
                return;
            }

            _isDetonated = true;
            while (!_stoppingToken.IsCancellationRequested)
            {
                RemainingTime -= 300;

                if (RemainingTime <= 0)
                {
                    Explode();
                    break;
                }

                await _gameManager.Timer.WaitAsync(RemainingTime, this);
            }

        }
        public void Attach(IBombWatcher bombWatcher)
        {
            if (!_bombWatchers.Contains(bombWatcher))
            {
                _bombWatchers.Add(bombWatcher);
            }
        }

        private void Explode()
        {
            foreach (var bombWatcher in _bombWatchers)
            {
                bombWatcher.BombExploded(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                View.Dispose();
            }

            _disposed = true;
        }


        public void Dispose()
        {
            Dispose(true);
        }

        public void RaiseTick(int round)
        {
            foreach (var affectedObject in _affectedObjects)
            {
                if (affectedObject.View is IBomberMapTile bombMapObject)
                {
                    bombMapObject.IndicateBomb(RemainingTime / 1000d);
                }
            }
        }

        public TimeSpan ElapsedTime { get; set; }
    }
}
