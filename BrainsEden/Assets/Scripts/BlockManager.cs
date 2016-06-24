using UnityEngine;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    static BlockManager m_instance;
    static public BlockManager instance { get { return m_instance; } }

    List<BlockData> aboveBlocks;
    List<BlockData> belowBlocks;
    List<BlockData> leftBlocks;
    List<BlockData> rightBlocks;
    List<BlockData> frontBlocks;
    List<BlockData> behindBlocks;


    // called by the block object, will add itself to it's respective list
    public void AddToBlockList(BlockData bData)
    {
        switch(bData.myBasePerspective)
        {
            case CameraState.Above:
                aboveBlocks.Add(bData);
                break;
            case CameraState.Below:
                belowBlocks.Add(bData);
                break;
            case CameraState.Front:
                frontBlocks.Add(bData);
                break;
            case CameraState.Behind:
                behindBlocks.Add(bData);
                break;
            case CameraState.Left:
                leftBlocks.Add(bData);
                break;
            case CameraState.Right:
                rightBlocks.Add(bData);
                break;
        }
    }

    void Awake()
    {
        m_instance = this;

        aboveBlocks = new List<BlockData>();
        belowBlocks = new List<BlockData>();
        leftBlocks = new List<BlockData>();
        rightBlocks = new List<BlockData>();
        frontBlocks = new List<BlockData>();
        behindBlocks = new List<BlockData>();
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
