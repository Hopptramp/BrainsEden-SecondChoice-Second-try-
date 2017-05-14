using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelDataScriptable : ScriptableObject
{
    [SerializeField] public int levelID;
    [SerializeField] public LevelCompletionData completionData;
    [SerializeField] public List<StoredBlockData> storedBlocks;
}

