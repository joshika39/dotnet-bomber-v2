using GameFramework.Application;
using GameFramework.Core.Factories;
using GameFramework.Core.Motion;
using GameFramework.GameFeedback;
using GameFramework.Impl.GameFeedback;
using GameFramework.Manager;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Drawing;
using Bomber.Game.Game.Map;
using Bomber.Game.Game.Tiles;
using Bomber.Game.Visuals.Views;
using GameFramework.Manager.State;
using Infrastructure.Application;
using Player = Bomber.Game.Game.Tiles.Player;

namespace Bomber.Game
{
    public class Gameplay : IGameStateChangedListener, IBombWatcher
    {
        private static IApplication2D? _application2D;
        public static IApplication2D? Application2D
        {
            get => _application2D;
            set => _application2D ??= value;
        }

        private bool _hadEnemies;

        public void OpenMap(string mapFileName, IGameMapView mapView2D)
        {
            if (Application2D is null)
            {
                return;
            }

            var positionFactory = Application2D.Services.GetRequiredService<IPositionFactory>();
            var lifeCycleManager = Application2D.Services.GetRequiredService<ILifeCycleManager>();

            mapView2D.Clear();

            Application2D.Manager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "Game ended"), GameResolution.Nothing);

            var mapSource = new GameMapSource(Application2D.Services, mapFileName);
            var map = new GameMap(mapSource, mapView2D, positionFactory, Application2D.ConfigurationService);

            _hadEnemies = mapSource.Enemies.Any();

            if (Application2D.Manager.State == GameState.InProgress)
            {
                Application2D.Manager.ResetGame();
            }
            var view = Application2D.BoardService.TileViewFactory2D.CreateInteractableTileView2D(positionFactory.CreatePosition(0, 0), Color.Aqua);
            var player = new Player(mapSource.PlayerPosition, "Bomber", "Test@test.com", lifeCycleManager);
            view.ViewLoaded();
            map.Interactables.Add(player);
            Application2D.Manager.StartGame(new GameplayFeedback(FeedbackLevel.Info, "Game started!"));
            Application2D.BoardService.SetActiveMap(map);
            foreach (var unit in map.Interactables)
            {
                unit.View.ViewLoaded();
                if (unit is Enemy enemy)
                {
                    Task.Run(async () =>
                    {
                        await enemy.ExecuteAsync();
                    });
                }
            }

            foreach (var bombDummy in map.MapSource.Bombs)
            {
                var bomb = new Bomb(positionFactory.CreatePosition(bombDummy.X, bombDummy.Y), 3, lifeCycleManager, bombDummy.RemainingTime);
                player.PlantedBombs.Add(bomb);
                bomb.Attach(this);
                bomb.Attach(player);
                bomb.View.ViewLoaded();
                map.Interactables.Add(bomb);
            }

            // NOTE: This is how you start the game.
            Application2D.BoardService.SetActiveMap(map);
            Application2D.Manager.StartGame(new GameplayFeedback(FeedbackLevel.Info, "Game test started"));
        }

        public void BombExploded(Bomb bomb)
        {
            if (Application2D is null)
            {
                return;
            }

            var map = Application2D.BoardService.GetActiveMap<GameMap>();
            var unit = map?.Interactables.FirstOrDefault(b => b is Player);
            if (map is null || unit is not Player bomber)
            {
                return;
            }

            var affectedObjects = map.MapPortion(bomb.Position, bomb.Radius);
            var entities = map.GetInteractablesAtPortion(affectedObjects);
            
            foreach (var entity in entities)
            {
                if (entity != bomb)
                {
                    entity.Delete();
                    map.Interactables.Remove(entity);
                }

                switch (entity)
                {
                    case Enemy:
                        bomber.Score += 1;
                        break;
                    case Player:
                        Application2D.Manager.EndGame(new GameplayFeedback(FeedbackLevel.Info, "You lost! You got exploded!"), GameResolution.Loss);
                        break;
                }
            }
            
            bomb.Delete();
            map.Interactables.Remove(bomb);

            
            if (!_hadEnemies || map.Interactables.Any(entity => entity is Enemy))
            {
                return;
            }
            
            Application2D.Manager.EndGame(new GameplayFeedback(FeedbackLevel.Info, $"You won! The game lasted: {Application2D.Manager.Timer.Elapsed:g}"), GameResolution.Win);
            Application2D.Manager.Timer.Reset();
        }

        public void HandleKeyPress(char keyChar)
        {
            if (Application2D is null)
            {
                return;
            }

            var map = Application2D.BoardService.GetActiveMap<GameMap>();
            var unit = map?.Interactables.FirstOrDefault(b => b is Player);
            if (map is null || unit is not Player bomber)
            {
                return;
            }

            switch (keyChar)
            {
                case 'd':
                    map.MoveInteractable(bomber, Move2D.Right);
                    break;
                case 'a':
                    map.MoveInteractable(bomber, Move2D.Left);
                    break;
                case 'w':
                    map.MoveInteractable(bomber, Move2D.Forward);
                    break;
                case 's':
                    map.MoveInteractable(bomber, Move2D.Backward);
                    break;
                case 'p':
                    PauseGame();
                    break;
                case 'r':
                    Application2D.Manager.ResetGame();
                    break;
                case 'b':
                    var bomb = bomber.PutBomb();
                    bomb.Attach(this);
                    map.Interactables.Add(bomb);
                    bomb.View.ViewLoaded();
                    break;
            }

            if (int.TryParse(keyChar.ToString(), out var bombIndex))
            {
                bomber.DetonateBombAt(bombIndex - 1);
            }

        }

        public void OnGameFinished(IGameplayFeedback feedback, GameResolution resolution)
        {
            var map = Application2D?.BoardService.GetActiveMap<GameMap>();
            if (map is null)
            {
                return;
            }

            foreach (var unit in map.Interactables)
            {
                unit.Dispose();
            }

            map.Interactables.Clear();
        }

        public void OnGamePaused()
        {
            Debug.WriteLine("Game paused");
        }

        public void OnGameResumed()
        {
            Debug.WriteLine("Game resumed");
        }

        public void OnGameStarted(IGameplayFeedback feedback)
        {
            Debug.WriteLine("Game started");
        }

        public void PauseGame()
        {
            if (Application2D is null)
            {
                return;
            }

            if (Application2D.Manager.State == GameState.Paused)
            {
                Application2D.Manager.ResumeGame();
            }
            else
            {
                Application2D.Manager.PauseGame();
            }
        }

        public void SaveGame()
        {
            var map = Application2D?.BoardService.GetActiveMap<GameMap>();
            map?.SaveProgress();
        }

        public void OnGameReset()
        {
            var map = Application2D?.BoardService.GetActiveMap<GameMap>();
            if (map is null)
            {
                return;
            }

            map.Interactables.Clear();
            map.MapObjects.Clear();
        }
    }
}