using UnityEngine;
using System.Collections.Generic;

public enum BlockType
{
    Default,
    Fake,
    End,
	Mobile
}

public class BlockManager : MonoBehaviour
{
    static BlockManager m_instance;
    static public BlockManager instance { get { return m_instance; } }

    List<BlockData> blockList;


    // called by the block object, will add itself to it's respective list
    public void AddToBlockList(BlockData bData)
    {
        blockList.Add(bData);
    }

    void Awake()
    {
        m_instance = this;

        blockList = new List<BlockData>();
    }

    public void DestroyBlock(BlockData bData)
    {
        blockList.Remove(bData);
        Destroy(bData.gameObject);
    }

    public void UpdateActiveBlocks(CameraState currentState)
    {
        bool remainActive;

       for(int i = 0; i < blockList.Count; ++i)
        {
            remainActive = false;
            //loop through the blocks compatible
            for (int j = 0; j < blockList[i].compatibleStates.Length; ++j)
            {
                if(currentState == blockList[i].compatibleStates[j] || blockList[i].myBasePerspective == currentState)
                {
                    blockList[i].gameObject.SetActive(true);
                    remainActive = true;

                    break;
                }
            }
            // if no blocks are compatible
            if (remainActive == false)
            {
                // set it to inactive
                blockList[i].gameObject.SetActive(false);
            }
        }
    }

    public void BlockCollided(BlockData bData, Collision coll)
    {
        switch (bData.tag)
        {
		case "Mobile":
			break;
            case "Fake":
                if(coll.gameObject.tag == "Player")
                    FakeBlockCollided(bData);
            break;
            case "End":
                EndBlockCollided();
                break;

        }
    }

	void FakeBlockCollided(BlockData bData)
    {
        Rigidbody rb = bData.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = ~RigidbodyConstraints.FreezeAll;
    }

    void EndBlockCollided()
    {
        GameManager.instance.EndLevel(false);
    }

}
