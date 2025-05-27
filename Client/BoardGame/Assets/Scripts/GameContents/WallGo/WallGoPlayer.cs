using Assets.Scripts.GameContents.Share;
using Dignus.Collections;
using System;

namespace Assets.Scripts.GameContents.WallGo
{
    public class WallGoPlayer : IPlayer
    {
        public string AccountId { get; private set; }

        public string Nickname { get; private set; }

        public bool HasUsedBreakWall { get; set; } = false;

        public ArrayQueue<Piece> PlayerPieces { get; set; } = new ArrayQueue<Piece>();

        public int MovePieceCount { get; set; } = 0;

        public Piece LastMovePiece { get; set; }

        public StateType State { get; private set; }
        public DateTime TurnStartTime { get; private set; }

        public WallGoPlayer(string accountId, string nickname)
        {
            AccountId = accountId;
            Nickname = nickname;
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
            MovePieceCount = 0;
            LastMovePiece = null;
        }
    }
}