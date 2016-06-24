using UnityEngine;
using System.Collections;

public enum CameraState
{
    Above,
    Left,
    Right,
    Front,
    Behind,
    Below
}

public enum VisibleState
{
    Visible,
    Invisible
}

public class GameManager : MonoBehaviour
{
    static GameManager m_instance;
    static public GameManager instance { get { return m_instance; } }

    public CameraState m_CameraState;
    

    // Use this for initialization
    void Start ()
    {
        m_instance = this;
        m_CameraState = CameraState.Above;

	}

    public void ChangePerspective(CameraState newPerspective)
    {
        // update the active blocks
        BlockManager.instance.UpdateActiveBlocks(m_CameraState, newPerspective);
        // update the new perspective
        m_CameraState = newPerspective;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
