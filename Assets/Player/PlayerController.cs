using Assets.Tilemaps;
using UnityEngine;

namespace Assets.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private TileGrid _grid;
        private Rigidbody2D _rigidbody;
        private TilemapStructure _groundMap;

        [SerializeField]
        private float _speed = 4f;

        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _groundMap = _grid.GetTilemap(TilemapType.Ground);
        }

        // Update is called once per frame
        private void Update()
        {
            Apply2DPhysicsMovement();
            DropSnow();
        }

        private void Apply2DPhysicsMovement()
        {
            var xMove = Input.GetAxisRaw("Horizontal");
            var yMove = Input.GetAxisRaw("Vertical");
            _rigidbody.velocity = new Vector2(xMove * _speed, yMove * _speed);
        }

        private void DropSnow()
        {
            // Grab the player position, floor the float to an int
            // We should floor, because rounding means that if we stand in the upper right corner of the tile
            // It will already give us the coordinates of the next tile which is incorrect since we're still standing on the previous tile
            var playerPositionXY = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

            // Set the tile to snow, there is a InBounds check in this method, and it won't trigger map updates if the type doesn't change
            _groundMap.SetTile(playerPositionXY.x, playerPositionXY.y, (int)GroundTileType.Snow, true);
        }
    }
}
