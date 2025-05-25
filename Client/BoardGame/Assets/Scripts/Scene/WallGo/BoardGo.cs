using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Dignus.Collections;
using Dignus.Unity.Extensions;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    public class BoardGo : MonoBehaviour
    {

        private readonly ArrayQueue<TileGo> _tiles = new();
        private void Awake()
        {
            for (int i = 0; i < 7; ++i)
            {
                for (int ii = 0; ii < 7; ++ii)
                {
                    var tileGo = this.InstantiateWithPool<TileGo>();

                    tileGo.Tile = new Tile()
                    {
                        GridPos = new Point(i, ii)
                    };
                    tileGo.transform.position = new Vector3(i, ii, 0);
                    _tiles.Add(tileGo);
                }
            }
        }

        public void GameStart()
        {

        }
        public void SpawnPiece(SpawnPiece spawnPiece)
        {

        }

        public void MovePiece(MovePiece movePiece)
        {

        }

        public void Dispose()
        {
            foreach (var item in _tiles)
            {
                item.Recycle();
            }
            _tiles.Clear();
        }
    }
}
