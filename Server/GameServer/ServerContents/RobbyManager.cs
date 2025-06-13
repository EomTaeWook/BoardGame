using BG.GameServer.Network;
using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BG.GameServer.ServerContents
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class RobbyManager() : ISessionComponent
    {
        private readonly ConcurrentDictionary<string, Player> _players = new();
        private readonly ConcurrentDictionary<int, RoomBase> _rooms = new();

        private readonly ConcurrentDictionary<int, RoomBase> _privateRooms = new();

        private UniqueSet<int> _roomNumbers = new();

        public List<RoomBase> GetRooms(int page, int pageSize)
        {
            return [.. _rooms.Values.Skip(page * pageSize).Take(pageSize)];
        }
        public bool TryAddPlayer(Player player)
        {
            return _players.TryAdd(player.AccountId, player);
        }
        public bool TryRemovePlayer(Player player)
        {
            return _players.Remove(player.AccountId, out Player _);
        }
        

        private int GeneratorRoomNumber()
        {
            lock(_roomNumbers.SyncRoot)
            {
                for (int i = 0; i < 10; ++i)
                {
                    var roomNumber = Random.Shared.Next(1000, 99999);

                    if (_roomNumbers.Contains(roomNumber) == false)
                    {
                        if(_roomNumbers.Add(roomNumber) == true)
                        {
                            return roomNumber;
                        }
                    }
                }
            }
            return -1;
        }
        public bool TryCreatePrivateGameRoom(GameType gameType, out RoomBase room)
        {
            room = null;
            if (gameType == GameType.Max)
            {
                return false;
            }

            var roomNumber = GeneratorRoomNumber();

            if(roomNumber == -1)
            {
                return false;
            }

            if (_privateRooms.TryGetValue(roomNumber, out RoomBase roomBase) == true)
            {
                return false;
            }

            if (gameType == GameType.WallGo)
            {
                room = new WallGoRoom(roomNumber);
            }

            _privateRooms[roomNumber] = room;

            return true;
        }
        public bool TryCreateGameRoom(GameType gameType, out RoomBase room)
        {
            room = null;
            if (gameType == GameType.Max)
            {
                return false;
            }

            var roomNumber = GeneratorRoomNumber();

            if (roomNumber == -1)
            {
                return false;
            }

            if (_rooms.TryGetValue(roomNumber, out RoomBase roomBase) == true)
            {
                return false;
            }

            if (gameType == GameType.WallGo)
            {
                room = new WallGoRoom(roomNumber);
            }

            _rooms[roomNumber] = room;

            return true;
        }

        public bool TryGetGameRoom(int roomNumber, RoomMode roomMode, out RoomBase room)
        {
            if(roomMode == RoomMode.Public)
            {
                return _rooms.TryGetValue(roomNumber, out room);
            }
            else if(roomMode == RoomMode.Private)
            {
                return _privateRooms.TryGetValue(roomNumber, out room);
            }

            room = null;
            return false;
        }

        public bool RemoveRoom(RoomMode roomMode, int roomNumber)
        {
            if (roomMode == RoomMode.Public)
            {
                _rooms.TryRemove(roomNumber, out _);
            }
            else if (roomMode == RoomMode.Private)
            {
                _privateRooms.TryRemove(roomNumber, out _);
            }
            _roomNumbers.Remove(roomNumber);
            return true;
        }

        void ISessionComponent.SetSession(ISession session)
        {
        }

        void ISessionComponent.Dispose()
        {
        }
    }
}
