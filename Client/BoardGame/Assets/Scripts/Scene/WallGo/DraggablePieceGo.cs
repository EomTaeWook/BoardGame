using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Log;
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
            _circleCollider2D.enabled = false;
            _isDragging = true;
        }
        private void OnMouseDrag()
        {
            if (_isDragging)
            {
                Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = pos;

                Collider2D hitCollider = Physics2D.OverlapPoint(pos);

                if (hitCollider != null && hitCollider.TryGetComponent(out TileGo tileGo))
                {
                    if (_currentHoveredTile != null)
                    {
                        _currentHoveredTile.SetMoveAvailable(false);
                    }
                    _currentHoveredTile = tileGo;
                    transform.position = tileGo.transform.position;
                    tileGo.SetMoveAvailable(true);
                }
            }
        }
        private void OnMouseUp()
        {
            _isDragging = false;

            Vector3 dropPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(dropPos, Vector2.zero);

            _circleCollider2D.enabled = true;


            if(_wallGoSceneController.IsPlayerTurn() == false)
            {
                if (_currentHoveredTile != null)
                {
                    _currentHoveredTile.SetMoveAvailable(false);
                    _currentHoveredTile = null;
                }

                transform.position = _originalPosition;
                return;
            }

            if (hit.collider != null && hit.collider.TryGetComponent(out TileGo tileGo))
            {
                transform.position = tileGo.transform.position;

                if (_currentHoveredTile != null)
                {
                    _currentHoveredTile.SetMoveAvailable(false);
                    _currentHoveredTile = null;
                }

                _wallGoSceneController.SpawnPieceRequest(this, tileGo.Tile.GridPosition);
                return;
            }
            if (_currentHoveredTile != null)
            {
                _currentHoveredTile.SetMoveAvailable(false);
                _currentHoveredTile = null;
            }
            
            transform.position = _originalPosition;
        }
    }
}
