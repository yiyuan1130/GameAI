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
}