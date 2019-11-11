using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcState : State
{
    public Player player;
    public Npc npc;

    public bool isFirstMeet = true;

    public new void Serialize(Serializer _serializer)
    {

    }
}
