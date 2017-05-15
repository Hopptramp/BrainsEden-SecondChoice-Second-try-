using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
public class StoredBlockData
{
    public int ID;
    public Vector3 localPosition;
    public BlockType type;
    public GameObject block;
    public int blockHealth;
    public BlockConnection[] connectedBlocks;
    public Vector3 destination;
    public float moveSpeed;
}

[System.Serializable]
public class LevelCompletionData
{
    public bool hasCompleted;
    public float timeTaken;
    public int totalFlips;
    public int totalSteps;
}

public class LevelManager : GameActors
{
    public LevelDataActive unsavedLevel;
    public List<LevelDataScriptable> storedLevels;
    [SerializeField] private int activeLevelID = 0;
    [SerializeField] private LevelDataActive currentLoadedLevel;

    [SerializeField] private GameObject defaultCube;
    [SerializeField] string newFileName;
    
    private void Awake()
    {
        unsavedLevel.gameObject.SetActive(false);

        if (PersistantManager.instance)
            storedLevels = PersistantManager.instance.ReturnStoredLevels();
        else
        {
            Object[] levels = Resources.LoadAll("");
            for (int i = 0; i < levels.Length; ++i)
            {
                storedLevels.Add((LevelDataScriptable)levels[i]);
            }
        }
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
        for(int i = 0; i < currentLoadedLevel.storedBlocks.Count; i++)
        {
            if (currentLoadedLevel.storedBlocks[i].ID == _id)
                return currentLoadedLevel.storedBlocks[i];
        }
        return currentLoadedLevel.storedBlocks[_id];
    }

    /// <summary>
    /// apply score changes to level
    /// </summary>
    /// <param name="_data"></param>
    public void OnLevelComplete(LevelCompletionData _data, int _levelReachedID)
    {
        //currentLoadedLevel.completionData = _data;
        storedLevels[activeLevelID].completionData = _data;
        if(PersistantManager.instance != null)
            PersistantManager.instance.UpdateChangesToCompletionData(storedLevels, _levelReachedID);
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
    public bool SwitchLevels(int _newlevel)
    {
        if (_newlevel >= storedLevels.Count)
            return false;
        activeLevelID = _newlevel;
        if (currentLoadedLevel != null)
            RemoveLevel(currentLoadedLevel);
        GenerateLevelFromLevelData(storedLevels[_newlevel]);
        return true;
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
    public void RemoveLevel(LevelDataActive _level)
    {
        foreach(StoredBlockData stored in _level.storedBlocks)
        {
            DestroyImmediate(stored.block);
        }
        DestroyImmediate(_level.blockHolder);
    }

    /// <summary>
    /// Remove the passed in level
    /// </summary>
    /// <param name="_level"></param>
    public void RemoveLevel()
    {
        foreach (StoredBlockData stored in currentLoadedLevel.storedBlocks)
        {
            DestroyImmediate(stored.block);
        }
        DestroyImmediate(currentLoadedLevel.blockHolder);
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
            block.startingHealth = storedData.blockHealth;
            block.connectedBlockIds = storedData.connectedBlocks;
            block.destination = storedData.destination;
            block.moveSpeed = storedData.moveSpeed;
            block.Initialise();
        }
    }

    #endregion

    #region Scriptable Object

#if UNITY_EDITOR
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
        unsavedLevel.SaveAsScriptableObject(newFileName);
    }
#endif

#endregion
}

#if UNITY_EDITOR

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
            level.GenerateLevelFromLevelData(level.storedLevels[0]);
        }

        if (GUILayout.Button("Save Scriptable Object"))
        {
            level.SaveAsScriptableObject();
        }


        // base.OnInspectorGUI();
    }
}

#endif