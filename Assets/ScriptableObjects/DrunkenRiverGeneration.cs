using Assets.Helpers;
using Assets.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DrunkenRiverGeneration", menuName = "Algorithms/DrunkenRiverGeneration")]
    public class DrunkenRiverGeneration : AlgorithmBase
    {
        public int MinRiverQuota;
        public int MaxRiverQuota;
        public int MinDistanceBetweenRiverStartPoints;
        public GroundTileType[] StartingTileTypes;

        [Range(0, 100)]
        public int RiverDriftChance;

        private System.Random _random;

        public override void Apply(TilemapStructure tilemap)
        {
            _random = new System.Random(tilemap.Grid.Seed);
            var rivers = new List<DrunkenRiver>();

            var validStartPositions = TilemapHelper.GetTilesByType(tilemap, StartingTileTypes.Select(a => (int)a));
            var amountOfRivers = _random.Next(MinRiverQuota, MaxRiverQuota + 1);

            for (int i=0; i < amountOfRivers; i++)
            {
                // Get valid startPoint with respect to distance between existing rivers
                var startPoint = GetValidStartPosition(validStartPositions, rivers);
                if (!startPoint.HasValue) break;

                // Find valid endPos (closest deep water tile to startPos)
                var endPos = TilemapHelper.FindClosestTileByType(tilemap, startPoint.Value, (int)GroundTileType.DeepWater);
                if (!endPos.HasValue)  break;

                // Build river from start to end position
                var river = new DrunkenRiver(_random, RiverDriftChance, startPoint.Value, endPos.Value);
                if (river.Build())
                {
                    rivers.Add(river);
                }
            }

            // Set river tiles into tilemap
            int riverTileId = (int)GroundTileType.River;
            foreach (var riverPosition in rivers.SelectMany(a => a.RiverPositions))
            {
                tilemap.SetTile(riverPosition.x, riverPosition.y, riverTileId);
            }
        }
        private Vector2Int? GetValidStartPosition(List<Vector2Int> startPositions, List<DrunkenRiver> rivers)
        {
            // If no tiles available return null
            if (!startPositions.Any()) return null;

            // Get a random starting tile
            var startPoint = startPositions[_random.Next(0, startPositions.Count)];
            startPositions.Remove(startPoint);

            // Also here we have an attempt check
            const int maxAttempts = 500;
            int attempt = 0;

            // While there is any river where it's startpoint isn't enough distance away from our new startPoint
            while (rivers.Any(river => !river.CheckDistance(startPoint, MinDistanceBetweenRiverStartPoints)))
            {
                if (attempt >= maxAttempts)
                {
                    return null;
                }
                attempt++;

                // Get the next random tile
                if (!startPositions.Any()) return null;
                startPoint = startPositions[_random.Next(0, startPositions.Count)];
                startPositions.Remove(startPoint);
            }

            return startPoint;
        }

        class DrunkenRiver
        {
            public Vector2Int StartPos;
            public Vector2Int EndPos;
            public HashSet<Vector2Int> RiverPositions;

            private readonly System.Random _random;
            private readonly int _riverDriftChance;
            private const int _maxAttempts = 1000;

            public DrunkenRiver(System.Random random, int riverDriftChance, Vector2Int startPos, Vector2Int endPos)
            {
                _random = random;
                _riverDriftChance = riverDriftChance;
                StartPos = startPos;
                EndPos = endPos;
                RiverPositions = new HashSet<Vector2Int> { StartPos, EndPos };
            }

            public bool CheckDistance(Vector2Int startPos, int minDistance)
            {
                float distance = ((startPos.x - StartPos.x) * (startPos.x - StartPos.x) + (startPos.y - StartPos.y) * (startPos.y - StartPos.y));
                return distance > minDistance * minDistance;
            }

            public bool Build()
            {
                Vector2Int currentPos = StartPos;

                int attempts = 0;
                while (currentPos != EndPos)
                {
                    // Make sure we never end in an infinite loop
                    if (attempts >= _maxAttempts) return false;
                    attempts++;

                    var differenceX = currentPos.x - EndPos.x;
                    var differenceY = currentPos.y - EndPos.y;

                    // If we are on a straight path towards the endPos, we want to have some chance to drift
                    if (differenceX == 0 || differenceY == 0)
                    {
                        var driftChance = _random.Next(0, 100);
                        if (driftChance <= _riverDriftChance)
                        {
                            var difference = _random.Next(1, 4);
                            var direction = _random.Next(0, 2);

                            // One of our axis is in a straight line, we don't want to keep going straight so lets drift
                            if (differenceX == 0)
                            {
                                for (int i = 0; i < difference; i++)
                                {
                                    currentPos = new Vector2Int(direction == 0 ? currentPos.x - 1 : currentPos.x + 1, currentPos.y);
                                    RiverPositions.Add(currentPos);
                                }
                            }
                            else if (differenceY == 0)
                            {
                                for (int i = 0; i < difference; i++)
                                {
                                    currentPos = new Vector2Int(currentPos.x, direction == 0 ? currentPos.y - 1 : currentPos.y + 1);
                                    RiverPositions.Add(currentPos);
                                }
                            }
                        }
                    }

                    // Basic direction guide towards the end position
                    if (differenceX > 0)
                    {
                        currentPos = new Vector2Int(currentPos.x - 1, currentPos.y);
                        RiverPositions.Add(currentPos);
                    }
                    else if (differenceX < 0)
                    {
                        currentPos = new Vector2Int(currentPos.x + 1, currentPos.y);
                        RiverPositions.Add(currentPos);
                    }
                    if (differenceY > 0)
                    {
                        currentPos = new Vector2Int(currentPos.x, currentPos.y - 1);
                        RiverPositions.Add(currentPos);
                    }
                    else if (differenceY < 0)
                    {
                        currentPos = new Vector2Int(currentPos.x, currentPos.y + 1);
                        RiverPositions.Add(currentPos);
                    }
                }
                return true;
            }
        }
    }
}
