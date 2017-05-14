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
    InBetweenLevels,
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
    public GameState gameState;
}

public class GameManager : MonoBehaviour
{
    static public GameManager instance { get; private set; }

    public static GameState gameState { get; private set; }
    public static CameraState cameraState { get; private set; }

    public GameObject player;
    public GameObject mainCamera;
    public Rotation rotation;
    [SerializeField] public LevelManager levelManager;
    private int currentLevelID = 0;

    // scoring - Need scoring criteria for 1/2/3 stars
    private float timerValue = 0;
    private int stepsValue = 0;
    private int flipValue = 0;
    private int levelValue = 0;


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
        instance = this;
        rotationData = new RotationData();
        rotationData.currentState = cameraState;
        rotationData.intendedState = cameraState;
        rotationData.transitionState = TransitionState.None;
        rotationData.target = player;
        rotationData.gameState = gameState;
    }

    // Use this for initialization
    void Start ()
    {
        InitGame();
        //BeginLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CompleteLevel();
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
        gameState = GameState.InBetweenLevels;
        UpdateRotationData(true);
        rotation.TriggerRotation(-90, "x");
        currentLevelID = PersistantManager.instance == null ? 0 : PersistantManager.instance.ReturnLevelID();
    }

    /// <summary>
    /// Receive button input to begin level
    /// </summary>
    public void BeginLevel()
    {
        gameState = GameState.Play;
        levelManager.SwitchLevels(currentLevelID);
        onPlayStart(rotationData);
    }

    /// <summary>
    /// Place player at start
    /// </summary>
    /// <param name="_position"></param>
    public void PlacePlayer(Vector3 _position)
    {
        player.transform.position = _position;
    }

    /// <summary>
    /// called when the player steps on the end tile
    /// </summary>
    public void CompleteLevel()
    {
        // store the level data in the scriptable object
        LevelCompletionData data = new LevelCompletionData();
        data.hasCompleted = true;
        data.timeTaken = timerValue;
        data.totalFlips = flipValue;
        data.totalSteps = stepsValue;

        // inform levelmanager
        levelManager.OnLevelComplete(data);
        levelManager.RemoveLevel();
        levelManager.SwitchLevels(++currentLevelID);
        // onPlayPause(rotationData);
        gameState = GameState.InBetweenLevels;
        rotationData.gameState = gameState;
        rotation.TriggerRotation(-90, "x");
    }

    #endregion

    #region Score
    void ScoreTracking()
    {
        timerValue += Time.deltaTime;
        timerText.text = timerValue.ToString("00:00");
        flipText.text = "Flips\n" + flipValue.ToString();
        stepsText.text = "Steps\n" + stepsValue.ToString();
        levelText.text = "Level\n" + levelValue.ToString();
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


        //update rotation data and set out
        rotationData.currentState = cameraState;
        rotationData.gameState = gameState;
        postRotation(rotationData, isInit);


        if (cameraState == CameraState.Below) // entering pause
        {
            gameState = gameState == GameState.InBetweenLevels ? GameState.InBetweenLevels : GameState.Pause;
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
        rotationData.gameState = gameState;
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PostRotationLogic(RotationData _rotationData, bool _isInit) { }

    #endregion


 
}
