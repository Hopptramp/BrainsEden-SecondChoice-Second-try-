using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class LevelCounter : ScriptableObject
{
    [SerializeField] public int levelIteration;

}