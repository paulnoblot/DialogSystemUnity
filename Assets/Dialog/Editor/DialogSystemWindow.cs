using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogSystemWindow : EditorWindow
{
    [OnOpenAssetAttribute(2)]
    static bool OpenWindow(int instanceID, int line)
    {
        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        if (path.EndsWith(".dialog"))
        {
            DialogBoard board = null;
            Serializer.ReadFromFile(path, out board);
            board.name = EditorUtility.InstanceIDToObject(instanceID).name;
            CreateWindow(board, path);
        }
        else
            return false;
        return true;
    }



    public DialogBoard target;
    string path;

    float selectionBoxMinDiag = 2f;
    public bool nodeEditionMultiWindow = false;
    Vector2? selectionBoxStartPosition = null;
    Event ev;

    Vector2 nodeSize = new Vector2(120, 28);

    GUIStyle nodeStyle;
    Color rootNodeColor = new Color(1f, .6f, 0f, 1f);

    Color outlineNodeColor = new Color(0, .8f, 1f, 1f);

    public static void CreateWindow(DialogBoard target, string path)
    {
        DialogSystemWindow window = (DialogSystemWindow) CreateInstance(typeof(DialogSystemWindow));
        window.target = target;
        window.path = path;
        window.Show();

        // replace by an Init function ?
        window.nodeStyle = GUI.skin.button;
    }
        
    public void Save()
    {
        Serializer.WriteToFile(path, target);
        AssetDatabase.Refresh();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        ev = Event.current;

        MouseEvents();
                
        ShortcutEvents();

        MoveBoard();

        DrawBoxes();


        DrawMenuBar();

        DrawSeletionBox();
    }
        
    void DrawMenuBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(80), GUILayout.Width(this.position.width));

        if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton, GUILayout.Width(120))) { Save(); }

        GUILayout.FlexibleSpace();

        // Show in Project ?

        EditorGUILayout.EndHorizontal();
    }


    void EditNode(object obj)
    {
        NodeEditorWindow.CreateWindow(this, target.nodes.Where(x => x.id == (int) obj).Single());
    }

    #region MOUSE & SHORTCUTS   
    void ShortcutEvents()
    {
        if (ev.isKey)
        {
            switch (ev.keyCode)
            {
                case KeyCode.Delete:
                    DeleteSelectedNodes();
                    break;
                default:
                    break;
            }
        }
    }

    void MouseEvents()
    {
        if (ev.isMouse)
        {
            if (ev.button == 0)
            {
                MouseLeft();
            }
            if (ev.button == 1)
            {
                MouseRightClick();
            }
        }
    }
        
    void MouseLeft()
    {
        if (!ev.control)
        {
            if (ev.clickCount == 2)
            {
                MouseLeftDoubleClick();
                return;
            }

            switch (ev.GetTypeForControl(0))
            {
                case EventType.MouseDown:
                    MouseLeftDown();
                    break;
                case EventType.MouseDrag:
                    MouseLeftDrag();
                    break;
                case EventType.MouseUp:
                    MouseLeftUp();
                    break;
                default:
                    break;
            }
        }
    }

    void MouseLeftDoubleClick()
    {
        foreach (Node n in target.nodes)
        {
            if (Contains(GetRect(n), ev.mousePosition))
            {
                EditNode(n.id as object);
                return;
            }
        }
    }

    void MouseLeftDown()
    {
        foreach (Node n in target.nodes)
        {
            if (Contains(GetRect(n), ev.mousePosition))
            {
                if (SelectedNodes().Count == 0)
                {
                    n.editor.isSelected = true;
                }
                else if (!n.editor.isSelected)
                {
                    if (!ev.isKey || ev.keyCode != KeyCode.LeftShift)
                        DeselectAllNodes();
                    n.editor.isSelected = true;
                }
                return;
            }
        }

        if (ev.keyCode != KeyCode.LeftShift)
            DeselectAllNodes();
        selectionBoxStartPosition = ev.mousePosition;
    }

    void MouseLeftDrag()
    {
        List<Node> selectedNodes = SelectedNodes();
        if (selectedNodes.Count == 0)
        {
            if (ev.keyCode != KeyCode.LeftShift)
                DeselectAllNodes();
        }

        foreach (Node n in selectedNodes)
        {
            n.editor.position += ev.delta;
        }
    }

    void MouseLeftUp()
    {
        if (selectionBoxStartPosition != null)
        {
            GetSelectedBoxes(ToWellOrientedRect((Vector2)selectionBoxStartPosition, ev.mousePosition));
            selectionBoxStartPosition = null;
        }
    }

    void MouseRightClick()
    {
        foreach (Node n in target.nodes)
        {
            if (Contains(GetRect(n), ev.mousePosition))
            {
                DrawContextMenuOnBox(n.id);
                return;
            }
        }
        DrawContextMenu();
    }

    void MoveBoard()
    {
        if (ev.control && !ev.isScrollWheel)
            target.editor.offset += ev.delta;
    }
    #endregion

    void GetSelectedBoxes(Rect rect)
    {
        foreach (Node n in target.nodes)
            n.editor.isSelected = Intersect(rect, new Rect(AddBoardOffset(n.editor.position), nodeSize));
    }


    void DrawContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("CreateNode"), false, CreateNewNode);
        menu.ShowAsContext();
    }

    void CreateNewNode()
    {
        Node n = (Node)Activator.CreateInstance(target.node.GetType());
        n.editor.position = ev.mousePosition;
        target.AddNode(n);
    }
        
    void DrawContextMenuOnBox(int indexNode)
    {
        GenericMenu boxContextMenu = new GenericMenu();

        boxContextMenu.AddItem(new GUIContent("Edit Node"), false, EditNode, indexNode);

        boxContextMenu.AddSeparator("");
        boxContextMenu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, indexNode);
        boxContextMenu.ShowAsContext();
    }

    // obj as Vector2Int
    // x : id of the origin node
    // y : id of the destination node 
    public void CreateTransition(object obj)
    {
        Vector2Int v = (Vector2Int)obj;
        Node n = target.nodes.Where(x => x.id == v.x).Single();

        Transition t = new Transition();
        t.idNode = v.y;
        n.nextNodes.Add(t);
    }

    public void RemoveTransition(object obj)
    {
        Vector2Int v = (Vector2Int)obj;
        Node n = target.nodes.Where(x => x.id == v.x).Single();
        n.nextNodes.RemoveAll(x => x.idNode == v.y);
    }

    void DeleteSelectedNodes()
    {
        List<int> idNodeToRemove = new List<int>();
        foreach (Node n in target.nodes)
            if (n.editor.isSelected)
                idNodeToRemove.Add(n.id);
        foreach (int id in idNodeToRemove)
            DeleteNode(id);
    }

    void DeleteNode(object obj)
    {
        Node n = target.nodes.Where(x => x.id == (int)obj).Single();
        target.nodes.Remove(n);
    }

    void DrawOutline(Rect rect)
    {
        GUI.color = outlineNodeColor;
        Rect outlineRect = rect;
        outlineRect.position -= new Vector2(4, 4);
        outlineRect.size += new Vector2(8, 8);
        GUI.Box(outlineRect, "", nodeStyle);
        GUI.color = Color.white;
    }

    void DrawBoxes()
    {
        Node to;
        foreach (Node from in target.nodes)
        {
            for (int i = 0; i < from.nextNodes.Count; ++i)
            {
                to = target.nodes.Where(x => x.id == from.nextNodes[i].idNode).Single();
                DrawTransition(from, to);
            }
        }

        foreach (Node n in target.nodes)
        {
            GUI.color = n.editor.isSelected ? Color.yellow : Color.white;
            GUI.Box(GetRect(n), n.name, nodeStyle);
        }
        GUI.color = Color.white;
    }

    void DrawTransition(Node from, Node to)
    {
        Rect rectFrom = GetRect(from);
        Rect rectTo = GetRect(to);

        Vector2 begin = new Vector2(rectFrom.x + rectFrom.width, rectFrom.y + rectFrom.height / 2);
        Vector2 end = new Vector2(rectTo.x, rectTo.y + rectTo.height / 2);
        float xDistance = Mathf.Abs(begin.x - end.x);

        Handles.BeginGUI();
        Handles.DrawBezier(
            begin,
            end,
            begin + new Vector2(xDistance * 0.5f, 0),
            end - new Vector2(xDistance * 0.5f, 0),
            Color.black, null, 2);
        Handles.EndGUI();
    }

    void DrawSeletionBox()
    {
        if (selectionBoxStartPosition == null)
            return;

        int width = (int)(ev.mousePosition - (Vector2)selectionBoxStartPosition).x;
        int height = (int)(ev.mousePosition - (Vector2)selectionBoxStartPosition).y;

        if (new Vector2(width, height).magnitude < selectionBoxMinDiag)
            return;

        GUI.color = new Color(0, .8f, 1f, .2f);

        GUI.Box(ToWellOrientedRect((Vector2)selectionBoxStartPosition, ev.mousePosition), "");

        GUI.color = Color.white;
    }
        
    List<Node> SelectedNodes()
    {
        return target.nodes.Where(x => x.editor.isSelected).ToList();
    }

    Vector2 AddBoardOffset(Vector2 v)
    {
        return v + target.editor.offset;
    }

    Rect GetRect(Node n)
    {
        return new Rect(n.editor.position + target.editor.offset, nodeSize);
    }

    void DeselectAllNodes()
    {
        foreach (Node n in target.nodes)
            n.editor.isSelected = false;
    }

    bool Contains(Rect rect, Vector2 vec)
    {
        return
            vec.x >= rect.x &&
            vec.x <= (rect.x + rect.width) &&
            vec.y >= rect.y &&
            vec.y <= (rect.y + rect.height);
    }

    bool Intersect(Rect a, Rect b)
    {
        return !(b.x > a.x + a.width
                || b.x + b.width < a.x
                || b.y > a.y + a.height
                || b.y + b.height < a.y);
    }

    Rect ToWellOrientedRect(Vector2 start, Vector2 end)
    {
        Vector2 a = new Vector2(
            Mathf.Min(start.x, end.x),
            Mathf.Min(start.y, end.y));

        Vector2 b = new Vector2(
            Mathf.Max(start.x, end.x),
            Mathf.Max(start.y, end.y));

        return new Rect(a, b - a);

    }

}
