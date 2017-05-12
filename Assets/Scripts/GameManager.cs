using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

#region Enum States

public enum CameraState
{
    Above,
    Left,
    Right,
    Front,
    Behind,
    Below,
    None
}

public enum TransitionState
{
    FromAboveToFront, 
    FromAboveToRight,
    FromAboveToLeft,
    FromAboveToBehind,

    FromLeftToFront,
    FromLeftToAbove,
    FromLeftToBelow,
    FromLeftToBehind,

    FromRightToFront,
    FromRightToAbove,
    FromRightToBelow,
    FromRightToBehind,

    FromBelowToFront,
    FromBelowToRight,
    FromBelowToLeft,
    FromBelowToBehind,

    FromFrontToAbove,
    FromFrontToRight,
    FromFrontToLeft,
    FromFrontToBelow,

    FromBehindToAbove,
    FromBehindToRight,
    FromBehindToLeft,
    FromBehindToBelow,

    None,
}

public enum VisibleState
{
    Visible,
    Invisible
}

public enum GameState
{
    Idle,
    Play,
    Pause,
}

#endregion

/// <summary>
/// Contains any necessary information about the camera rotation
/// </summary>
public struct RotationData
{
    public CameraState currentState;
    public CameraState intendedState;
    public TransitionState transitionState;
    public GameObject target;
}

public class GameManager : MonoBehaviour
{
    static GameManager m_instance;
    static public GameManager instance { get { return m_instance; } }

    public GameState gameState { get; private set; }
    public CameraState cameraState { get; private set; }

    [SerializeField] private Transform startPos;
    public GameObject player;
    public GameObject mainCamera;
    public Rotation rotation;

    private float playTime = 0;

    //public float killHeight = -5;
    //[SerializeField] bool ResetLevel = true;
    [SerializeField] GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject UpButton, DownButton;

    ///Pre and post rotation delegate events
    public delegate void PostRotation(RotationData _rotationData, bool _isInit);
    public delegate void PreRotation(RotationData _rotationData);
    public PostRotation postRotation = PostRotationLogic;
    public PreRotation preRotation = PreRotationLogic;

    public static RotationData rotationData;

    #region MonoBehaviour

    void Awake()
    {
        m_instance = this;
        rotationData = new RotationData();
        rotationData.currentState = cameraState;
        rotationData.intendedState = cameraState;
        rotationData.transitionState = TransitionState.None;
        rotationData.target = player;
    }

    // Use this for initialization
    void Start ()
    {
        InitGame();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
           // pauseMenu.SetActive(true);
        }
    }

    void InitGame()
    {
        player.transform.position = startPos.position;
        //player.GetComponent<PlayerOcclusionDetection>().mainCam = camera.transform;
        //mainCamera.GetComponent<Rotation>().player = player;
        UpdateRotationData(true);
    }

    #endregion



    #region Pre/Post rotation delegates

    /// <summary>
    /// Update rotation data struct and trigger PostRotation logic
    /// </summary>
    /// <param name="isInit"></param>
    public void UpdateRotationData(bool isInit)
    {
        Vector3 dir = player.transform.position - mainCamera.transform.position;
        dir = new Vector3(Mathf.Floor(dir.x), Mathf.Floor(dir.y), Mathf.Floor(dir.z));
        //print(dir);

        if (dir.y >= -11 && dir.y <= -9)
            cameraState = CameraState.Above;
        if (dir.y <= 11 && dir.y >= 9)
            cameraState = CameraState.Below;
        if (dir.z <= 11 && dir.z >= 9)
            cameraState = CameraState.Behind;
        if (dir.z >= -11 && dir.z <= -9)
            cameraState = CameraState.Front;
        if (dir.x <= 11 && dir.x >= 9)
            cameraState = CameraState.Left;
        if (dir.x >= -11 && dir.x <= -9)
            cameraState = CameraState.Right;

        if (cameraState != CameraState.Above && cameraState != CameraState.Below)
        {
            UpButton.GetComponent<Button>().interactable = false;
            DownButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            UpButton.GetComponent<Button>().interactable = true;
            DownButton.GetComponent<Button>().interactable = true;
        }

        BlockManager.instance.UpdateActiveBlocks(cameraState);

        //update rotation data and set out
        rotationData.currentState = cameraState;
        postRotation(rotationData, isInit);
        print("transition: " + rotationData.transitionState + "  - new State: " + rotationData.currentState);
    }


    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PreRotationLogic(RotationData _rotationData)
    {
        rotationData = _rotationData;
    }

    #endregion


 
}
