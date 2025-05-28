using Assets.Scripts.GameContents;
using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Network;
using BG.GameServer.ServerContents.EventHandler;
using Protocol.GSAndClient;
using System.Collections.Generic;

namespace BG.GameServer.ServerContents
{
    internal class WallGoRoom : RoomBase
    {
        private readonly WallGoBoard _wallGoBoard;
        private readonly WallGoEventHandler _wallGoEventHandler;
        public WallGoRoom(int roomNumber) : base(roomNumber, GameType.WallGo, 1, 4)
        {
            _wallGoEventHandler = new WallGoEventHandler();
            _wallGoBoard = new WallGoBoard(_wallGoEventHandler);

            RegisterEventHandlers();
        }

        public override bool StartGame()
        {
            if (MinUserCount > GetMembers().Count)
            {
                return false;
            }

            var players = new List<IPlayer>();
            players.AddRange(GetMembers());
            _wallGoBoard.SetPlayers(players);
            _wallGoBoard.StartGame();

            return true;
        }
        public override void Dispose()
        {
            _wallGoEventHandler.StartTurn -= WallGoEventHandler_StartTurn;
            _wallGoEventHandler.EndGame -= WallGoEventHandler_EndGame;
            _wallGoEventHandler.StartGame -= WallGoEventHandler_StartGame;
            _wallGoEventHandler.ChangeState -= WallGoEventHandler_ChangeState;
            _wallGoEventHandler.SpawnPiece -= WallGoEventHandler_SpawnPiece;
            _wallGoEventHandler.MovePiece -= WallGoEventHandler_MovePiece;
            _wallGoEventHandler.PlaceWall -= WallGoEventHandler_PlaceWall;
            _wallGoEventHandler.RemoveWall -= WallGoEventHandler_RemoveWall;
        }
        private void RegisterEventHandlers()
        {
            _wallGoEventHandler.StartTurn += WallGoEventHandler_StartTurn;
            _wallGoEventHandler.StartGame += WallGoEventHandler_StartGame;
            _wallGoEventHandler.EndGame += WallGoEventHandler_EndGame;
            _wallGoEventHandler.ChangeState += WallGoEventHandler_ChangeState;
            _wallGoEventHandler.SpawnPiece += WallGoEventHandler_SpawnPiece;
            _wallGoEventHandler.MovePiece += WallGoEventHandler_MovePiece;
            _wallGoEventHandler.PlaceWall += WallGoEventHandler_PlaceWall;
            _wallGoEventHandler.RemoveWall += WallGoEventHandler_RemoveWall;
        }

        public void MovePieceReqeust(MovePiece movePiece)
        {
            var wallGoPlayer = _wallGoBoard.GetPlayer(movePiece.AccountId);

            _wallGoBoard.MovePiece(wallGoPlayer, movePiece.PieceId, movePiece.Dest);
        }
        public bool PlaceWallReqeust(PlaceWall placeWall)
        {
            var wallGoPlayer = _wallGoBoard.GetPlayer(placeWall.AccountId);

            return _wallGoBoard.TryPlaceWall(wallGoPlayer, placeWall.Point, placeWall.Direction);
        }
        public bool RemoveWallReqeust(RemoveWall removeWall)
        {
            var wallGoPlayer = _wallGoBoard.GetPlayer(removeWall.AccountId);

            return _wallGoBoard.TryRemoveWall(wallGoPlayer, removeWall.Point, removeWall.Direction);
        }
        public void SpawnPieceReqeust(SpawnPiece spawnPiece)
        {
            var wallGoPlayer = _wallGoBoard.GetPlayer(spawnPiece.AccountId);

            _wallGoBoard.TrySpawnPiece(wallGoPlayer, spawnPiece.PieceId, spawnPiece.SpawnedPoint);
        }

        private void WallGoEventHandler_RemoveWall(RemoveWall obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.RemoveWall, obj));
        }
        private void WallGoEventHandler_PlaceWall(PlaceWall obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.PlaceWall, obj));
        }

        private void WallGoEventHandler_MovePiece(MovePiece obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.MovePiece, obj));
        }

        private void WallGoEventHandler_SpawnPiece(SpawnPiece obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.SpawnPiece, obj));
        }

        private void WallGoEventHandler_ChangeState(ChangeState obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.ChangeState, obj));
        }

        private void WallGoEventHandler_StartGame(StartGame obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.StartGame, obj));
        }
        private void WallGoEventHandler_EndGame(EndGame obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.EndGame, obj));
        }

        private void WallGoEventHandler_StartTurn(StartTurn obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.StartTurn, obj));
        }
    }
}
