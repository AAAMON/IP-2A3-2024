using System;
using System.Collections.Generic;

namespace roomconnectivity
{
    public class RoomManager
    {
        private int id = 0;
        private List<IRoom> rooms = new List<IRoom>();

        public bool CreateRoom(string name)
        {
            if (GetRoomByName(name) == null)
            {
                Room newRoom = new Room(++id, name);
                rooms.Add(newRoom);
                return true;
            }
            return false;
        }
        public bool DeleteRoom(string name)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                IRoom room = rooms[i];
                if (room.GetRoomName() == name && room.GetPlayersNumber() == 0)
                {
                    rooms.RemoveAt(i);

                    return true;
                }
            }
            return false;
        }

        public List<IRoom> GetAvailableRooms()
        {
            List<IRoom> availableRooms = new List<IRoom>();
            for (int i = 0; i < rooms.Count; i++)
            {
                IRoom room = rooms[i];
                if (room.IsRoomFull() == false)
                {
                    availableRooms.Add(room);
                }
            }
            return availableRooms;
        }
        public IRoom GetRoomByName(string name)
        {
            foreach (IRoom room in rooms)
            {
                if (room.GetRoomName() == name)
                    return room;
            }
            return null;
        }
        public IRoom GetRoomById(int id)
        {
            foreach (IRoom room in rooms)
            {
                if (room.GetRoomId() == id)
                    return room;
            }
            return null;
        }
        public void ConnectToRoom(string name, User user)
        {
            IRoom room = GetRoomByName(name);
            room.AddUser(user);
        }
        public void ConnectToRoom(int id, User user)
        {
            IRoom room = GetRoomById(id);
            room.AddUser(user);
        }

    }
}
