using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogManager
{
    static DialogManager singleton;

    FileInfo[] dialogFiles;

    public static DialogManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = new DialogManager();
            }

            return singleton;
        }
    }

    private DialogManager()
    {
        LoadDatabase();
    }

    public DialogBoard GetDialog(string filename)
    {
        filename += ".dialog";
        foreach (FileInfo file in dialogFiles)
        {
            if (file.Name == filename)
            {
                DialogBoard dialog;
                Serialization.Serializer.ReadFromFile<DialogBoard>(file.FullName, out dialog);
                return dialog;
            }
        }
        return null;
    }

    public void LoadDatabase()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogs");
        dialogFiles = GetAllFiles(path, "*.dialog");
    }

    public static FileInfo[] GetAllFiles(string path, string filepattern = "*")
    {
        List<FileInfo> fileInfos = new List<FileInfo>();

        if (Directory.Exists(path))
        {
            string[] filePaths = Directory.GetFiles(path, filepattern);
            foreach (string filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    fileInfos.Add(new FileInfo(filePath));
                }
            }
        }
        else
        {
            throw new DirectoryNotFoundException("Directory doesn't exists.");
        }


        return fileInfos.ToArray();
    }
}
