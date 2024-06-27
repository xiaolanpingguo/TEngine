using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class CreateScriptableObject : CustomObjectSelectionWindow<CreateScriptableObject, MonoScript>
{
    protected override string ObjectSearchFilterString => "t:script";
    protected override string[] ObjectSearchFolders => new string[] { "Assets" };

    protected override bool DelayClosingCall => false;

    protected override string SelectButtonText => "Create Scriptable Object from Selection...";

    protected override void InitializeWindow(params object[] extraParams)
    {
        Debug.Log("CreateScriptableObject:InitializeWindow");
    }

    protected override bool IsValidObject(MonoScript script)
    {
        if (script == null || string.IsNullOrEmpty(script.text))
        {
            return false;
        }
        Type type = script.GetClass();
        if (type == null || !type.IsSubclassOf(typeof(ScriptableObject)))
        {
            return false;
        }

        return true;
    }

    public bool CreateScriptableAsset(MonoScript script, string destinationFolder)
    {
        if (!IsValidObject(script))
        {
            Debug.Log("CreateScriptableObject:Invalid script to create from, nothing done.");
            return false;
        }
        Type type = script.GetClass();

        string path = destinationFolder ?? "Assets";

        string filename = script.name;

        //use the filename from the CreateAsset attribute if there
        CreateAssetMenuAttribute createAssetMenuAttribute = GetCreateAssetMenuAttribute(type);
        if (createAssetMenuAttribute != null)
        {
            if (!string.IsNullOrEmpty(createAssetMenuAttribute.fileName))
            {
                filename = createAssetMenuAttribute.fileName;
            }
        }
        filename += ".asset";
        string fullname = EditorUtility.SaveFilePanel("Choose where to create the asset", path, filename, "asset");
        if (string.IsNullOrEmpty(fullname))
        {
            Debug.Log($"CreateScriptableObject:Scriptable Asset cancelled.");
            return false;
        }
        //strip to be from project folder
        int leftAsset = fullname.IndexOf("Assets/");
        if (leftAsset < 0)
        {
            Debug.Log($"CreateScriptableObject:Scriptable invalid path, cannot find the Assets folder in it: {fullname}");
            return false;
        }
        fullname = fullname.Substring(leftAsset, fullname.Length - leftAsset);

        ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(scriptableObject, fullname);
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullname);
        Debug.Log($"CreateScriptableObject:Scriptable Asset created in: {fullname}");
        return true;
    }

    protected UnityEngine.CreateAssetMenuAttribute GetCreateAssetMenuAttribute(Type type)
    {
        System.Attribute[] attributes = System.Attribute.GetCustomAttributes(type);
        foreach (Attribute attribute in attributes)
        {
            UnityEngine.CreateAssetMenuAttribute att = attribute as UnityEngine.CreateAssetMenuAttribute;
            if (att != null)
            {
                return att;
            }
        }
        return null;
    }
}

//------------------------
public static class CreateScriptableObjectMenu
{
    [MenuItem("Assets/Create/Create Scriptable Object...", false, -1)]
    private static void MagnumCreateAssetWindow()
    {
        string destination = null;
        if (Selection.activeObject != null)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(path))
            {
                destination = path;
            }
        }

        CreateScriptableObject createSOWindow = CreateScriptableObject.Get(-1, "Create Scriptable Object", null, null);
        createSOWindow.ShowModalUtility();
        if (createSOWindow.SelectedObject == null)
        {
            Debug.Log("CreateScriptableObject:Closed, cancelled, nothing done.");
            return;
        }

        createSOWindow.CreateScriptableAsset(createSOWindow.SelectedObject, destination);
    }
}
