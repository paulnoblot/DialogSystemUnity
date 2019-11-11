using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node : ISerializable
{
    public int id = 0;
    public string name = "New Node";
    public List<Transition> nextNodes = new List<Transition>();

    public NodeEditorContent editor = new NodeEditorContent();

    public virtual void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref id);
        _serializer.Serialize(ref name);
        _serializer.Serialize(nextNodes);

        _serializer.Serialize(ref editor);
    }
}

public class NodeEditorContent : ISerializable
{
    public Vector2 position = Vector2.zero;
    public bool isSelected = false;

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref position);
    }
}
