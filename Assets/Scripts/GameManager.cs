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

    public static GameState gameState { get; private set; }
    public static CameraState cameraState { get; private set; }



    [SerializeField] private Transform startPos;
    public GameObject player;
    public GameObject mainCamera;
    public Rotation rotation;

    // scoring
    private float timerValue = 0;
    private float stepsValue = 0;
    private float flipValue = 0;
    private float levelValue = 0;

    [SerializeField] private Text timerText;
    [SerializeField] private Text stepsText;
    [SerializeField] private Text flipText;
    [SerializeField] private Text levelText;



    private float playTime = 0;

    //public float killHeight = -5;
    //[SerializeField] bool ResetLevel = true;
    [SerializeField] private Transform belowMenu;

    [SerializeField] GameObject pauseMenu;


    // delegate events
    public delegate void PostRotation(RotationData _rotationData, bool _isInit);
    public delegate void DataDelegate(RotationData _rotationData);
    public PostRotation postRotation = PostRotationLogic;
    public DataDelegate preRotation = PreRotationLogic;
    public DataDelegate onPlayStart = OnPlayStart;
    public DataDelegate onPlayPause = OnPlayPause;

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
        BeginLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
           // pauseMenu.SetActive(true);
        }

        if(gameState == GameState.Play)
        {            
            ScoreTracking();
        }
        else if (gameState == GameState.Pause)
        {

        }
        else
        {

        }
    }
    #endregion

    #region Game Management

    void InitGame()
    {
        player.transform.position = startPos.position;
        UpdateRotationData(true);
    }

    /// <summary>
    /// Receive button input to begin level
    /// </summary>
    public void BeginLevel()
    {
        onPlayStart(rotationData);
    }

    public void CompleteLevel()
    {
        onPlayPause(rotationData);
    }

    #endregion

    #region Score
    void ScoreTracking()
    {
        timerValue += Time.deltaTime;
        timerText.text = timerValue.ToString("00:00");
        flipText.text = "Flips: " + flipValue.ToString();
        stepsText.text = "Steps: " + stepsValue.ToString();
        levelText.text = "Level: " + levelValue.ToString();
    }

    public void IncrementSteps()
    {
        ++stepsValue;
    }

    #endregion

    #region delegates

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

        // MOVE TO POST ROTATION LOGIC IN BLOCK MANAGER     
        //BlockManager.instance.UpdateActiveBlocks(cameraState);

        //update rotation data and set out
        rotationData.currentState = cameraState;
        postRotation(rotationData, isInit);
        print("transition: " + rotationData.transitionState + "  - new State: " + rotationData.currentState);

        if (cameraState == CameraState.Below) // entering pause
        {
            gameState = GameState.Pause;
            onPlayPause(rotationData);
        }
        else if (gameState == GameState.Pause) // leaving pause
        {

        }
        else // normal flip
        {
            ++flipValue; 
        }

        
    }

    /// <summary>
    /// Delegate that triggers play start logic in other scripts
    /// </summary>
    static void OnPlayStart(RotationData _rotationData)
    {
        gameState = GameState.Play;
    }

    /// <summary>
    /// Delegate that triggers any pauses in logic in other scripts
    /// </summary>
    static void OnPlayPause(RotationData _rotationData) { }

    /// <summary>
    /// Delegate that triggers any pre rotation logic in other scripts
    /// </summary>
    static void PreRotationLogic(RotationData _rotationData)
    {
        if (gameState == GameState.Pause && _rotationData.intendedState != CameraState.Below)
        {
            gameState = GameState.Play;
            instance.onPlayStart(rotationData);
        }
        rotationData = _rotationData;
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PostRotationLogic(RotationData _rotationData, bool _isInit) { }

    #endregion


 
}
