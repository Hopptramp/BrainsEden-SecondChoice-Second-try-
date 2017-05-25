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

        int highestID = -1;
        BlockData[] datas = GetComponentsInChildren<BlockData>();
        for(int i = 0; i <datas.Length; i++)
        {
            if (datas[i].ID > highestID)
                highestID = datas[i].ID;
        }
        foreach (BlockData data in datas)
        {
            StoredBlockData storedBlock = new StoredBlockData();
            if (data.ID < 0)
                data.ID = ++highestID;
            storedBlock.ID = data.ID;
            storedBlock.localPosition = data.transform.localPosition;
            storedBlock.type = data.blockType;
            storedBlock.inactivePerspectives = data.inactivePerspectives;
            switch (storedBlock.type)
            {
                case BlockType.Default:
                    break;
                case BlockType.Teleport:
                    storedBlock.connectedBlocks = (data as Block_Teleport).connectedBlockIds;
                    break;
                case BlockType.Moving:
                    storedBlock.destination = (data as Block_Moving).destination;
                    storedBlock.moveSpeed = (data as Block_Moving).moveSpeed;
                    break;
                case BlockType.Falling:
                    storedBlock.blockHealth = (data as Block_Falling).startingHealth;
                    break;
                case BlockType.Pushable:
                    break;
                case BlockType.Start:
                    break;
                case BlockType.End:
                    break;
                default:
                    break;
            }
            storedBlock.block = data.gameObject;
            storedBlocks.Add(storedBlock);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Level Scriptable Object")]
    public static LevelDataScriptable CreateLevelDataScriptable(List<StoredBlockData> _stored, string _fileName)
    {
        LevelDataScriptable asset;
        asset = ScriptableObject.CreateInstance<LevelDataScriptable>();

        if (Resources.Load(_fileName) != null)
        {
            Debug.LogError("File already exists.");
            return asset;
        }
          
        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + _fileName + ".asset");

        // APPLY VARIABLES HERE
        //asset.levelID = _id;
        asset.storedBlocks = _stored;
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(asset);
        return asset;
    }


    public LevelDataScriptable SaveAsScriptableObject(string _fileName)
    {
        List<StoredBlockData> temp = new List<StoredBlockData>();
        for (int i = 0; i < storedBlocks.Count; ++i)
        {
            temp.Add(storedBlocks[i]);
        }

        scriptableObject = CreateLevelDataScriptable(temp, _fileName);
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
