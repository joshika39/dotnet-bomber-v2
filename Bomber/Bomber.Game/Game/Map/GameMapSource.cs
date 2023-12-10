using Bomber.Game.Dummies;
using Bomber.Game.Game.Tiles;
using GameFramework.Core.Factories;
using GameFramework.Core.Position;
using GameFramework.Impl.Map.Source;
using GameFramework.Objects.Interactable;
using GameFramework.Objects.Static;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Xml.Linq;

namespace Bomber.Game.Game.Map
{
    public class GameMapSource : JsonMapSource2D<TileTypes>
    {
        private readonly IPositionFactory _factory;
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<DummyEntity> Enemies { get; set; }
        public IEnumerable<DummyBomb> Bombs { get; set; }
        public IPosition2D PlayerPosition { get; set; }

        public GameMapSource(IServiceProvider provider, string name, string description, string filePath, int[,] data, ICollection<IInteractableObject2D> units, int col, int row) : base(provider, filePath, data, units, col, row)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Enemies = units.Select(unit => new DummyEntity(unit.Position.X, unit.Position.Y));
            Bombs = new List<DummyBomb>();
            _factory = provider.GetRequiredService<IPositionFactory>();
            PlayerPosition = _factory.CreatePosition(1, 1);
        }

        public GameMapSource(IServiceProvider provider, string filePath) : base(provider, filePath)
        {
            Name = Query.GetStringAttribute("name") ?? throw new InvalidOperationException("Map name is not specified");
            Description = Query.GetStringAttribute("description") ?? throw new InvalidOperationException("Map description is not specified");
            Enemies = Query.GetObject<List<DummyEntity>>("enemies") ?? new List<DummyEntity>();
            Bombs = Query.GetObject<List<DummyBomb>>("bombs") ?? new List<DummyBomb>();
            var x = Query.GetIntAttribute("player.x") ?? 1;
            var y = Query.GetIntAttribute("player.y") ?? 1;
            _factory = provider.GetRequiredService<IPositionFactory>();
            PlayerPosition = _factory.CreatePosition(x, y);
        }

        public override void SaveLayout(IEnumerable<IStaticObject2D> updatedMapObjects, IEnumerable<IInteractableObject2D> updatedUnits)
        {
            var units = updatedUnits.ToList();
            var player = units.FirstOrDefault(u => u is Player);
            if (player is not null)
            {
                Query.SetAttribute("player.x", player.Position.X);
                Query.SetAttribute("player.y", player.Position.Y);
            }

            Enemies = units.Where(u => u is Enemy).Select(unit => new DummyEntity(unit.Position.X, unit.Position.Y));
            Bombs = units.Where(u => u is Bomb).Select(unit => new DummyBomb(unit.Position.X, unit.Position.Y, (unit as Bomb)!.Radius, (unit as Bomb)!.RemainingTime));
            Query.SetObject("enemies", Enemies);
            Query.SetObject("bombs", Bombs);
        }
    }
}
