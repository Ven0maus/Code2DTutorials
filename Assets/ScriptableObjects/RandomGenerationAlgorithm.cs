using Assets.Tilemaps;
using System;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RandomGeneration", menuName = "Algorithms/RandomGeneration")]
    public class RandomGenerationAlgorithm : AlgorithmBase
    {
        public override void Apply(TilemapStructure tilemap)
        {
            var validEnumValues = (GroundTileType[])Enum.GetValues(typeof(GroundTileType));
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    // We use Unity's Random class to retrieve a random value
                    // between 0 and the length of the enums available.
                    // Cast enum random value to the underlying int,
                    // we defined in the enum type.
                    var randomValue = (int)validEnumValues[UnityEngine.Random.Range(0, validEnumValues.Length)];

                    // We set the tile type into our tilemap data structure
                    tilemap.SetTile(x, y, randomValue);
                }
            }
        }
    }
}
