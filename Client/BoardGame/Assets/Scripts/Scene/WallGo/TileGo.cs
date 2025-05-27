using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    [PrefabPath(Consts.Path.WallGo)]
    public class TileGo : MonoBehaviour
    {
        public Tile Tile { get; set; }

        [SerializeField]
        private SpriteRenderer _topWall;
        [SerializeField]
        private SpriteRenderer _rightWall;
        [SerializeField]
        private SpriteRenderer _bottomWall;
        [SerializeField]
        private SpriteRenderer _leftWall;

        [SerializeField]
        private CapsuleCollider2D _topWallCollider;
        [SerializeField]
        private CapsuleCollider2D _rightWallCollider;
        [SerializeField]
        private CapsuleCollider2D _bottomWallCollider;
        [SerializeField]
        private CapsuleCollider2D _leftWallCollider;

        [SerializeField]
        private SpriteRenderer _availableTopWall;
        [SerializeField]
        private SpriteRenderer _availableRightWall;
        [SerializeField]
        private SpriteRenderer _availableBottomWall;
        [SerializeField]
        private SpriteRenderer _availableLeftWall;

        [SerializeField]
        private BoxCollider2D _tileCollider;

        [SerializeField]
        private SpriteRenderer _available;

        public void Init()
        {
            _topWall.gameObject.SetActive(false);
            _rightWall.gameObject.SetActive(false);
            _bottomWall.gameObject.SetActive(false);
            _leftWall.gameObject.SetActive(false);

            _topWallCollider.enabled = false;
            _bottomWallCollider.enabled = false;
            _rightWallCollider.enabled = false;
            _leftWallCollider.enabled = false;
        }


        public bool TryGetWallDirection(Collider2D collider, out Direction direction)
        {
            direction = Direction.Up;
            if (_topWallCollider == collider)
            {
                direction = Direction.Up;
                return true;
            }
            else if (_rightWallCollider == collider)
            {
                direction = Direction.Right;
                return true;
            }
            else if (_bottomWallCollider == collider)
            {
                direction = Direction.Down;
                return true;
            }
            else if (_leftWallCollider == collider)
            {
                direction = Direction.Left;
                return true;
            }
            return false;
        }

        public void SetMoveAvailable(bool available)
        {
            _available.gameObject.SetActive(available);
        }
        public bool IsAvailable()
        {
            return _available.gameObject.activeSelf;
        }
        public void SetWallAvailable(bool available, Direction direction)
        {
            _tileCollider.enabled = !available;

            if (direction == Direction.Up)
            {
                _topWallCollider.enabled = available;
                _availableTopWall.gameObject.SetActive(available);
            }
            else if (direction == Direction.Down)
            {
                _bottomWallCollider.enabled = available;
                _availableBottomWall.gameObject.SetActive(available);
            }
            else if (direction == Direction.Right)
            {
                _rightWallCollider.enabled = available;
                _availableRightWall.gameObject.SetActive(available);
            }
            else if (direction == Direction.Left)
            {
                _leftWallCollider.enabled = available;
                _availableLeftWall.gameObject.SetActive(available);
            }
        }

        public void PlaceWall(Direction direction, Color color)
        {
            if (direction == Direction.Down)
            {
                Tile.WallBottom = true;
                this._bottomWall.color = color;
                this._bottomWall.gameObject.SetActive(true);
            }
            else if (direction == Direction.Up)
            {
                Tile.WallTop = true;
                this._topWall.color = color;
                this._topWall.gameObject.SetActive(true);
            }
            else if (direction == Direction.Left)
            {
                Tile.WallLeft = true;
                this._leftWall.color = color;
                this._leftWall.gameObject.SetActive(true);            
            }
            else if (direction == Direction.Right)
            {
                Tile.WallRight = true;
                this._rightWall.color = color;
                this._rightWall.gameObject.SetActive(true);
            }
        }
        public void RemoveWall(Direction direction)
        {
            if (direction == Direction.Down)
            {
                Tile.WallBottom = false;
                this._bottomWall.gameObject.SetActive(false);
            }
            else if (direction == Direction.Up)
            {
                Tile.WallTop = false;
                this._topWall.gameObject.SetActive(false);
            }
            else if (direction == Direction.Left)
            {
                Tile.WallLeft = false;
                this._leftWall.gameObject.SetActive(false);
            }
            else if (direction == Direction.Right)
            {
                Tile.WallRight = false;
                this._rightWall.gameObject.SetActive(false);
            }
        }
    }
}
