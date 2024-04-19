using System;
using System.Collections.Generic;


public class User
{
    private string name;
    private bool connectedToRoom = false;
    bool auth = false;
    int roomId;
    public User(string username)
    {
        this.name = username;
    }
    public void SetConnectedToRoom(bool connectedToRoom, int roomId)
    {
        this.roomId = roomId; this.connectedToRoom = connectedToRoom;
    }
    public void SetAuth(bool auth)
    {
        this.auth = auth;
    }
    public string GetUserName()
    {
        return name;
    }
    public bool GetStatus()
    {
        return connectedToRoom;
    }
    public bool GetAuth()
    {
        return auth;
    }
    public int GetRoomId()
    {
        return roomId;
    }
}
