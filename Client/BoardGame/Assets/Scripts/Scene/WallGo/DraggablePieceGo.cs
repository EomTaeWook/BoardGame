using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    [PrefabPath(Consts.Path.WallGo)]
    public class DraggablePieceGo : MonoBehaviour
    {
        [SerializeField]
        protected CircleCollider2D _circleCollider2D;
        [SerializeField]
        protected SpriteRenderer _spriteRenderer;

        private Vector3 _originalPosition;
        private bool _isDragging = false;
        private TileGo _currentHoveredTile;
        private Camera _camera;

        public Piece Piece { get; private set; }

        protected WallGoSceneController _wallGoSceneController;

        public void Init(WallGoSceneController wallGoSceneController, Piece piece, Color color)
        {
            _wallGoSceneController = wallGoSceneController;
            _spriteRenderer.color = color;
            _camera = Camera.main;
            Piece = piece;
        }
        private void OnMouseDown()
        {
            _originalPosition = transform.position;
            _isDragging = true;
            _circleCollider2D.enabled = false;
        }
        private void OnMouseDrag()
        {
            if (_isDragging)
            {
                Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                transform.position = pos;

                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

                if (hit.collider != null && hit.collider.TryGetComponent(out TileGo tileGo))
                {
                    if (_currentHoveredTile != null)
                    {
                        _currentHoveredTile.SetAvailable(false);
                    }
                    _currentHoveredTile = tileGo;
                    transform.position = tileGo.transform.position;
                    tileGo.SetAvailable(true);
                }
            }
        }
        private void OnMouseUp()
        {
            _isDragging = false;

            Vector3 dropPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(dropPos, Vector2.zero);

            _circleCollider2D.enabled = true;

            if (hit.collider != null && hit.collider.TryGetComponent(out TileGo tileGo))
            {
                transform.position = tileGo.transform.position;

                if (_currentHoveredTile != null)
                {
                    _currentHoveredTile.SetAvailable(false);
                    _currentHoveredTile = null;
                }

                _wallGoSceneController.SpawnPieceRequest(this, tileGo.Tile.GridPos);
                return;
            }

            // 실패 → 원위치
            transform.position = _originalPosition;
        }
    }
}
