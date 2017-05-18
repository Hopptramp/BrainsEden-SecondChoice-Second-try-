using UnityEngine;
using System.Collections;

[System.Serializable]
public class TeleportBlock : BlockData {

    [SerializeField]
    public BlockConnection[] connectedBlockIds = new BlockConnection[5];
    private StoredBlockData currTargetBlock;    

    public override void Initialise()
    {
        currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(CameraState.Front));
        blockType = BlockType.Teleport;
        base.Initialise();
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(_rotationData.intendedState));
        base.PostRotationLogic(_rotationData, _isInit);
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        if (currTargetBlock.ID != ID)
        {
            Debug.Log("" + currTargetBlock.ID + "-" + currTargetBlock.localPosition);
            _player.TeleportTo(currTargetBlock.localPosition + Vector3.up);
        }
        base.BlockLandedOn(_player);
    }

    int GetTeleportTarget(CameraState _state)
    {
        for (int i = 0; i < connectedBlockIds.Length; i++)
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