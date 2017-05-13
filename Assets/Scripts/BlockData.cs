﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockData : GameActors
{
    public BlockType blockType;
    public Vector3 localPosition;
    public int ID;

    #region Teleporting Variables
    public BlockConnection [] connectedBlockIds = new BlockConnection[5];    
    private StoredBlockData currTargetBlock;
    #endregion

    public LevelDataActive level;


    /// <summary>
    /// initialise the block after being created
    /// </summary>
    public void Initialise()
    {
        InitDelegates();
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
            case BlockType.Start:
                break;
            case BlockType.End:
                break;
            default:
                break;
        }

        StoredBlockData data = level.storedBlocks[ID];
        data.block = gameObject;
        level.storedBlocks[ID] = data;
    }

    

    /// <summary>
    /// Override to recieve the post rotation event;
    /// </summary>
    /// <param name="_rotationData"></param>
    /// <param name="_isInit"></param>
    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        switch (blockType)
        {
            
            case BlockType.Default:
                break;
            case BlockType.Teleport:       
                //This isnt working properly yet;                       
                    currTargetBlock = GameManager.instance.levelManager.GetBlockByID(getTeleportTarget(_rotationData.intendedState));
                break;
            case BlockType.Moving:
                break;
            case BlockType.Falling:
                break;
            case BlockType.Start:
                break;
            case BlockType.End:
                break;
            default:
                break;
        }
        base.PostRotationLogic(_rotationData, _isInit);
    }

    /// <summary>
    /// Called from player when player has moved onto a block.
    /// </summary>
    /// <param name="_player">Reference to the player script.</param>
    public void BlockLandedOn(FixedPlayerMovement _player)
    {        
        switch (blockType)
        {
            case BlockType.Default:
                break;
            case BlockType.Teleport:
                if (currTargetBlock.ID != ID)
                {
                    //temporary, will make a function in player to call from here.      
                    Debug.Log(""+ currTargetBlock.ID + "__" + currTargetBlock.localPosition);
                    _player.TeleportTo(currTargetBlock.localPosition + Vector3.up);
                    //_player.transform.position = currTargetBlock.localPosition + Vector3.up;
                }
                break;
            case BlockType.Moving:
                break;
            case BlockType.Falling:
                break;
            case BlockType.Start:
                break;
            case BlockType.End:
                break;
            default:
                break;
        }
    }

    int getTeleportTarget(CameraState _state)
    {
        for (int i = 0; i < connectedBlockIds.Length; i ++)
        {
            if (connectedBlockIds[i].cameraView == _state)
            {
                return connectedBlockIds[i].connectedBlock;
            }
        }
        return ID;
    }

}


[System.Serializable]
public class BlockConnection
{
    public CameraState cameraView = CameraState.None;
    public int connectedBlock = -1;
}


