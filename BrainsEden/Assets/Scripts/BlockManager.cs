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

    public void UpdateActiveBlocks(CameraState previousState, CameraState currentState)
    {
        bool current = true;

        for (int j = 0; j < 2; j++)
        {
            switch (current ? currentState : previousState)
            {
                case CameraState.Above:
                    for (int i = 0; i < aboveBlocks.Count; ++i)
                    {
                        aboveBlocks[i].enabled = current ? true : false;
                    }
                    break;
                case CameraState.Below:
                    for (int i = 0; i < belowBlocks.Count; ++i)
                    {
                        belowBlocks[i].enabled = current ? true : false;
                    }
                    break;
                case CameraState.Front:
                    for (int i = 0; i < frontBlocks.Count; ++i)
                    {
                        frontBlocks[i].enabled = current ? true : false;
                    }
                    break;
                case CameraState.Behind:
                    for (int i = 0; i < behindBlocks.Count; ++i)
                    {
                        behindBlocks[i].enabled = current ? true : false;
                    }
                    break;
                case CameraState.Left:
                    for (int i = 0; i < leftBlocks.Count; ++i)
                    {
                        leftBlocks[i].enabled = current ? true : false;
                    }
                    break;
                case CameraState.Right:
                    for (int i = 0; i < rightBlocks.Count; ++i)
                    {
                        rightBlocks[i].enabled = current ? true : false;
                    }
                    break;
            }

            current = false;
        }
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
