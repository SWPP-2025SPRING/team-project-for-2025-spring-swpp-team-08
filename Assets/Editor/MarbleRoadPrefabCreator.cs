using UnityEngine;
using UnityEditor;
using System.IO;

public class MarbleRoadPrefabCreator : EditorWindow
{
    [MenuItem("Tools/Create Editable Prefabs from MarbleRoad")]
    public static void CreatePrefabs()
    {
        string sourceFolder = "Assets/DownloadedAssets/MarbleRoad";
        string outputFolder = "Assets/ProcessedPrefabs/MarbleRoad";

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { sourceFolder });
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (model == null) continue;

            // Instantiate for editing
            GameObject instance = GameObject.Instantiate(model);
            instance.transform.localScale = Vector3.one * 4f;

            foreach (MeshFilter mf in instance.GetComponentsInChildren<MeshFilter>())
            {
                MeshCollider mc = mf.GetComponent<MeshCollider>();
                if (mc == null) mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
            }

            // Save as new prefab
            string fileName = Path.GetFileNameWithoutExtension(path);
            string newPath = $"{outputFolder}/{fileName}_Processed.prefab";
            PrefabUtility.SaveAsPrefabAsset(instance, newPath);
            GameObject.DestroyImmediate(instance);
            count++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Created {count} editable prefabs in {outputFolder}.");
    }
}
