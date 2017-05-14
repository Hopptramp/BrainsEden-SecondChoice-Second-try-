using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelDataScriptable : ScriptableObject
{
    public int levelID;
    public LevelCompletionData completionData;
    public List<StoredBlockData> storedBlocks;
}

