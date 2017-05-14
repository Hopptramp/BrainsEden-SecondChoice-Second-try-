using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BlockData : GameActors
{
    public BlockType blockType;
    public Vector3 localPosition;
    public int ID;

#region Falling Block Variables
    public int startingHealth = 3;
    private int currentHealth;
#endregion

#region Teleporting Variables
    public BlockConnection [] connectedBlockIds = new BlockConnection[5];    
    private StoredBlockData currTargetBlock;
#endregion

#region Moving Variables
    public Vector3 destination;
    public float moveSpeed = 1;
#endregion

    public LevelDataActive level;

    private void Start()
    {
        // needs to be runtime (i think)
        InitDelegates();
    }

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
                currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(CameraState.Front));
                break;
            case BlockType.Moving:
                
                break;
            case BlockType.Falling:
                gameObject.SetActive(true);
                currentHealth = startingHealth;
                break;
            case BlockType.Start:
                GameManager.instance.PlacePlayer(transform.position + Vector3.up);
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


#region Delegates

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
                    currTargetBlock = GameManager.instance.levelManager.GetBlockByID(GetTeleportTarget(_rotationData.intendedState));
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

#endregion

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
                    Debug.Log(""+ currTargetBlock.ID + "__" + currTargetBlock.localPosition);
                    _player.TeleportTo(currTargetBlock.localPosition + Vector3.up);                  
                }
                break;
            case BlockType.Moving:
                break;
            case BlockType.Falling:
                currentHealth--;
                if (currentHealth <= 0)
                    StartCoroutine( RemoveBlock(_player, 1.5f));
                break;
            case BlockType.Start:
                break;
            case BlockType.End:
                GameManager.instance.CompleteLevel();
                break;
            default:
                break;
        }
    }

#region Block Type Functions

    int GetTeleportTarget(CameraState _state)
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

    IEnumerator RemoveBlock(FixedPlayerMovement _player, float _length)
    {
       
        float t = 0;
        while (t <= _length)
        {

            t += Time.deltaTime;
            yield return null;
        }
        yield return null;
        gameObject.SetActive(false);
        _player.OnMovementComplete();
    }

    IEnumerator MoveBlock()
    {
        transform.position = localPosition;
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        while (transform.position != localPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, localPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(MoveBlock());

    }

#endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(BlockData))]
public class BlockDataCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BlockData data = (BlockData)target;

        DrawDefaultInspector();

        //if (GUILayout.Button("Create Empty Level"))
        //{
        //    level.CreateNewLevel();
        //}

        //if (GUILayout.Button("Create From Scriptable Object"))
        //{
        //    level.GenerateLevelFromLevelData(level.storedLevels[0]);
        //}

        //if (GUILayout.Button("Save Scriptable Object"))
        //{
        //    level.SaveAsScriptableObject();
        //}


        // base.OnInspectorGUI();
    }
}
#endif




[System.Serializable]
public class BlockConnection
{
    public CameraState cameraView = CameraState.None;
    public int connectedBlock = -1;
}


