using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using Bomber.Game.Visuals.Views;
using GameFramework.Board;
using GameFramework.Core.Position;
using GameFramework.GameFeedback;
using GameFramework.Impl.GameFeedback;
using GameFramework.Impl.Tiles.Interactable;
using GameFramework.Manager;
using GameFramework.Objects;
using GameFramework.Objects.Interactable;
using GameFramework.Tiles;
using Infrastructure.Application;

namespace Bomber.Game.Game.Tiles
{
    public sealed class Player : InteractableTile, IBombWatcher
    {
        private readonly IGameManager _gameManager;
        private readonly ILifeCycleManager _lifeCycleManager;
        private readonly IBoardService _boardService;
        private bool _isAlive = true;
        public string Name { get; }
        public string Email { get; }
        public ICollection<Bomb> PlantedBombs { get; }
        public int Score { get; set; }

        public Player(IPosition2D position, string name, string email, ILifeCycleManager lifeCycleManager,
            IBoardService boardService) : base(position, boardService, Color.Purple)
        {
            if (Gameplay.Application2D is null)
            {
                throw new InvalidOperationException("ApplicationID must be set!");
            }

            _gameManager = Gameplay.Application2D.Manager;
            _lifeCycleManager = lifeCycleManager ?? throw new ArgumentNullException(nameof(lifeCycleManager));
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PlantedBombs = new ObservableCollection<Bomb>();
        }

        public override void SteppedOn(IInteractableObject2D interactableObject2D)
        {
            base.SteppedOn(interactableObject2D);

            if (interactableObject2D is Enemy)
            {
                Kill();
            }
        }

        public override void Step(IObject2D staticObject)
        {
            base.Step(staticObject);

            if (_gameManager.State != GameState.InProgress || !_isAlive)
            {
                return;
            }

            if (staticObject is IDeadlyTile)
            {
                Kill();
            }
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
            bomb.Detonate();
            PlantedBombs.Remove(bomb);
        }

        public Bomb PutBomb()
        {
            var bomb = new Bomb(Position, 3, _lifeCycleManager, _boardService);
            bomb.Attach(this);
            PlantedBombs.Add(bomb);
            return bomb;
        }


        public override void Dispose()
        {
            base.Dispose();

            while (PlantedBombs.Count > 0)
            {
                var bomb = PlantedBombs.ElementAt(PlantedBombs.Count - 1);
                PlantedBombs.Remove(bomb);
                bomb.Dispose();
            }
        }

        private void Kill()
        {
            _isAlive = false;
            _gameManager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "You lost because you're DEAD!"),
                GameResolution.Loss);
            Dispose();
        }

        public void BombExploded(Bomb bomb)
        {
            Debug.WriteLine($"Bomb exploded at {bomb.Position}");
        }
    }
}