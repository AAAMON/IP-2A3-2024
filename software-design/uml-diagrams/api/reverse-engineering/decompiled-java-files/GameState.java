// Decompiled by DJ v3.12.12.101 Copyright 2016 Atanas Neshkov  Date: 01/04/2024 20:31:54
// Home Page: http://www.neshkov.com/dj.html - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   GameState.java

package org.example;

import java.util.Vector;

// Referenced classes of package org.example:
//            Map, Spice_deck, Treachery_deck, Pair

public class GameState
{

    public GameState()
    {
    }

    private Vector players;
    private Map map;
    public Integer turn_count;
    public Spice_deck spice_deck;
    public Treachery_deck treachery_deck;
    public Pair aliances;
    public Vector myMap;
    public Vector mySpice_deck;
    public Vector myPlayer;
    public Vector myTreachery_deck;
}