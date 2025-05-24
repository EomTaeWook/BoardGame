using Dignus.Collections;
using System;

namespace Assets.Scripts.GameContents.WallGo
{
    public class WallGoPlayer : IPlayer
    {
        public enum StateType { SpawnPiece, SpawnPiece1, MovePeice, PlaceWall }
        public string AccountId { get; set; }

        public string Nickname { get; set; }

        public bool HasUsedBreakWall { get; set; } = false;

        public ArrayQueue<Piece> PlayerPieces { get; set; } = new ArrayQueue<Piece>();

        public int MovePieceCount { get; set; }

        public Piece LastMovePiece { get; set; }

        public StateType State { get; private set; }
        public DateTime TurnStartTime { get; private set; }

        public WallGoPlayer()
        {
            for (int i = 0; i < 2; ++i)
            {
                PlayerPieces.Add(new Piece(i, this));
            }
        }

        public void ChangeState(StateType state)
        {
            State = state;
        }
        public void StartTurn()
        {
            TurnStartTime = DateTime.UtcNow;
        }
        public void EndTurn()
        {
            MovePieceCount = 2;
            LastMovePiece = null;
        }
    }
}
