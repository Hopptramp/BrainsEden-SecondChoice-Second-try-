using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block_Teleport : BlockData {

    [SerializeField]
    public BlockConnection[] connectedBlockIds = new BlockConnection[5];
    private StoredBlockData currTargetBlock;
    [SerializeField]
    private ParticleSystem destinationParticles;


    public override void Initialise()
    {
        currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(CameraState.Behind));
        blockType = BlockType.Teleport;
        base.Initialise();

        ParticleSystem particles = Instantiate(designHolder.particles, transform) as ParticleSystem;
        particles.transform.localPosition = new Vector3(0, 0.5f, 0);
        destinationParticles = Instantiate(designHolder.secondParticles, transform) as ParticleSystem;
        destinationParticles.transform.localPosition = new Vector3(0, 0.5f, 0);
        SetDestinationParticles(CameraState.Behind);
        
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        if (GetTeleportTarget(_rotationData.intendedState) != ID)
            currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(_rotationData.intendedState));
        SetDestinationParticles(_rotationData.intendedState);
        base.PostRotationLogic(_rotationData, _isInit);
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        if (currTargetBlock.ID != ID)
        {
            if (!Physics.Raycast(currTargetBlock.localPosition, Vector3.up, 1))
                _player.TeleportTo(currTargetBlock.localPosition + Vector3.up);
        }
        base.BlockLandedOn(_player);
    }

    int GetTeleportTarget(CameraState _state)
    {
        for (int i = 0; i < connectedBlockIds.Length; i++)
        {
            if (connectedBlockIds[i].cameraView == _state && connectedBlockIds[i].connectedBlockID != -1)
            {
                return connectedBlockIds[i].connectedBlockID;
            }
        }
        return ID;
    }

    void SetDestinationParticles (CameraState _state)
    {
        if (currTargetBlock.ID != ID)
        {
            destinationParticles.gameObject.SetActive(true);
            destinationParticles.transform.position = currTargetBlock.localPosition + (Vector3.up * 1.5f);
        }
        else
            destinationParticles.gameObject.SetActive(false);

    }

}


[System.Serializable]
public class BlockConnection
{
    public CameraState cameraView = CameraState.None;
    public int connectedBlockID = -1;
    public BlockData blockD;
}