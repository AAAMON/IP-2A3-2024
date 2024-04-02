// Decompiled by DJ v3.12.12.101 Copyright 2016 Atanas Neshkov  Date: 01/04/2024 20:31:54
// Home Page: http://www.neshkov.com/dj.html - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Graveyard.java

package org.example;

import java.util.Vector;

// Referenced classes of package org.example:
//            IFaction

public class Graveyard
    implements IFaction
{

    public Graveyard()
    {
    }

    public Vector generals;
    public Integer troops[];
    public Boolean can_revive_generals;
    public Vector myFaction;
    public Vector myGeneral;
}