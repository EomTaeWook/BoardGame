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
        private GameObject _active;

        public void Init()
        {
            _topWall.gameObject.SetActive(false);
            _rightWall.gameObject.SetActive(false);
            _bottomWall.gameObject.SetActive(false);
            _leftWall.gameObject.SetActive(false);
        }

        public void SetAvailable(bool available)
        {
            _active.SetActive(available);
        }
    }
}
