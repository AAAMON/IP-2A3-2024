// Decompiled by DJ v3.12.12.101 Copyright 2016 Atanas Neshkov  Date: 01/04/2024 20:31:54
// Home Page: http://www.neshkov.com/dj.html - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Faction.java

package org.example;

import java.util.Map;
import java.util.Vector;

// Referenced classes of package org.example:
//            Player, Graveyard

public class Faction extends Player
{

    public Faction()
    {
    }

    public Integer troops[];
    public Graveyard graveyard;
    public Vector treachery_cards;
    public Vector traitors;
    public Vector generals;
    public Map troop_pozition;
    public Vector myPlayer;
    public Vector myGraveyard;
    public Vector myGeneral;
    public Vector myTreachery_cards;
}