using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using Serialization;

public class BoardCreatorWindow : EditorWindow
{
    [MenuItem("Window/DialogBoard/CreateDialog", false, 0)]
    public static void CreateDialog()
    {
        CreateDialogAsset(typeof(NpcState), typeof(NpcNode));
    }

    [MenuItem("Window/DialogBoard/OpenWindow", false, 0)]
    public static void CreateWindow() 
    {
        BoardCreatorWindow window = CreateInstance(typeof(BoardCreatorWindow)) as BoardCreatorWindow;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Inherited classes of State :");

        foreach (Type t in typeof(State).Assembly.GetTypes())
        {
            if (t.IsSubclassOf(typeof(State)))
            {
                if (GUILayout.Button(t.ToString()))
                {
                    CreateDialogAsset(t);
                }
            }
        }

        if (GUILayout.Button("Create Dialog Board"))
        {
            CreateDialogAsset(typeof(NpcState), typeof(NpcNode));
        }
    }

    public static void CreateDialogAsset(Type stateType)
    {
        DialogBoard dialog = new DialogBoard();
        dialog.state = (State)Activator.CreateInstance(stateType);

        string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

        string path = "";
        string fileName = "";
        int index = 0;

        do // get an available filename
        {
            fileName = "New Dialog" + (index > 0 ? " (" + index + ")" : "");
            path = Path.Combine(folderPath, fileName + ".dialog");
            index++;
        }
        while (File.Exists(path));

        // Create the file and write a new quest in it
        Serializer.WriteToFile(path, dialog);
        AssetDatabase.Refresh();
    }

    public static void CreateDialogAsset(Type stateType, Type nodeType)
    {
        DialogBoard dialog = new DialogBoard();
        dialog.state = (State)Activator.CreateInstance(stateType);
        dialog.node = (Node)Activator.CreateInstance(nodeType);

        string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

        string path = "";
        string fileName = "";
        int index = 0;

        do // get an available filename
        {
            fileName = "New Dialog" + (index > 0 ? " (" + index + ")" : "");
            path = Path.Combine(folderPath, fileName + ".dialog");
            index++;
        }
        while (File.Exists(path));

        // Create the file and write a new quest in it
        Serializer.WriteToFile(path, dialog);
        AssetDatabase.Refresh();
    }
}
