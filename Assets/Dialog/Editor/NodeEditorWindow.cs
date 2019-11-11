using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class NodeEditorWindow : EditorWindow
{
    class LogicEditor
    {
        public int idSelected = 0;
    }

    class TransitionEditorInfo
    {
        public List<LogicEditor> logics = new List<LogicEditor>();
    }


    DialogSystemWindow board;
    Node target;

    List<TransitionEditorInfo> editorInfo;
    int selectedIdAddTransition = 0;

    public static void CreateWindow(DialogSystemWindow board, Node node)
    {
        NodeEditorWindow window = GetWindow(typeof(NodeEditorWindow)) as NodeEditorWindow;
        window.board = board;
        window.target = node;
        window.editorInfo = new List<TransitionEditorInfo>();
        foreach (Transition t in node.nextNodes)
        {
            TransitionEditorInfo te = new TransitionEditorInfo();
            foreach (Logic l in t.logics)
            {
                te.logics.Add(new LogicEditor());
            }
            window.editorInfo.Add(te);
        }
        window.Show();
    }

    private void OnGUI()
    {
        DrawNodeInfo();

        EditorGUILayout.LabelField("Transitions", EditorStyles.boldLabel);
        DisplayTransitions();
        DrawAddTransition();
    }
    
    void DrawNodeInfo()
    {
        FieldInfo[] fieldInfos = target.GetType().GetFields();

        target.name = EditorGUILayout.TextField("name : ", target.name);

        if (fieldInfos.Length > 0)
            EditorGUILayout.LabelField("Attributs", EditorStyles.boldLabel);

        foreach (FieldInfo i in fieldInfos)
        {
            if (i.IsPublic && i.DeclaringType == target.GetType())
            {
                EditorGUILayout.LabelField(i.Name + " : " + i.FieldType.ToString());
                if (i.FieldType.IsEnum)
                {
                    i.SetValue(target, EditorGUILayout.Popup((int)i.GetValue(target), Enum.GetNames(i.FieldType)));
                }
                else
                {
                    switch (i.FieldType.ToString().Split('.').Last().ToLower())
                    {
                        case "int32":
                            i.SetValue(target, EditorGUILayout.IntField((int)i.GetValue(target)));
                            break;
                        case "string":
                            i.SetValue(target, EditorGUILayout.TextField((string)i.GetValue(target)));
                            break;
                        case "double":
                            i.SetValue(target, EditorGUILayout.DoubleField((double)i.GetValue(target)));
                            break;
                        case "float":
                            i.SetValue(target, EditorGUILayout.FloatField((float)i.GetValue(target)));
                            break;
                        case "bool":
                            i.SetValue(target, EditorGUILayout.Toggle((bool)i.GetValue(target)));
                            break;
                        default:
                            i.SetValue(target, EditorGUILayout.ObjectField((UnityEngine.Object)i.GetValue(target), i.FieldType));
                            break;
                    }
                }
            }
        }
    }

    void DisplayTransitions()
    {
        string[] options = new string[] { "all true", "at least one" };

        Transition t;
        for (int i = 0; i < target.nextNodes.Count; i++)
        {
            t = target.nextNodes[i];

            EditorGUILayout.LabelField(board.target.nodes.Where(x => x.id == t.idNode).Single().name);

            EditorGUILayout.BeginHorizontal();
            t.allTrue = EditorGUILayout.Popup("", t.allTrue ? 0 : 1, options) == 0 ? true : false;
            if (GUILayout.Button("-"))
                board.RemoveTransition(new Vector2Int(target.id, t.idNode));
            EditorGUILayout.EndHorizontal();

            for (int j = 0; j < t.logics.Count; j++)
            {
                DrawLogic(i, j);
            }

            if (GUILayout.Button("add logic"))
            {
                t.logics.Add(new Logic());
                editorInfo[i].logics.Add(new LogicEditor());
            }
        }
    }

    void DrawLogic(int transitionId, int logicId)
    {
        Logic logic = target.nextNodes[transitionId].logics[logicId];

        EditorGUILayout.BeginVertical();
        string tab = "";
        for (int i = 0; i < logic.logicInfos.Count; i++)
        {
            EditorGUILayout.LabelField(tab + logic.logicInfos[i].Name);
            tab += "  ";
        }
        Type t = logic.GetLastObjectType(board.target.state);

        Logic.Type type;
        if (CastToLogicType(t, out type))
        {
            DrawComparer(logic);
        }
        else
        {
            List<LogicInfo> options = GetlogicsInfos(t);
            EditorGUILayout.BeginHorizontal();
            editorInfo[transitionId].logics[logicId].idSelected = EditorGUILayout.Popup(editorInfo[transitionId].logics[logicId].idSelected, options.Select(x => x.Name).ToArray());
            if (GUILayout.Button("+"))
            {
                logic.logicInfos.Add(options[editorInfo[transitionId].logics[logicId].idSelected]);
                if (CastToLogicType(logic.GetLastObjectType(board.target.state), out type))
                {
                    InitLogic(logic);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    void DrawComparer(Logic logic)
    {
        EditorGUILayout.BeginHorizontal();

        logic.comparison = EditorGUILayout.Popup((int)logic.comparison, Logic.GetComparisonForType((Logic.Type)logic.type).Select(x => x.ToString()).ToArray());

        switch ((Logic.Type)logic.type)
        {
            case Logic.Type.Bool:
                logic.Comparer = EditorGUILayout.Toggle((bool)logic.Comparer);
                break;
            case Logic.Type.Enum:
                Type enumType = logic.GetLastObjectType(board.target.state);
                if (!enumType.IsEnum)
                    break;
                logic.Comparer = EditorGUILayout.Popup((int)logic.Comparer, Enum.GetNames(enumType));
                break;
            case Logic.Type.String:
                logic.Comparer = EditorGUILayout.TextField((string)logic.Comparer);
                break;
            case Logic.Type.Double:
                logic.Comparer = EditorGUILayout.DoubleField((double)logic.Comparer);
                break;
            case Logic.Type.Float:
                logic.Comparer = EditorGUILayout.FloatField((float)logic.Comparer);
                break;
            case Logic.Type.Int:
                logic.Comparer = EditorGUILayout.IntField((int)logic.Comparer);
                break;
            default:
                break;
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawAddTransition()
    {
        EditorGUILayout.BeginHorizontal();

        List<Node> options = new List<Node>();
        foreach (Node n in board.target.nodes)
            if (target.nextNodes.Where(x => x.idNode == n.id).Count() == 0)
                options.Add(n);

        if (options.Count == 0)
            return;

        selectedIdAddTransition = EditorGUILayout.Popup("", selectedIdAddTransition, options.Select(x => x.name).ToArray());

        if (GUILayout.Button("+"))
            board.CreateTransition(new Vector2Int(target.id, options[selectedIdAddTransition].id));

        EditorGUILayout.EndHorizontal();
    }

    bool CastToLogicType(Type t, out Logic.Type type)
    {
        type = Logic.Type.Bool;

        if (t.IsEnum)
        {
            type = Logic.Type.Enum;
            return true;
        }

        switch (t.ToString().Split('.').Last().ToLower())
        {
            case "bool":
                type = Logic.Type.Bool;
                return true;
            case "string":
                type = Logic.Type.String;
                return true;
            case "double":
                type = Logic.Type.Double;
                return true;
            case "float":
                type = Logic.Type.Float;
                return true;
            case "int32":
                type = Logic.Type.Int;
                return true;
            default:
                return false;
        }
    }

    List<LogicInfo> GetlogicsInfos(Type t)
    {
        List<LogicInfo> infos = new List<LogicInfo>();
        foreach (FieldInfo i in GetFieldInfos(t))
            infos.Add(new LogicInfo(i, t));
        foreach (PropertyInfo i in GetPropertyInfos(t))
            infos.Add(new LogicInfo(i, t));
        foreach (MethodInfo i in GetMethodInfos(t))
            infos.Add(new LogicInfo(i, t));
        return infos;
    }

    List<FieldInfo> GetFieldInfos(Type t)
    {
        return t.GetFields().Where(x => x.DeclaringType == t && x.IsPublic).ToList();
    }

    List<PropertyInfo> GetPropertyInfos(Type t)
    {
        return t.GetProperties().Where(x => x.DeclaringType == t && x.GetGetMethod() != null && x.GetGetMethod().IsPublic).ToList();
    }

    List<MethodInfo> GetMethodInfos(Type t)
    {
        return t.GetMethods().Where(x => x.DeclaringType == t && x.IsPublic && x.ReturnType != typeof(void)).ToList();
    }

    void InitLogic(Logic logic)
    {
        Logic.Type type;
        
        if (!CastToLogicType(logic.GetLastObjectType(board.target.state), out type))
            return;

        logic.type = (int)type;
        logic.comparison = 0;

        switch (type)
        {
            case Logic.Type.Bool:
                logic.Comparer = false;
                break;
            case Logic.Type.Enum:
            case Logic.Type.Double:
            case Logic.Type.Float:
            case Logic.Type.Int:
                logic.Comparer = 0;
                break;
            case Logic.Type.String:
                logic.Comparer = "";
                break;
        }
    }
}
