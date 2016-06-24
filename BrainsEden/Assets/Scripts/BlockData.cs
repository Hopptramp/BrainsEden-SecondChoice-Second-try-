using UnityEngine;
using System.Collections;

public class BlockData : MonoBehaviour
{
    CameraState currentCameraState;
    VisibleState currentVisibleState { get { return currentVisibleState; } set { currentVisibleState = value; } }

    public CameraState myBasePerspective;
    public CameraState[] compatibleStates;


	// Use this for initialization
	void Start ()
    {
        BlockManager.instance.AddToBlockList(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentCameraState = GameManager.instance.m_CameraState;
	}
}
