using UnityEngine;
using System.Collections;

public class BlockData : MonoBehaviour
{
    public BlockType m_blockType;
    public BlockType m_secondBlockType;
    
    CameraState currentCameraState;
    VisibleState currentVisibleState { get { return currentVisibleState; } set { currentVisibleState = value; } }

    public CameraState myBasePerspective;
    public CameraState[] compatibleStates;

	[SerializeField]
	private Vector3	firstPos, // starting pos
		secondPos; // end pos

	[SerializeField]
	private bool useSpeed; //use moveSpeed instead of moveTime

	[SerializeField]
	private float moveTime = 2f, moveSpeed = 2f;

	private bool moving;

	// Use this for initialization
	void Start ()
    {
        BlockManager.instance.AddToBlockList(this);

		//if a movile block, set the staring position the be the firstPos
		if (m_blockType == BlockType.Mobile) 
		{
			transform.position = firstPos;
		}
	}
        //if(m_blockType == BlockType.Fake)
        //    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    //}
	
	// Update is called once per frame
	void Update ()
    {
		if ((m_blockType == BlockType.Mobile && !moving) || (m_secondBlockType == BlockType.Mobile && !moving))
		{
			MoveToSecond();
			moving = true;
		}

        if(transform.position.y < -5)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //BlockManager.instance.DestroyBlock(this);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

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

	void MoveToSecond ()
	{
		if (useSpeed)
		{
			iTween.MoveTo (gameObject, iTween.Hash ("position", secondPos, "speed", moveSpeed, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "MoveToFirst"));
		}
		else
		{
			iTween.MoveTo (gameObject, iTween.Hash ("position", secondPos, "time", moveTime, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "MoveToFirst"));
		}
	}

	void MoveToFirst ()
	{
		if (useSpeed)
		{
			iTween.MoveTo (gameObject, iTween.Hash ("position", firstPos, "speed", moveSpeed, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "MoveToSecond"));
		}
		else
		{
			iTween.MoveTo (gameObject, iTween.Hash ("position", firstPos, "time", moveTime, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "MoveToSecond"));
		}
	}


}
