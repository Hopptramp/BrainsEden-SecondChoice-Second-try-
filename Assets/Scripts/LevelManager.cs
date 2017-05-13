using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum BlockType
{
    Default,
    Teleport,
    Moving,
    Falling,
    Start,
    End
}

[System.Serializable]
public struct StoredBlockData
{
    public int ID;
    public Vector3 localPosition;
    public BlockType type;
    public GameObject block;
}

[System.Serializable]
public struct LevelCompletionData
{
    public bool hasCompleted;
    public float timeTaken;
    public int totalFlips;
    public int totalSteps;
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelDataScriptable : ScriptableObject
{
    public int levelID;
    public List<StoredBlockData> storedBlocks;

    public LevelCompletionData completionData;
}

[System.Serializable]
public class LevelDataActive : MonoBehaviour
{
    public List<StoredBlockData> storedBlocks;   
    public GameObject blockHolder;
    public LevelCompletionData completionData;
    public LevelDataScriptable scriptableObject;


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

        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = asset;

        // APPLY VARIABLES HERE
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
    [SerializeField] private int activeLevelID = 0;
    [SerializeField] private LevelDataActive currentLoadedLevel;

    [SerializeField] private GameObject defaultCube;

    private void Update()
    {

    }

    #region delegates

    protected override void PreRotationLogic(RotationData _rotationData)
    {
        base.PreRotationLogic(_rotationData);
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {

        base.PostRotationLogic(_rotationData, _isInit);
    }

    #endregion

    #region public API

    /// <summary>
    /// return block data via ID
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public StoredBlockData GetBlockByID(int _id)
    {
        return currentLoadedLevel.storedBlocks[_id];
    }

    /// <summary>
    /// apply score changes to level
    /// </summary>
    /// <param name="_data"></param>
    public void OnLevelComplete(LevelCompletionData _data)
    {
        currentLoadedLevel.completionData = _data;
    }

    #endregion

    #region Level creation/removal

    /// <summary>
    /// load level via scriptable object
    /// </summary>
    /// <param name="_newlevel"></param>
    public void SwitchLevels(LevelDataScriptable _newlevel)
    {
        activeLevelID = _newlevel.levelID;
        if (currentLoadedLevel != null)
            Destroy(currentLoadedLevel);
        GenerateLevelFromLevelData(_newlevel);
    }

    /// <summary>
    /// lead level via level ID
    /// </summary>
    /// <param name="_newlevel"></param>
    public void SwitchLevels(int _newlevel)
    {
        activeLevelID = _newlevel;
        if (currentLoadedLevel != null)
            RemoveLevel(currentLoadedLevel);
        GenerateLevelFromLevelData(storedLevels[_newlevel]);
    }

    /// <summary>
    /// load current level
    /// </summary>
    public void ReloadLevel()
    {
        if (currentLoadedLevel != null)
            RemoveLevel(currentLoadedLevel);
        GenerateLevelFromLevelData(storedLevels[activeLevelID]);
    }

    /// <summary>
    /// Remove the passed in level
    /// </summary>
    /// <param name="_level"></param>
    void RemoveLevel(LevelDataActive _level)
    {
        foreach(StoredBlockData stored in _level.storedBlocks)
        {
            DestroyImmediate(stored.block);
        }
        DestroyImmediate(_level.blockHolder);
    }

    /// <summary>
    /// Generate level from scriptable object
    /// </summary>
    /// <param name="_levelData"></param>
    public void GenerateLevelFromLevelData(LevelDataScriptable _levelData)
    {
        GameObject temp = new GameObject("LoadedLevel" + _levelData.levelID.ToString());
        temp.transform.SetParent(transform);
        currentLoadedLevel = temp.AddComponent<LevelDataActive>();
        currentLoadedLevel.storedBlocks = _levelData.storedBlocks;
        currentLoadedLevel.scriptableObject = _levelData;
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

    #endregion

    #region Scriptable Object

    /// <summary>
    /// create a new blank level template
    /// </summary>
    public void CreateNewLevel()
    {
        GameObject levelObject = new GameObject("Unsaved Level " + storedLevels.Count);
        unsavedLevel = levelObject.AddComponent<LevelDataActive>();
        levelObject.transform.SetParent(transform);
    }

    /// <summary>
    /// Save the unsavedLevel as a new scriptable object
    /// </summary>
    public void SaveAsScriptableObject()
    {
        unsavedLevel.SaveAsScriptableObject(storedLevels.Count);
    }

    #endregion
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
