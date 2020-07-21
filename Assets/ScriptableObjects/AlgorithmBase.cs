using Assets.Tilemaps;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    public abstract class AlgorithmBase : ScriptableObject
    {
        public abstract void Apply(TilemapStructure tilemap);
    }
}