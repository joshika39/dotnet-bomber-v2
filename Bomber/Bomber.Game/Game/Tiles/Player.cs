using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using Bomber.Game.Visuals.Views;
using GameFramework.Board;
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

namespace Bomber.Game.Game.Tiles
{
    public sealed class Player : IInteractableObject2D, IViewLoadedSubscriber, IBombWatcher
    {
        private readonly IGameManager _gameManager;
        private readonly ILifeCycleManager _lifeCycleManager;
        private readonly IBoardService _boardService;
        private bool _isAlive = true;
        private bool _disposed;

        public IPosition2D Position { get; private set; }
        public IScreenSpacePosition ScreenSpacePosition { get; }
        public bool IsObstacle => false;
        public Guid Id { get; }
        public IMovingObjectView View { get; }
        public string Name { get; }
        public string Email { get; }
        public ICollection<Bomb> PlantedBombs { get; }
        public int Score { get; set; }

        public Player(IPosition2D position, string name, string email, ILifeCycleManager lifeCycleManager)
        {
            if (Gameplay.Application2D is null)
            {
                throw new InvalidOperationException("ApplicationID must be set!");
            }
            View = Gameplay.Application2D.BoardService.TileViewFactory2D.CreateInteractableTileView2D(position, Color.Aqua);
            _gameManager = Gameplay.Application2D.Manager;
            _lifeCycleManager = lifeCycleManager ?? throw new ArgumentNullException(nameof(lifeCycleManager));
            _boardService = Gameplay.Application2D.BoardService;
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Id = Guid.NewGuid();
            PlantedBombs = new ObservableCollection<Bomb>();
            View.Attach(this);
            ScreenSpacePosition = View.ScreenSpacePosition;
        }

        public void SteppedOn(IInteractableObject2D interactableObject2D)
        {
            if (interactableObject2D is Enemy)
            {
                Kill();
            }
        }
        public void Step(IObject2D staticObject)
        {
            if (_gameManager.State != GameState.InProgress || !_isAlive)
            {
                return;
            }

            if (staticObject is IDeadlyTile)
            {
                Kill();
            }

            Position = staticObject.Position;
            View.UpdatePosition(Position);
        }

        public void DetonateBombAt(int bombIndex)
        {
            if (!PlantedBombs.Any())
            {
                return;
            }

            if (bombIndex < 0 || bombIndex >= PlantedBombs.Count)
            {
                return;
            }

            var bomb = PlantedBombs.ElementAt(bombIndex);
            Task.Run(async () => await bomb.Detonate());
            PlantedBombs.Remove(bomb);
        }

        public Bomb PutBomb()
        {
            var bomb = new Bomb(Position, 3,  _lifeCycleManager);
            bomb.Attach(this);
            PlantedBombs.Add(bomb);
            return bomb;
        }

        public void Delete()
        {
            Dispose();
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
                while (PlantedBombs.Count > 0)
                {
                    var bomb = PlantedBombs.ElementAt(PlantedBombs.Count - 1);
                    PlantedBombs.Remove(bomb);
                    bomb.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Kill()
        {
            _isAlive = false;
            _gameManager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "You lost because you're DEAD!"), GameResolution.Loss);
            Dispose();
        }

        public void BombExploded(Bomb bomb)
        {
            Debug.WriteLine($"Bomb exploded at {bomb.Position}");
        }

        public void OnLoaded(IMovingObjectView view)
        {
            View.UpdatePosition(Position);
        }
    }
}
