using UnityEngine;
using System.Collections;

public class BlockData : MonoBehaviour
{
    CameraState currentCameraState { get { return currentCameraState; } set { currentCameraState = value; } }
    VisibleState currentVisibleState { get { return currentVisibleState; } set { currentVisibleState = value; } }

    public CameraState myBasePerspective;
    [SerializeField] CameraState[] compatibleStates;


	// Use this for initialization
	void Start ()
    {
        BlockManager.instance.AddToBlockList(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
