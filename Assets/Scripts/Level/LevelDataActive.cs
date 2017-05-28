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

    public GameObject defaultCube;


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
        for(int i = 0; i <datas.Length; i++)
        {
            datas[i].ID = i;
        }
        foreach (BlockData data in datas)
        {
            StoredBlockData storedBlock = new StoredBlockData();            
            storedBlock.ID = data.ID;
            storedBlock.localPosition = data.transform.localPosition;
            storedBlock.type = data.blockType;
            storedBlock.inactivePerspectives = data.inactivePerspectives;
            switch (storedBlock.type)
            {
                case BlockType.Default:
                    break;
                case BlockType.Teleport:
                    Block_Teleport asTP = (data as Block_Teleport);
                    foreach (BlockConnection connect in asTP.connectedBlockIds)
                    {
                        connect.connectedBlockID = connect.blockD.ID;
                    }
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

    public void CreateBlock(BlockType _type)
    {
        GameObject blockObject = Instantiate(defaultCube, transform) as GameObject;
        blockObject.name = _type.ToString();
        BlockData block = blockObject.GetComponent<BlockData>();

        switch (_type)
        {
            case BlockType.Default:
                break;
            case BlockType.Teleport:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_Teleport>();
                //(block as Block_Teleport).connectedBlockIds = storedData.connectedBlocks;
                break;
            case BlockType.Moving:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_Moving>();
                //(block as Block_Moving).destination = storedData.destination;
               // (block as Block_Moving).moveSpeed = storedData.moveSpeed;
                break;
            case BlockType.Falling:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_Falling>();
               // (block as Block_Falling).startingHealth = storedData.blockHealth;
                break;
            case BlockType.Pushable:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_Pushable>();
                break;
            case BlockType.Start:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_Start>();
                break;
            case BlockType.End:
                DestroyImmediate(block);
                block = blockObject.AddComponent<Block_End>();
                break;
            default:
                break;
        }
    }

    public StoredBlockData GetBlockDatabyID( int _ID)
    {
        return storedBlocks[_ID];
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

        GUILayout.Space(5);

        if (GUILayout.Button("Create Start"))
        {
            level.CreateBlock(BlockType.Start);
        }
        if (GUILayout.Button("Create Teleport"))
        {
            level.CreateBlock(BlockType.Teleport);
        }
        if (GUILayout.Button("Create Pushable"))
        {
            level.CreateBlock(BlockType.Pushable);
        }
        if (GUILayout.Button("Create Moving"))
        {
            level.CreateBlock(BlockType.Moving);
        }
        if (GUILayout.Button("Create Falling"))
        {
            level.CreateBlock(BlockType.Falling);
        }
        if (GUILayout.Button("Create End"))
        {
            level.CreateBlock(BlockType.End);
        }

        //if (GUILayout.Button("Save Scriptable Object"))
        //{
        //    level.SaveAsScriptableObject();
        //}


        //   base.OnInspectorGUI();
    }
}

#endif
