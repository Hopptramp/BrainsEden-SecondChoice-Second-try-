using UnityEngine;
using System.Collections.Generic;

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

    public void UpdateActiveBlocks(CameraState currentState)
    {
        bool remainActive;

       for(int i = 0; i < blockList.Count; ++i)
        {
            remainActive = false;
            //loop through the blocks compatible
            for (int j = 0; j < blockList[i].compatibleStates.Length; ++j)
            {
                if(currentState == blockList[i].compatibleStates[j])
                {
                    blockList[i].gameObject.SetActive(true);
                    remainActive = true;
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

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
