using Assets.Scripts.GameContents;
using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Network;
using BG.GameServer.ServerContents.EventHandler;
using Protocol.GSAndClient;

namespace BG.GameServer.ServerContents
{
    internal class WallGoRoom : RoomBase
    {
        private WallGoBoard _wallGoBoard;
        private WallGoEventHandler _wallGoEventHandler;
        public WallGoRoom(long roomNumber, IServiceProvider serviceProvider) : base(roomNumber, 4)
        {
            _wallGoEventHandler = new WallGoEventHandler();
            _wallGoBoard = new WallGoBoard(_wallGoEventHandler);

            RegisterEventHandlers();
        }

        public void Start()
        {
            _wallGoBoard.StartGame();
        }
        public void Dispose()
        {
            _wallGoEventHandler.StartTurn -= WallGoEventHandler_StartTurn;
            _wallGoEventHandler.EndGame -= WallGoEventHandler_EndGame;
            _wallGoEventHandler.StartGame -= WallGoEventHandler_StartGame;
            _wallGoEventHandler.ChangeState -= WallGoEventHandler_ChangeState;
            _wallGoEventHandler.SpawnPiece -= WallGoEventHandler_SpawnPiece;
            _wallGoEventHandler.MovePiece -= WallGoEventHandler_MovePiece;
            _wallGoEventHandler.PlaceWall -= WallGoEventHandler_PlaceWall;
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
        }
        public void MovePieceReqeust(IPlayer wallGoPlayer, MovePiece movePiece)
        {
            _wallGoBoard.MovePiece(wallGoPlayer, movePiece.PieceId, movePiece.Dest);
        }
        public void PlacWallReqeust(IPlayer wallGoPlayer, PlaceWall placeWall)
        {
            _wallGoBoard.TryPlaceWall(wallGoPlayer, placeWall.Direction);
        }
        public void SpawnPieceReqeust(IPlayer wallGoPlayer, SpawnPiece spawnPiece)
        {
            _wallGoBoard.TrySpawnPiece(wallGoPlayer, spawnPiece.PieceId, spawnPiece.SpawnedPoint);
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
