using System.Drawing;
using Bomber.Game.Game.Map;
using GameFramework.Board;
using GameFramework.Core.Motion;
using GameFramework.Core.Position;
using GameFramework.GameFeedback;
using GameFramework.Impl.GameFeedback;
using GameFramework.Impl.Tiles.Interactable;
using GameFramework.Manager;
using GameFramework.Objects;
using GameFramework.Objects.Interactable;
using GameFramework.Tiles;
using GameFramework.Visuals.Tiles;
using Infrastructure.Application;
using Infrastructure.Time.Listeners;

namespace Bomber.Game.Game.Tiles
{
    public sealed class Enemy : InteractableTile, ITickListener, IViewLoadedSubscriber
    {
        private readonly CancellationToken _stoppingToken;
        private Move2D _direction;
        private bool _disposed;
        private readonly IBoardService _service;
        private readonly IGameManager _gameManager;
        
        public Enemy(IPosition2D position, ILifeCycleManager lifeCycleManager) : base(position, Gameplay.Application2D!.BoardService, Color.Red)
        {
            if (Gameplay.Application2D is null)
            {
                throw new InvalidOperationException("ApplicationID must be set!");
            }
            _service = Gameplay.Application2D.BoardService;
            _gameManager = Gameplay.Application2D.Manager;
            Position = position ?? throw new ArgumentNullException(nameof(position));
            _stoppingToken = lifeCycleManager.Token;
            _direction = GetRandomMove();
        }

        public async Task ExecuteAsync()
        {
            while (!_stoppingToken.IsCancellationRequested && !_disposed)
            {
                var newPeriodInSeconds = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, 3);

                await _gameManager.Timer.WaitAsync(newPeriodInSeconds * 1000, this);
            }
        }

        public override void SteppedOn(IInteractableObject2D interactable)
        {
            base.SteppedOn(interactable);
            
            if (interactable is Player)
            {
                _gameManager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "You died!"), GameResolution.Loss);
            }
        }

        public override void Step(IObject2D staticObject)
        {
            if (staticObject is IDeadlyTile || staticObject.IsObstacle)
            {
                _direction = GetRandomMove();
            }
            
            Position = staticObject.Position;
            View.Position2D = Position;
        }

        public void Delete()
        {
            Dispose();
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
    }
}
