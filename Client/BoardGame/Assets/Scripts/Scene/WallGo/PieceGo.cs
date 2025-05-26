using Assets.Scripts.GameContents.WallGo;
using Assets.Scripts.Internals;
using Dignus.Unity.Attributes;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    [PrefabPath(Consts.Path.WallGo)]
    public class PieceGo : MonoBehaviour
    {
        [SerializeField]
        protected CircleCollider2D _circleCollider2D;
        [SerializeField]
        protected SpriteRenderer _spriteRenderer;
        [SerializeField]
        protected SpriteRenderer _activeSpriteRenderer;

        protected Piece _piece;

        protected bool _isPlayerPiece;

        public void Init(Piece piece, Color color, bool isPlayerPiece)
        {
            _isPlayerPiece = isPlayerPiece;
            _spriteRenderer.color = color;
            _circleCollider2D.enabled = isPlayerPiece;
            _activeSpriteRenderer.gameObject.SetActive(false);
            _piece = piece;
        }
        public void Active(bool isActive)
        {
            _activeSpriteRenderer.gameObject.SetActive(isActive);
        }

        public Piece GetPiece()
        {
            return _piece;
        }
    }
}
