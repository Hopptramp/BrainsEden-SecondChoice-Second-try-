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
    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
    
    void Awake()
    {
        m_instance = this;
       // m_CameraState = CameraState.Above;
    }

    // Use this for initialization
    void Start ()
    {
       

	}

    public void UpdateCameraState()
    {
        Vector3 dir = player.transform.position - camera.transform.position;
        dir = new Vector3(Mathf.Floor(dir.x), Mathf.Floor(dir.y), Mathf.Floor(dir.z));
        //print(dir);

        if (dir.y >= -11 && dir.y <= -9)
            m_CameraState = CameraState.Above;
        if (dir.y <= 11 && dir.y >= 9)
            m_CameraState = CameraState.Below;
        if (dir.z <= 11 && dir.z >= 9)
            m_CameraState = CameraState.Behind;
        if (dir.z >= -11 && dir.z <= -9)
            m_CameraState = CameraState.Front;
        if (dir.x <= 11 && dir.x >= 9)
            m_CameraState = CameraState.Left;
        if (dir.x >= -11 && dir.x <= -9)
            m_CameraState = CameraState.Right;

        BlockManager.instance.UpdateActiveBlocks(m_CameraState);
    }

	
	// Update is called once per frame
	void Update ()
    {
       // UpdateCameraState();
	}
}
