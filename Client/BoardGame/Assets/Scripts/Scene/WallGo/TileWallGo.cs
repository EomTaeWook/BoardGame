using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using DataContainer;
using Dignus.Unity.Attributes;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    [PrefabPath(Consts.Path.WallGo)]
    public class TileWallGo : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _availableTileWall;

        [SerializeField]
        private SpriteRenderer _tileWall;

        private Tile _tile;

        private TileWallDirectionType _direction;
        public void Init(Tile tile, TileWallDirectionType direction)
        {
            _tile = tile;
            _direction = direction;

            if (_direction == TileWallDirectionType.Top)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        public Tile GetTile()
        {
            return _tile;
        }
        public Direction GetDirection()
        {
            if (_direction == TileWallDirectionType.Right)
            {
                return Direction.Right;
            }
            else if (_direction == TileWallDirectionType.Top)
            {
                return Direction.Up;
            }

            throw new System.InvalidOperationException($"unsupported direction type: {_direction}");
        }
        public void RemoveWall()
        {
            _tileWall.gameObject.SetActive(false);
        }
        public void PlaceWall(Color color)
        {
            _tileWall.color = color;
            _tileWall.gameObject.SetActive(true);
        }
        public void SetWallAvailable(bool available)
        {
            _availableTileWall.gameObject.SetActive(available);
        }
    }
}

