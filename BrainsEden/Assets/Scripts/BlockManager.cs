using UnityEngine;
using System.Collections.Generic;

public enum BlockType
{
    Default,
    Fake,
    End,
	Mobile,
	Kill
}

public class BlockManager : MonoBehaviour
{
    static BlockManager m_instance;
    static public BlockManager instance { get { return m_instance; } }

    List<BlockData> blockList;

	List<GameObject> corpse = new List<GameObject>();

	public List<List<BlockData>> shadeLists;
	public List<float> multipliers; //stores the multipliers for each layer

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

	public void DepthShade(Vector3 _camForward)
	{
		Vector3 pass = Vector3.zero;

		if (_camForward == Vector3.right)
		{
			pass = Vector3.right;
		}
		else if (_camForward == Vector3.left)
		{
			pass = Vector3.left;
		}
		else if (_camForward == Vector3.up)
		{
			pass = Vector3.up;
		}
		else if (_camForward == Vector3.down)
		{
			pass = Vector3.down;
		}
		else if (_camForward == Vector3.forward)
		{
			pass = Vector3.forward;
		}
		else
		{
			pass = Vector3.back;
		}

		Shade (pass);
	}

	private void Shade (Vector3 axis)
	{
		if (shadeLists == null)
		{
			shadeLists = new List<List<BlockData>> ();
			multipliers = new List<float> ();
		}
		else
		{
			//reset colours before clearing
			//go through each LIST OF LISTS
			for (int i = 0; i < shadeLists.Count; i++)
			{
				multipliers[i] = 1 / multipliers[i];

				//go through each block in the individual Lists
				for (int j = 0; j < shadeLists [i].Count; j++)
				{
					//apply the multiplier to each block's colour 
					Color temp = shadeLists[i][j].gameObject.GetComponent<MeshRenderer>().material.color;
					temp.r *= multipliers[i];
					temp.g *= multipliers[i];
					temp.b *= multipliers[i];
					shadeLists [i] [j].gameObject.GetComponent<MeshRenderer> ().material.color = temp;
				}
			}

			shadeLists.Clear ();
			multipliers.Clear();
		}

		//sort all blocks into lists according to pos value in axis direction 
		for (int i = 0; i < blockList.Count; i++)
		{
			//at the first block, create a new list
			if (i == 0)
			{
				shadeLists.Add (new List<BlockData> ());
				shadeLists [0].Add (blockList [i]);

				//no need to continue for this val
				continue;
			}

			//has the value been added to a list?
			bool added = false;

			//search the lists for one with the same converted value
			for (int j = 0; j < shadeLists.Count; j++)
			{
				added = false;

				//if the converted value of this list == the converted value of the current
				if (CustomVectMultiply(shadeLists [j] [0].transform.position, axis) == CustomVectMultiply(blockList [i].transform.position, axis))
				{
					shadeLists [j].Add (blockList [i]);

					//say that the value was found in a list
					added = true;
					break;
				}
			}

			//if the value wasnt found in a list, create a new one
			if (!added)
			{
				//create a new list and add the current block
				List<BlockData> tempList = new List<BlockData> ();
				tempList.Add (blockList [i]);

				//add the list to the list of lists
				shadeLists.Add (tempList);
			}
		}

		//List<List<BlockData>> firstCopy = shadeLists;
		string comparison;

		//FIND THE VALUE TO COMPARE
		if (axis.x != 0)
		{
			if (axis.x == -1)
			{
				//sort descending in x
				comparison = "-x";
			}
			else
			{
				//sort ascending in x
				comparison = "x";
			}
		}
		else if (axis.y != 0)
		{
			if (axis.y == -1)
			{
				comparison = "-y";
			}
			else
			{
				comparison = "y";
			}
		}
		else
		{
			if (axis.z == -1)
			{
				//sort descending in z
				comparison = "-z";
			}
			else
			{
				//sort ascending in z
				comparison = "z";
			}
		}

		//SORT THE LISTS according to their converted square magnitude values
		for (int i = 0; i < shadeLists.Count; i++)
		{
			for (int j = 0; j < shadeLists.Count - 1; j++)
			{
				switch (comparison)
				{
				case "x":
					//if one at j is > one at j+1
					if (shadeLists [j][0].transform.position.x < shadeLists [j + 1][0].transform.position.x)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}
					break;
				case "-x":
					if (shadeLists [j][0].transform.position.x > shadeLists [j + 1][0].transform.position.x)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}					
					break;
				case "y":
					if (shadeLists [j][0].transform.position.y < shadeLists [j + 1][0].transform.position.y)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}
					break;
				case "-y":
					if (shadeLists [j][0].transform.position.y > shadeLists [j + 1][0].transform.position.y)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}
					break;
				case "z":
					if (shadeLists [j][0].transform.position.z < shadeLists [j + 1][0].transform.position.z)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}
					break;
				case "-z":
					if (shadeLists [j][0].transform.position.z > shadeLists [j + 1][0].transform.position.z)
					{
						List<BlockData> temp = shadeLists [j + 1];
						shadeLists [j + 1] = shadeLists [j];
						shadeLists [j] = temp;
					}
					break;
				}
			}
		}

		List<List<BlockData>> copy = shadeLists;

		List<BlockData> one = shadeLists [0],
		two = shadeLists [1],
		three = shadeLists [2];

		//GO THROUGH AND DO THE CURRENT-LIST-NUMBER/NUMBER-OF-LISTS, STORE AS MULTIPLIER FLOAT?
		float noOfLists = shadeLists.Count;

		//go through each LIST OF LISTS
		for (int i = 0; i < shadeLists.Count; i++)
		{
			multipliers.Add(1 - (i / noOfLists));

			//go through each block in the individual Lists
			for (int j = 0; j < shadeLists [i].Count; j++)
			{
				//apply the multiplier to each block's colour 
				Color temp = shadeLists[i][j].gameObject.GetComponent<MeshRenderer>().material.color;
				temp.r *= multipliers[i];
				temp.g *= multipliers[i];
				temp.b *= multipliers[i];
				shadeLists [i] [j].gameObject.GetComponent<MeshRenderer> ().material.color = temp;
			}
		}
	}

	private Vector3 CustomVectMultiply(Vector3 _first, Vector3 _second)
	{
		Vector3 output = new Vector3 ();
		output.x = _first.x * _second.x;
		output.y = _first.y * _second.y;
		output.z = _first.z * _second.z;

		return output;
	}

    public void BlockCollided(BlockData bData, Collision coll)
    {
        switch (bData.tag)
        {
		case "Kill":
			if(coll.gameObject.tag == "Player")
				KillBlockCollided();
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

	void KillBlockCollided()
	{
		if (corpse.Count != 0)
		{
			foreach (GameObject _obj in corpse)
			{
				Destroy (_obj);
			}

			corpse.Clear ();
		}

		//LOGIC FOR DISCONNECTING THE PLAYER AND ASSIGNING THEIR COMPONENETS TO CORPSE

		GameManager.instance.EndLevel (false);
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
