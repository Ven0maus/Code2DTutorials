using GoRogue.Pathing;
using Roguelike.Entities;
using Roguelike.Entities.Actors;
using Roguelike.World;
using Roguelike.World.WorldGen;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Screens
{
    internal class WorldScreen : ScreenSurface
    {
        public readonly Tilemap Tilemap;
        public readonly ActorManager ActorManager;
        public readonly FastAStar Pathfinder;

        public Player Player { get; private set; }

        public WorldScreen(int width, int height) : base(width, height)
        {
            // Setup tilemap
            Tilemap = new Tilemap(width, height);

            // Setup a new surface matching with our tiles
            Surface = new CellSurface(width, height, Tilemap.Tiles);

            // Add the entity component to the world screen, so we can track entities
            ActorManager = new ActorManager();
            SadComponents.Add(ActorManager.EntityComponent);

            // Setup the pathfinder
            Pathfinder = new FastAStar(new LambdaGridView<bool>(Tilemap.Width, Tilemap.Height, (a) => !BlocksMovement(Tilemap[a.X, a.Y].Obstruction)), Distance.Manhattan);
        }

        public void Generate()
        {
            Surface.Clear();

            // Generate new dungeon layout
            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out var dungeonRooms);
            if (dungeonRooms.Count == 0)
                throw new Exception("Faulty dungeon generation, no rooms!");

            var spawnPosition = dungeonRooms[0].Center;

            if (Player == null)
            {
                // Init player if doesn't exist yet
                CreatePlayer(spawnPosition);
            }
            else
            {
                // Update player position to the new position
                Player.Position = spawnPosition;

                // Do a full explore, because the "newly seen" tiles might not fully encompass all tiles because of teleportation.
                Player.ExploreCurrentFov();
            }
            
            // Create npcs
            CreateNpcs(dungeonRooms);
        }

        public void CreatePlayer(Point position)
        {
            Player = new Player(position);
            ActorManager.Add(Player);

            // Initial player stats draw
            ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        public void CreateNpcs(IReadOnlyList<Rectangle> dungeonRooms)
        {
            // Cleanup old npcs but re-add the player
            ScreenContainer.Instance.World.ActorManager.Clear();
            ScreenContainer.Instance.World.ActorManager.Add(Player);

            // between playerlvl -2 or the playerlvl +1
            var levelRequirement = (min: Math.Max(1, Player.Stats.Level - 2), max: Player.Stats.Level + 1);

            const int maxNpcPerRoom = 2;
            foreach (var room in dungeonRooms)
            {
                // Define how many npcs will be in this room
                var npcs = ScreenContainer.Instance.Random.Next(0, maxNpcPerRoom + 1);

                // All positions within the room except the perimeter positions and the player position
                var validPositions = room.Positions()
                    .Except(room.PerimeterPositions().Append(Player.Position))
                    .ToList();

                for (int i=0; i < npcs; i++)
                {
                    // Select a random position from the list
                    var randomPosition = validPositions[ScreenContainer.Instance.Random.Next(0, validPositions.Count)];

                    // Create the goblin npc with the given position and add it to the actor manager
                    // Have the goblins be around the same level as the player
                    var goblin = new Goblin(randomPosition);
                    goblin.Stats.SetLevel(ScreenContainer.Instance.Random.Next(levelRequirement.min, levelRequirement.max + 1));

                    ActorManager.Add(goblin);

                    // Make sure we don't spawn another at this position
                    validPositions.Remove(randomPosition);
                }
            }

            // Update the visibility of actors
            ScreenContainer.Instance.World.ActorManager.UpdateVisibility();
        }

        private static bool BlocksMovement(ObstructionType obstructionType)
        {
            return obstructionType switch
            {
                ObstructionType.MovementBlocked or ObstructionType.FullyBlocked => true,
                _ => false,
            };
        }
    }
}
