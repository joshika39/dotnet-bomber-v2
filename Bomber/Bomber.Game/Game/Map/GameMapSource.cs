using System.Drawing;
using Bomber.Game.Dummies;
using Bomber.Game.Game.Tiles;
using GameFramework.Board;
using GameFramework.Core.Position;
using GameFramework.Core.Position.Factories;
using GameFramework.Impl.Map.Source;
using GameFramework.Objects.Interactable;
using GameFramework.Objects.Static;
using Microsoft.Extensions.DependencyInjection;

namespace Bomber.Game.Game.Map
{
    public class GameMapSource : JsonMapSource2D
    {
        private readonly IPositionFactory _factory;
        private readonly IBoardService _boardService;
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<DummyEntity> Enemies { get; set; }
        public IEnumerable<DummyBomb> Bombs { get; set; }
        public IPosition2D PlayerPosition { get; set; }

        public override void SaveLayout(IEnumerable<IStaticObject2D> updatedMapObjects,
            IEnumerable<IInteractableObject2D> updatedUnits)
        {
            var units = updatedUnits.ToList();
            var player = units.FirstOrDefault(u => u is Player);
            if (player is not null)
            {
                Query.SetAttribute("player.x", player.Position.X);
                Query.SetAttribute("player.y", player.Position.Y);
            }

            Enemies = units.Where(u => u is Enemy).Select(unit => new DummyEntity(unit.Position.X, unit.Position.Y));
            Bombs = units.Where(u => u is Bomb).Select(unit => new DummyBomb(unit.Position.X, unit.Position.Y,
                (unit as Bomb)!.Radius, (unit as Bomb)!.RemainingTime));
            Query.SetObject("enemies", Enemies);
            Query.SetObject("bombs", Bombs);
        }

        public GameMapSource(string filePath, string name, string description, IServiceProvider provider, int col,
            int row, Color? bgColor = null, ICollection<IInteractableObject2D>? interactables = null,
            bool bypass = false) : base(filePath, provider, col, row, bgColor, interactables, bypass)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            interactables ??= new List<IInteractableObject2D>();
            Enemies = interactables.Select(unit => new DummyEntity(unit.Position.X, unit.Position.Y));
            Bombs = new List<DummyBomb>();
            _factory = provider.GetRequiredService<IPositionFactory>();
            PlayerPosition = _factory.CreatePosition(1, 1);
            _boardService = provider.GetRequiredService<IBoardService>();
        }

        public GameMapSource(string filePath, IServiceProvider provider) : base(filePath, provider, Color.Black)
        {
            Name = Query.GetStringAttribute("name") ?? throw new InvalidOperationException("Map name is not specified");
            Description = Query.GetStringAttribute("description") ??
                          throw new InvalidOperationException("Map description is not specified");
            Enemies = Query.GetObject<List<DummyEntity>>("enemies") ?? new List<DummyEntity>();
            Bombs = Query.GetObject<List<DummyBomb>>("bombs") ?? new List<DummyBomb>();
            var x = Query.GetIntAttribute("player.x") ?? 1;
            var y = Query.GetIntAttribute("player.y") ?? 1;
            _factory = provider.GetRequiredService<IPositionFactory>();
            PlayerPosition = _factory.CreatePosition(x, y);
            _boardService = provider.GetRequiredService<IBoardService>();
        }

        protected override Func<int, IPosition2D, IStaticObject2D> GetConverter()
        {
            return (i, position) =>
            {
                if (i == 1)
                {
                    return TileFactory2D.CreateStaticObject2D(position, Color.Gray, true);
                }

                if (i == 0)
                {
                    return new BomberTile(position, Gameplay.Application2D!.BoardService);
                }
                
                throw new InvalidOperationException("Invalid tile type");
            };
        }
    }
}