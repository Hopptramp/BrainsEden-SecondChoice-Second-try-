using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum BlockType
{
    Default,
    Teleport,
    Moving,
    Falling
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelID;
    public List<BlockData> storedBlocks;
}

    [System.Serializable]
public class Level : MonoBehaviour
{
    [SerializeField] List<BlockData> storedBlocks;
    static public int levelID;
    static LevelData asset;

    public void CollectBlocks()
    {
        if (storedBlocks == null)
            storedBlocks = new List<BlockData>();
        else
            storedBlocks.Clear();

        GetComponentsInChildren(storedBlocks);
    }

    [MenuItem("Assets/Create/My Scriptable Object")]
    public static void CreateMyAsset()
    {
        asset = ScriptableObject.CreateInstance<LevelData>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + "Level-" + levelID + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    public void SaveAsScriptableObject()
    {
        CreateMyAsset();
        asset.levelID = levelID;
        asset.storedBlocks = storedBlocks;
    }
}

[CustomEditor(typeof(Level))]
public class LevelCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Level level = (Level)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Collect Blocks"))
        {
            level.CollectBlocks();
        }

        if (GUILayout.Button("Save Scriptable Object"))
        {
            level.SaveAsScriptableObject();
        }


        //   base.OnInspectorGUI();
    }
}

public class LevelManager : MonoBehaviour
{
    public List<Level> unsavedLevels;
    public List<LevelData> storedLevels;

	// Use this for initialization
	void Start () {
	
	}
	

    public void CreateNewLevel()
    {
        GameObject levelObject = new GameObject("Level " + unsavedLevels.Count);
        Level level = levelObject.AddComponent<Level>();
        unsavedLevels.Add(level);
        levelObject.transform.SetParent(transform);
    }
}

[CustomEditor(typeof(LevelManager))]
public class LevelManagerCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        LevelManager level = (LevelManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Create new level"))
        {
            level.CreateNewLevel();
        }


       // base.OnInspectorGUI();
    }
}
