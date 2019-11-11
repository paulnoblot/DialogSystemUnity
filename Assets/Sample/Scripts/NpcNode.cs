using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcNode : Node
{
    public string choiceString = "";
    public string content = ""; 

    override
    public void Serialize(Serializer _serializer)
    {
        base.Serialize(_serializer);

        _serializer.Serialize(ref choiceString);
        _serializer.Serialize(ref content);
    }
}
