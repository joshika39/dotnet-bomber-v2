using System.Drawing;
using Bomber.Game.Game.Map;
using GameFramework.Board;
using GameFramework.Core.Motion;
using GameFramework.Core.Position;
using GameFramework.GameFeedback;
using GameFramework.Impl.GameFeedback;
using GameFramework.Manager;
using GameFramework.Objects;
using GameFramework.Objects.Interactable;
using GameFramework.Tiles;
using GameFramework.Visuals.Tiles;
using GameFramework.Visuals.Views;
using Infrastructure.Application;
using Infrastructure.Time.Listeners;

namespace Bomber.Game.Game.Tiles
{
    public sealed class Enemy : IInteractableObject2D, ITickListener, IViewLoadedSubscriber
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IMovingObjectView View { get; }

        private readonly CancellationToken _stoppingToken;
        private Move2D _direction;
        private bool _disposed;
        private readonly IBoardService _service;
        private readonly IGameManager _gameManager;

        public IPosition2D Position { get; private set; }
        public IScreenSpacePosition ScreenSpacePosition { get; }
        public bool IsObstacle => false;

        public Enemy(IPosition2D position, ILifeCycleManager lifeCycleManager)
        {
            if (Gameplay.Application2D is null)
            {
                throw new InvalidOperationException("ApplicationID must be set!");
            }
            View = Gameplay.Application2D.BoardService.TileViewFactory2D.CreateInteractableTileView2D(position, Color.DarkRed);
            _service = Gameplay.Application2D.BoardService;
            _gameManager = Gameplay.Application2D.Manager;
            Position = position ?? throw new ArgumentNullException(nameof(position));
            _stoppingToken = lifeCycleManager.Token;
            _direction = GetRandomMove();
            View.Attach(this);
            ScreenSpacePosition = View.ScreenSpacePosition;
        }

        public async Task ExecuteAsync()
        {
            while (!_stoppingToken.IsCancellationRequested && !_disposed)
            {
                var newPeriodInSeconds = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, 3);

                await _gameManager.Timer.WaitAsync(newPeriodInSeconds * 1000, this);
            }
        }

        public void SteppedOn(IInteractableObject2D interactable)
        {
            if (interactable is Player)
            {
                _gameManager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "You died!"), GameResolution.Loss);
            }
        }

        public void Step(IObject2D staticObject)
        {
            if (staticObject is IDeadlyTile || staticObject.IsObstacle)
            {
                _direction = GetRandomMove();
            }
            Position = staticObject.Position;
            View.UpdatePosition(Position);
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        private static Move2D GetRandomMove()
        {
            return System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 4) switch
            {
                0 => Move2D.Left,
                1 => Move2D.Right,
                2 => Move2D.Forward,
                3 => Move2D.Backward,
                _ => throw new InvalidOperationException("Unsupported move!")
            };
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
            var map = _service.GetActiveMap<GameMap>();

            if (map is null)
            {
                return;
            }

            var mapObject = map.SimulateMove(Position, _direction);
            while (mapObject is null || mapObject.IsObstacle || mapObject is IDeadlyTile || map.HasEnemy(mapObject.Position))
            {
                _direction = GetRandomMove();
                mapObject = map.SimulateMove(Position, _direction);
            }
            map.MoveInteractable(this, _direction);
        }

        public TimeSpan ElapsedTime { get; set; }

        public void OnLoaded(IMovingObjectView view)
        {
            View.UpdatePosition(Position);
        }
    }
}
