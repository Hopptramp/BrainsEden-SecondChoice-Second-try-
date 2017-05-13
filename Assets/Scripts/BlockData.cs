using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockData : MonoBehaviour
{
    public BlockType blockType;
    public Vector3 localPosition;
    public int ID;

    /// <summary>
    /// initialise the block after being created
    /// </summary>
    public void Initialise()
    {
        switch (blockType)
        {
            case BlockType.Default:
                break;
            case BlockType.Teleport:
                break;
            case BlockType.Moving:
                break;
            case BlockType.Falling:
                break;
            default:
                break;
        }
    }

}
