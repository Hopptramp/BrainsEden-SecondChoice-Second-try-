using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

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
        foreach (BlockData data in datas)
        {
            StoredBlockData storedBlock = new StoredBlockData();
            storedBlock.ID = ++temp;
            storedBlock.localPosition = data.transform.localPosition;
            storedBlock.type = data.blockType;
            storedBlock.block = data.gameObject;
            storedBlock.blockHealth = data.startingHealth;
            storedBlock.connectedBlocks = data.connectedBlockIds;
            storedBlocks.Add(storedBlock);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Level Scriptable Object")]
    public static LevelDataScriptable CreateMyAsset(int _id, List<StoredBlockData> _stored)
    {
        LevelDataScriptable asset;

        asset = ScriptableObject.CreateInstance<LevelDataScriptable>();


        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + "Level-" + _id + ".asset");


        // APPLY VARIABLES HERE
        asset.levelID = _id;
        asset.storedBlocks = _stored;
        AssetDatabase.SaveAssets();
        return asset;
    }

    public LevelDataScriptable SaveAsScriptableObject(int _id)
    {
        List<StoredBlockData> temp = new List<StoredBlockData>();
        for (int i = 0; i < storedBlocks.Count; ++i)
        {
            temp.Add(storedBlocks[i]);
        }

        scriptableObject = CreateMyAsset(_id, temp);
        return scriptableObject;

    }
#endif
}

#if UNITY_EDITOR
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

#endif
