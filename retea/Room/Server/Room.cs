using System;
using System.Collections.Generic;
using System.Text; 

namespace roomconnectivity
{
    public class Room : IRoom
    {
        private int id;
        private string name;
        private int PlayersNumber = 0;
        private List<User> Players;

        public Room(int id, string name)
        {
            this.id = id;
            this.name = name;
            Players = new List<User>();
        }

        public bool IsRoomFull()
        {
            return PlayersNumber >= 6;
        }

        public int GetPlayersNumber()
        {
            return PlayersNumber;
        }

        public int GetRoomId()
        {
            return id;
        }

        public string GetRoomName()
        {
            return name;
        }

        public bool AddUser(User user)
        {
            if (!user.GetStatus())
            {
                Players.Add(user);
                PlayersNumber++;
                user.SetConnectedToRoom(true, id);
                return true;
            }
            return false;
        }

        public bool KickUser(User user)
        {
            if (user.GetStatus() && user.GetRoomId() == id)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (user.GetUserName() == Players[i].GetUserName())
                    {
                        Players.RemoveAt(i);
                        user.SetConnectedToRoom(false, -1);
                        PlayersNumber--;
                        return true;
                    }
                }
            }
            return false;
        }

        public string[] GetPlayerNames()
        {
            StringBuilder sb = new StringBuilder();
            foreach (User player in Players)
            {
                sb.Append(player.GetUserName()).Append(", ");
            }
            string[] playerNames = sb.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            return playerNames;
        }
    }
}
