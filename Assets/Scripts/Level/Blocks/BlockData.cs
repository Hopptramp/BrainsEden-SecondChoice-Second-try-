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
    public int ID = -1;
    public CameraState [] inactivePerspectives;
    protected BlockComponents designHolder;

    public LevelDataActive level;

    private void Start()
    {
        // needs to be runtime (i think)
        InitDelegates();
        
    }

    private void OnDisable()
    {
        RemoveDelegates();
    }

    private void OnDestroy()
    {
        RemoveDelegates();
    }

    /// <summary>
    /// initialise the block after being created
    /// </summary>
    public virtual void Initialise()
    {
        StoredBlockData data = level.GetBlockDatabyID(ID);
        data.block = gameObject;
        level.storedBlocks[ID] = data;

        //initialise design components
        designHolder = GameManager.instance.levelManager.GetBlockMaterial((int)blockType);
        GetComponent<MeshRenderer>().material = designHolder.material;
    }


#region Delegates

    /// <summary>
    /// Override to recieve the post rotation event;
    /// </summary>
    /// <param name="_rotationData"></param>
    /// <param name="_isInit"></param>
    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {        
        base.PostRotationLogic(_rotationData, _isInit);
    }

#endregion

    /// <summary>
    /// Called from player when player has moved onto a block.
    /// </summary>
    /// <param name="_player">Reference to the player script.</param>
    public virtual void BlockLandedOn(FixedPlayerMovement _player)
    {
        _player.transform.position = transform.position + Vector3.up;
        
    }    

    protected bool ActiveInPerspective(CameraState _state)
    {
        for (int i = 0; i < inactivePerspectives.Length; i++)
        {
            if (inactivePerspectives[i] == _state)
                return false;
        }
        return true;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(BlockData))]
[CanEditMultipleObjects]
public class BlockDataCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BlockData data = (BlockData)target;      

        //data.ID = EditorGUILayout.IntField("Block ID", data.ID);
        //data.localPosition = EditorGUILayout.Vector3Field("Block Position", data.localPosition);
        //data.blockType = (BlockType)EditorGUILayout.EnumPopup("Block Type", data.blockType);

        switch (data.blockType)
        {
            case BlockType.Default:
                break;
            case BlockType.Teleport:
                //SerializedProperty prop = serializedObject.FindProperty("connectedBlockIds");
                //serializedObject.Update();                
                //EditorGUILayout.PropertyField(prop, true);
                //serializedObject.ApplyModifiedProperties();                
                break;
            case BlockType.Moving:
                //data.destination = EditorGUILayout.Vector3Field("Move To", data.destination);
                //data.moveSpeed = EditorGUILayout.Slider(data.moveSpeed, 0, 10);
                break;
            case BlockType.Falling:
                //data.startingHealth = EditorGUILayout.IntField("Starting Health", data.startingHealth);
                break;
            case BlockType.Start:
                break;
            case BlockType.End:
                break;
            default:
                break;
        }
        DrawDefaultInspector();

       
    }
}
#endif







