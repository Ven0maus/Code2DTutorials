using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Tilemaps
{
    public class TileTypes
    {
        [Serializable]
        public class GroundTiles : TileData<GroundTileType>
        { }

        [Serializable]
        public class ObjectTiles : TileData<ObjectTileType>
        { }

        public abstract class TileData<T> : TileData
            where T : Enum 
        {
            public T TileType;
            public override int TileTypeId { get { return Convert.ToInt32(TileType); } }
        }

        public abstract class TileData 
        {
            public Sprite Sprite;
            public Color Color;
            public TileBase Tile;
            public Tile.ColliderType ColliderType;
            public virtual int TileTypeId { get; }
        }
    }
}
