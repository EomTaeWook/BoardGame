using Assets.Scripts.Extensions;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using Dignus.Collections;
using Dignus.Unity.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scene.WallGo
{
    public class BoardGo : MonoBehaviour
    {
        private Dictionary<string, ArrayQueue<PieceGo>> _playerPiecesByAccountId = new();
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

        public void SpawnPiece(string accountId,
            Color color,
            Piece spawnPiece,
            bool isPlayerPiece
            )
        {
            if (_playerPiecesByAccountId.TryGetValue(accountId, out ArrayQueue<PieceGo> pieces) == false)
            {
                pieces = new ArrayQueue<PieceGo>();
                _playerPiecesByAccountId.Add(accountId, pieces);
            }

            var pieceGo = this.InstantiateWithPool<PieceGo>();
            pieceGo.Init(spawnPiece, color, isPlayerPiece);
            _playerPiecesByAccountId[accountId].Add(pieceGo);
            pieceGo.transform.position = new Vector3(spawnPiece.GridPosition.X, spawnPiece.GridPosition.Y);
        }

        public void MovePiece(Piece movePiece)
        {

        }

        public void Dispose()
        {
            foreach (var kv in _playerPiecesByAccountId)
            {
                foreach (var value in kv.Value)
                {
                    value.Recycle();
                }
                kv.Value.Clear();
            }
            _playerPiecesByAccountId.Clear();

            foreach (var item in _tiles)
            {
                item.Recycle();
            }
            _tiles.Clear();
        }
    }
}
