using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DialogBoard : ISerializable
{
    public string name = "";

    int idNode = 0;
    public int rootNode = 0;
    public List<Node> nodes = new List<Node>();
    public State state = new State();
    Node enterNode;

    // model
    public Node node = new Node();

    // editor
    public DialogBoardEditorContent editor = new DialogBoardEditorContent();

    public void AddNode(Node n)
    {
        n.id = idNode;
        nodes.Add(n);
        idNode++;
    }

    public Node GetNode(int id)
    {
        return nodes.Where(x => x.id == id).Single();
    }

    public List<Node> GetNextNodes(Node node, bool conditionValidate = true)
    {
        List<Node> nextNodes = new List<Node>();
        foreach (int id in node.nextNodes.Where(x => !conditionValidate || x.Evaluate(state)).Select(x => x.idNode).ToList())
        {
            nextNodes.Add(GetNode(id));
        }
        return nextNodes;
    }

    public List<Node> GetNextNodes(int idNode, bool conditionValidate = true)
    {
        return GetNextNodes(GetNode(idNode), conditionValidate);
    }

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref state);
        _serializer.Serialize(ref node);

        _serializer.Serialize(ref idNode);
        _serializer.Serialize(nodes);
    }
}

public class DialogBoardEditorContent : ISerializable
{
    public Vector2 offset;

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref offset);
    }
}