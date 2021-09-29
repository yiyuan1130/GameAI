using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity Editor ���Ҽ������ı����ļ�
/// </summary>
public class CreateFileEditor : Editor
{
    [MenuItem("Assets/Create/Readme")]
    static void CreateTextFile()
    {
        CreateFile("txt", "Readme");
    }

    /// <summary>
    /// �����ļ�����ļ�
    /// </summary>
    /// <param name="fileEx"></param>
    static void CreateFile(string fileEx, string fileName)
    {
        //��ȡ��ǰ��ѡ���Ŀ¼�������Assets��·����
        var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var path = Application.dataPath.Replace("Assets", "") + "/";
        var newFileName = "new " + fileName + "." + fileEx;
        var newFilePath = selectPath + "/" + newFileName;
        var fullPath = path + newFilePath;

        //�򵥵���������
        if (File.Exists(fullPath))
        {
            var newName = "new_" + fileEx + "-" + UnityEngine.Random.Range(0, 100) + "." + fileEx;
            newFilePath = selectPath + "/" + newName;
            fullPath = fullPath.Replace(newFileName, newName);
        }

        //����ǿհ��ļ������벢û�����UTF-8
        File.WriteAllText(fullPath, "-- test", Encoding.UTF8);

        AssetDatabase.Refresh();

        //ѡ���´������ļ�
        var asset = AssetDatabase.LoadAssetAtPath(newFilePath, typeof(Object));
        Selection.activeObject = asset;
    }

    //[MenuItem("Assets/Create/CommonFolder")]
    //static void CreateCommonFolder() {
    //    var assetPath = Application.dataPath;
    //    Debug.Log(assetPath);
    //    string commonFolderPath = assetPath + "NewFolder";
    //    Directory.CreateDirectory(assetPath);
    //    string materials = commonFolderPath + "/Materials";
    //    string prefabs = commonFolderPath + "/Prefabs";
    //    string scenes = commonFolderPath + "/Scenes";
    //    string scripts = commonFolderPath + "/Scripts";
    //    Directory.CreateDirectory(materials);
    //    Directory.CreateDirectory(prefabs);
    //    Directory.CreateDirectory(scenes);
    //    Directory.CreateDirectory(scripts);

    //    string readmePath = commonFolderPath + "/Readme.txt";
    //    File.WriteAllText(readmePath, "-- test", Encoding.UTF8);

    //    AssetDatabase.Refresh();
    //}
}