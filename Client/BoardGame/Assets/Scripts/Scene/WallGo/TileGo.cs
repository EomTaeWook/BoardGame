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

        public void SetMoveAvailable(bool available)
        {
            _available.gameObject.SetActive(available);
        }
        public bool IsAvailable()
        {
            return _available.gameObject.activeSelf;
        }
    }
}
