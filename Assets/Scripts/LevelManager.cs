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
    public GameObject block;
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelDataScriptable : ScriptableObject
{
    public int levelID;
    public List<StoredBlockData> storedBlocks;
}

[System.Serializable]
public class LevelDataActive : MonoBehaviour
{
    public List<StoredBlockData> storedBlocks;   
    public GameObject blockHolder;


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
            storedBlock.block = data.gameObject;
            storedBlocks.Add(storedBlock);
        }
    }

    [MenuItem("Assets/Create/My Scriptable Object")]
    public static void CreateMyAsset(int _id, List<StoredBlockData> _stored)
    {
        LevelDataScriptable asset;
       
        asset = ScriptableObject.CreateInstance<LevelDataScriptable>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + "Level-" + _id + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
        asset.levelID = _id;
        asset.storedBlocks = _stored;
    }

    public void SaveAsScriptableObject(int _id)
    {
        List<StoredBlockData> temp = new List<StoredBlockData>();
        for(int i = 0; i < storedBlocks.Count; ++i)
        {
            temp.Add(storedBlocks[i]);
        }
           //storedBlocks;
        CreateMyAsset(_id, temp);

    }
}

public class LevelManager : GameActors
{
    public LevelDataActive unsavedLevel;
    public List<LevelDataScriptable> storedLevels;
    public LevelDataScriptable currentSavedLevel;
    [SerializeField] private int levelID = 0;
    [SerializeField] private LevelDataActive currentLoadedLevel;

    [SerializeField] private GameObject defaultCube;

    protected override void OnPlayStart(RotationData _rotationData)
    {
        
    }

    public void SaveAsScriptableObject()
    {
        unsavedLevel.SaveAsScriptableObject(storedLevels.Count);
    }

    public StoredBlockData GetBlockByID(int _id)
    {
        return currentLoadedLevel.storedBlocks[_id];
    }

    public void SwitchLevels(LevelDataScriptable _newlevel)
    {
        if(currentLoadedLevel != null)
            Destroy(currentLoadedLevel);
        GenerateLevelFromLevelData(_newlevel);
    }

    public void SwitchLevels(int _newlevel)
    {
        if (currentLoadedLevel != null)
            RemoveLevel(currentLoadedLevel);
        GenerateLevelFromLevelData(storedLevels[_newlevel]);
    }

    void RemoveLevel(LevelDataActive _level)
    {
        foreach(StoredBlockData stored in _level.storedBlocks)
        {
            DestroyImmediate(stored.block);
        }
        DestroyImmediate(_level.blockHolder);
    }

    public void GenerateLevelFromLevelData(LevelDataScriptable _levelData)
    {
        GameObject temp = new GameObject("LoadedLevel" + currentSavedLevel.levelID.ToString());
        temp.transform.SetParent(transform);
        currentLoadedLevel = temp.AddComponent<LevelDataActive>();
        currentLoadedLevel.storedBlocks = _levelData.storedBlocks;
        currentLoadedLevel.blockHolder = temp;

        foreach (StoredBlockData storedData in _levelData.storedBlocks)
        {
            GameObject blockObject = Instantiate(defaultCube, currentLoadedLevel.transform) as GameObject;
            BlockData block = blockObject.GetComponent<BlockData>();
            block.level = currentLoadedLevel;
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
        unsavedLevel = levelObject.AddComponent<LevelDataActive>();
        levelObject.transform.SetParent(transform);
    }
}

[CustomEditor(typeof(LevelDataActive))]
public class LevelCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        LevelDataActive level = (LevelDataActive)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Collect Blocks"))
        {
            level.CollectBlocks();
        }

        //if (GUILayout.Button("Save Scriptable Object"))
        //{
        //    level.SaveAsScriptableObject();
        //}


        //   base.OnInspectorGUI();
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
            level.GenerateLevelFromLevelData(level.currentSavedLevel);
        }

        if (GUILayout.Button("Save Scriptable Object"))
        {
            level.SaveAsScriptableObject();
        }


        // base.OnInspectorGUI();
    }
}
