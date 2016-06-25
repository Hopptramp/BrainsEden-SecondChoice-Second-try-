using UnityEngine;
using System.Collections;

public class BlockData : MonoBehaviour
{
    public BlockType m_blockType;
    CameraState currentCameraState;
    VisibleState currentVisibleState { get { return currentVisibleState; } set { currentVisibleState = value; } }

    public CameraState myBasePerspective;
    public CameraState[] compatibleStates;


	// Use this for initialization
	void Start ()
    {
        BlockManager.instance.AddToBlockList(this);
        //if(m_blockType == BlockType.Fake)
        //    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentCameraState = GameManager.instance.m_CameraState;
	}

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject == GameManager.instance.player)
        {
            BlockManager.instance.BlockCollided(this, coll);
            if (m_blockType == BlockType.Fake)
                Invoke("DestroyBlock", 3);
            
        }
    }

    void DestroyBlock()
    {
        BlockManager.instance.DestroyBlock(this);
    }
}
