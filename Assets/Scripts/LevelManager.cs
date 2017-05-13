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

[System.Serializable]
public struct StoredBlockData
{
    public int ID;
    public Vector3 localPosition;
    public BlockType type;
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelID;
    public List<StoredBlockData> storedBlocks;
}

    [System.Serializable]
public class Level : MonoBehaviour
{
    public List<StoredBlockData> storedBlocks;
    static public int levelID;
    static LevelData asset;

    /// <summary>
    /// Find all references to block data in children
    /// </summary>
    public void CollectBlocks()
    {
        if (storedBlocks == null)
            storedBlocks = new List<StoredBlockData>();
        else
            storedBlocks.Clear();

        BlockData[] datas = GetComponentsInChildren<BlockData>();

        int temp = -1;
        foreach(BlockData data in datas)
        {
            StoredBlockData storedBlock = new StoredBlockData();
            storedBlock.ID = ++temp;
            storedBlock.localPosition = data.transform.localPosition;
            storedBlock.type = data.blockType;
            storedBlocks.Add(storedBlock);
        }

        //GetComponentsInChildren(storedBlocks);
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
    public Level unsavedLevel;
    public List<LevelData> storedLevels;
    public LevelData activeLevel;
    private Transform levelParent;

    [SerializeField] private GameObject defaultCube;


    public void GenerateLevelFromLevelData()
    {
        levelParent = new GameObject("LoadedLevel" + activeLevel.levelID.ToString()).transform;
        levelParent.SetParent(transform);

        foreach (StoredBlockData storedData in activeLevel.storedBlocks)
        {
            GameObject blockObject = Instantiate(defaultCube, levelParent) as GameObject;
            BlockData block = blockObject.GetComponent<BlockData>();
            block.localPosition = storedData.localPosition;
            blockObject.transform.localPosition = block.localPosition;
            block.blockType = storedData.type;
            block.ID = storedData.ID;
            block.Initialise();
        }
    }
	
    public void CreateNewLevel()
    {
        GameObject levelObject = new GameObject("Unsaved Level " + storedLevels.Count);
        unsavedLevel = levelObject.AddComponent<Level>();
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

        if (GUILayout.Button("Create Empty Level"))
        {
            level.CreateNewLevel();
        }

        if (GUILayout.Button("Create From Scriptable Object"))
        {
            level.GenerateLevelFromLevelData();
        }


        // base.OnInspectorGUI();
    }
}
