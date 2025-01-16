using Roguelike.Screens;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.World.WorldGen
{
    internal static class DungeonGenerator
    {
        private const int MaxAttempts = 100;
        private const int ChanceForDoorPlacement = 60;

        public static void Generate(Tilemap tilemap, int maxRooms, int minRoomSize, int maxRoomSize, out IReadOnlyList<Rectangle> dungeonRooms)
        {
            tilemap.Reset();

            var random = ScreenContainer.Instance.Random;
            var rooms = new List<Rectangle>();
            dungeonRooms = rooms;

            const int borderSize = 2;

            // Generate rooms
            for (int attempts = 0; attempts < MaxAttempts; attempts++)
            {
                if (rooms.Count == maxRooms) break;

                int width = random.Next(minRoomSize, maxRoomSize);
                int height = random.Next(minRoomSize, maxRoomSize);

                // Exclude border tiles so we can set walls properly + keep an empty space as the border
                int x = random.Next(borderSize, tilemap.Width - width - borderSize);
                int y = random.Next(borderSize, tilemap.Height - height - borderSize);

                Rectangle newRoom = new(x, y, width, height);

                // Check for overlap with existing rooms
                if (!rooms.Any(r => r.Intersects(newRoom)))
                {
                    rooms.Add(newRoom);
                    CarveRoom(tilemap, newRoom);
                }
            }

            // Connect rooms with tunnels
            for (int i = 1; i < rooms.Count; i++)
            {
                Rectangle roomA = rooms[i - 1];
                Rectangle roomB = rooms[i];

                CarveTunnel(tilemap, roomA.Center, roomB.Center);
            }

            AddWalls(tilemap);
            AddDoors(tilemap, rooms);
            InsertStairs(tilemap, rooms);
        }

        private static void InsertStairs(Tilemap tilemap, List<Rectangle> rooms)
        {
            // Select a random starting room, excluding the first room (player spawn)
            var randomRoom = rooms[ScreenContainer.Instance.Random.Next(1, rooms.Count)];

            // Place a stairs down at the room
            tilemap[randomRoom.Center.ToIndex(tilemap.Width)].Type = TileType.StairsDown;
        }

        private static void CarveRoom(Tilemap tilemap, Rectangle room)
        {
            for (int x = room.X; x < room.X + room.Width; x++)
            {
                for (int y = room.Y; y < room.Y + room.Height; y++)
                {
                    // Set floor tile
                    tilemap[x, y].Type = TileType.Floor;
                }
            }
        }

        private static void CarveTunnel(Tilemap tilemap, Point start, Point end)
        {
            Point current = start;

            while (current.X != end.X)
            {
                // Set floor tile
                tilemap[current.X, current.Y].Type = TileType.Floor;
                current = new Point(current.X + (current.X < end.X ? 1 : -1), current.Y);
            }

            while (current.Y != end.Y)
            {
                // Set floor tile
                tilemap[current.X, current.Y].Type = TileType.Floor;
                current = new Point(current.X, current.Y + (current.Y < end.Y ? 1 : -1));
            }
        }

        private static void AddWalls(Tilemap tilemap)
        {
            for (int x=0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    // Check if the current tile is a floor
                    if (tilemap[x, y].Type == TileType.Floor)
                    {
                        // Get neighbors of the current tile
                        var neighbors = new Point(x, y).GetNeighborPoints(true);
                        foreach (var neighbor in neighbors)
                        {
                            // If a neighbor is inbounds and not a floor, set it as a wall
                            if (tilemap.InBounds(neighbor.X, neighbor.Y) && tilemap[neighbor.X, neighbor.Y].Type != TileType.Floor)
                            {
                                // Set wall glyph and make obstruction fully blocked
                                tilemap[neighbor.X, neighbor.Y].Type = TileType.Wall;
                            }
                        }
                    }
                }
            }
        }

        private static void AddDoors(Tilemap tilemap, List<Rectangle> rooms)
        {
            foreach (var room in rooms)
            {
                var wallPositions = room.Expand(1, 1).PerimeterPositions();

                // Check if tile is a floor and if both horizontal / vertical neighbors are a match with eachother
                foreach (var position in wallPositions)
                {
                    if (!tilemap.InBounds(position.X, position.Y)) continue;

                    // If tile is a floor
                    if (tilemap[position.X, position.Y].Type == TileType.Floor)
                    {
                        // Get directional neighbors
                        var north = tilemap[position.X, position.Y + 1, false];
                        var south = tilemap[position.X, position.Y - 1, false];
                        var east = tilemap[position.X + 1, position.Y, false];
                        var west = tilemap[position.X - 1, position.Y, false];

                        if ((north?.Type == TileType.Floor && south?.Type == TileType.Floor && east?.Type == TileType.Wall && west?.Type == TileType.Wall) ||
                            (north?.Type == TileType.Wall && south?.Type == TileType.Wall && east?.Type == TileType.Floor && west?.Type == TileType.Floor))
                        {
                            // % chance to place a door
                            if (ScreenContainer.Instance.Random.Next(100) < ChanceForDoorPlacement)
                            {
                                tilemap[position.X, position.Y].Type = TileType.Door;
                            }
                        }
                    }
                }
            }
        }
    }
}