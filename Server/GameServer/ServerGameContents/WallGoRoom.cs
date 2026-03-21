using Assets.Scripts.GameContents;
using Assets.Scripts.GameContents.Share;
using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Actors;
using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents.EventHandler;
using Dignus.Actor.Core;
using Dignus.Actor.Core.Messages;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BG.GameServer.ServerGameContents
{
    internal class WallGoRoom : RoomBaseActor
    {
        public const int MaxPlayerCount = 4;
        public const int MinPlayerCount = 2;

        private readonly WallGoEventHandler _wallGoEventHandler;
        private readonly WallGoBoard _wallGoBoard;
        public WallGoRoom(int roomNumber, IActorRef roomManagerRef) : base(roomNumber, GameType.WallGo, MaxPlayerCount, roomManagerRef)
        {
            _wallGoEventHandler = new WallGoEventHandler();
            _wallGoBoard = new WallGoBoard(_wallGoEventHandler);

            RegisterEventHandlers();
        }
        public override void Dispose()
        {
            _wallGoBoard.Stop();
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
        public override StartGameRoomReason StartGame()
        {
            if (MinPlayerCount > _accountIdToPlayerMap.Count)
            {
                return StartGameRoomReason.NotEnoughUser;
            }

            var players = new List<IPlayer>();
            players.AddRange(_accountIdToPlayerMap.Values);
            _wallGoBoard.SetPlayers(players);
            _wallGoBoard.StartGame();

            return StartGameRoomReason.Success;
        }

        public ValueTask HandleMovePieceReqeust(MovePieceReqeust message, Player player)
        {
            var movePiece = new MovePiece()
            {
                AccountId = player.AccountId,
                Dest = new Point(message.MovePointX, message.MovePointY),
                PieceId = message.PieceId
            };
            var wallGoPlayer = _wallGoBoard.GetPlayer(movePiece.AccountId);
            _wallGoBoard.MovePiece(wallGoPlayer, movePiece.PieceId, movePiece.Dest);

            return ValueTask.CompletedTask;
        }
        public ValueTask HandlePlaceWallReqeust(PlaceWallReqeust message, Player player)
        {
            var placeWall = new PlaceWall()
            {
                AccountId = player.AccountId,
                Direction = (Direction)message.Direction,
                Point = new Point(message.TilePointX, message.TilePointY)
            };

            var wallGoPlayer = _wallGoBoard.GetPlayer(placeWall.AccountId);
            _wallGoBoard.TryPlaceWall(wallGoPlayer, placeWall.Point, placeWall.Direction);

            return ValueTask.CompletedTask;
        }
        public ValueTask HandleRemoveWallReqeust(RemoveWallReqeust message, Player player)
        {
            var removeWall = new RemoveWall()
            {
                AccountId = player.AccountId,
                Point = new Point(message.TilePointX, message.TilePointY),
                Direction = (Direction)message.Direction,
            };
            var wallGoPlayer = _wallGoBoard.GetPlayer(removeWall.AccountId);
            _wallGoBoard.TryRemoveWall(wallGoPlayer, removeWall.Point, removeWall.Direction);

            return ValueTask.CompletedTask;
        }
        public ValueTask HandleSpawnPieceReqeust(SpawnPieceReqeust message, Player player)
        {
            var spawnPiece = new SpawnPiece()
            {
                AccountId = player.AccountId,
                PieceId = message.PieceId,
                SpawnedPoint = new Point(message.SpawnedPointX, message.SpawnedPointY)
            };

            var wallGoPlayer = _wallGoBoard.GetPlayer(spawnPiece.AccountId);

            _wallGoBoard.TrySpawnPiece(wallGoPlayer, spawnPiece.PieceId, spawnPiece.SpawnedPoint);

            return ValueTask.CompletedTask;
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
            foreach(var member in _accountIdToPlayerMap.Values)
            {
                member.SessionRef.Post(new PlayerMessage<EndGame>(obj, member), Self);
            }
            Broadcast(Packet.MakePacket(WallGoServerEvent.EndGame, obj));
        }

        private void WallGoEventHandler_StartTurn(StartTurn obj)
        {
            Broadcast(Packet.MakePacket(WallGoServerEvent.StartTurn, obj));
        }

        protected override ValueTask HandleGameMessage(IActorMessage message, IActorRef sender)
        {
            return message switch
            {
                PlayerMessage<PlaceWallReqeust> request => HandlePlaceWallReqeust(request.Value, request.Player),
                PlayerMessage<MovePieceReqeust> request => HandleMovePieceReqeust(request.Value, request.Player),
                PlayerMessage<RemoveWallReqeust> request => HandleRemoveWallReqeust(request.Value, request.Player),
                PlayerMessage<SpawnPieceReqeust> request => HandleSpawnPieceReqeust(request.Value, request.Player),

                _ => ValueTask.CompletedTask
            };
        }
    }
}
