using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity Editor 下右键创建文本类文件
/// </summary>
public class CreateFileEditor : Editor
{
    [MenuItem("Assets/Create/Readme")]
    static void CreateTextFile()
    {
        CreateFile("txt", "Readme");
    }

    /// <summary>
    /// 创建文件类的文件
    /// </summary>
    /// <param name="fileEx"></param>
    static void CreateFile(string fileEx, string fileName)
    {
        //获取当前所选择的目录（相对于Assets的路径）
        var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var path = Application.dataPath.Replace("Assets", "") + "/";
        var newFileName = "new " + fileName + "." + fileEx;
        var newFilePath = selectPath + "/" + newFileName;
        var fullPath = path + newFilePath;

        //简单的重名处理
        if (File.Exists(fullPath))
        {
            var newName = "new_" + fileEx + "-" + UnityEngine.Random.Range(0, 100) + "." + fileEx;
            newFilePath = selectPath + "/" + newName;
            fullPath = fullPath.Replace(newFileName, newName);
        }

        //如果是空白文件，编码并没有设成UTF-8
        File.WriteAllText(fullPath, "-- test", Encoding.UTF8);

        AssetDatabase.Refresh();

        //选中新创建的文件
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