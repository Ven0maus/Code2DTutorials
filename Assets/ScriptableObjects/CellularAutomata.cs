using Assets.Tilemaps;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CellularAutomata", menuName = "Algorithms/CellularAutomata")]
    public class CellularAutomata : AlgorithmBase
    {
        public int MinAlive, Repetitions;

        [Tooltip("If this is checked, ReplacedBy will have no effect.")]
        public bool ReplaceByDominantTile;

        public ObjectTileType TargetTile, ReplacedBy;

        public override void Apply(TilemapStructure tilemap)
        {
            int targetTileId = (int)TargetTile;
            int replaceTileId = (int)ReplacedBy;
            for (int i = 0; i < Repetitions; i++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    for (int y = 0; y < tilemap.Height; y++)
                    {
                        // Check if the current tile is our target tile
                        var tile = tilemap.GetTile(x, y);
                        if (tile == targetTileId)
                        {
                            // Retrieve all 8 neighbors of our current tile
                            var neighbors = tilemap.GetNeighbors(x, y);

                            // Count all the neighbors that are of type target tile
                            int targetTilesCount = neighbors.Count(a => a.Value == targetTileId);

                            // If the min alive count is not reached, we replace the tile
                            if (targetTilesCount < MinAlive)
                            {
                                if (ReplaceByDominantTile)
                                {
                                    // Group tiles on tiletype, then order them in descending order based on group size
                                    // Select the group's key which is the tiletype because thats what we grouped on
                                    // And select the first one (first group's key), because that's the dominant tile type
                                    var dominantTile = neighbors
                                        .GroupBy(a => a.Value)
                                        .OrderByDescending(a => a.Count())
                                        .Select(a => a.Key)
                                        .First();

                                    tilemap.SetTile(x, y, dominantTile, setDirty: false);
                                }
                                else
                                {
                                    tilemap.SetTile(x, y, replaceTileId, setDirty: false);
                                }     
                            }
                        }
                    }
                }
            }
        }
    }
}
