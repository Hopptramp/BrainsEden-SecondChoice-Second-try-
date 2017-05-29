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
    BeforeLevel,
    AfterLevel,
}

#endregion

/// <summary>
/// Contains any necessary information about the camera rotation
/// </summary>
public struct RotationData
{
    public CameraState fromState;
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
    public LevelDataScriptable loadedLevel;
    private int currentLevelID = 0;
    private int levelReachedID = 0;
    private Vector3 currentLevelStartPos;

    // scoring - Need scoring criteria for 1/2/3 stars
    private float timerValue = 0;
    private int stepsValue = 0;
    private int flipValue = -1; // workaround
    private int levelValue = 0;


    [SerializeField] private Text timerText;
    [SerializeField] private Text stepsText;
    [SerializeField] private Text flipText;
    [SerializeField] private Text levelText;





    private float playTime = 0;

    //public float killHeight = -5;
    //[SerializeField] bool ResetLevel = true;
    [SerializeField] private InGameMenuController belowMenu;

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
        rotation.TriggerRotation(-90, "x", 0);
        InitLevel();
        if(PersistantManager.instance)
            levelReachedID = PersistantManager.instance.levelReachedID;
        //BeginLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerFell();
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

    public void NextLevel()
    {
        int nextLevel = PersistantManager.instance.ReturnLevelID();

        // returns -1 when you complete last level
        if (nextLevel == -1)
        {
            QuitToMenu();
            return;
        }

        InitLevel();
        belowMenu.ActivateMenu(true, gameState);
    }

    void InitLevel()
    {
        rotation.TriggerRotation(0, "y");
        rotation.TriggerRotation(0, "z");
        gameState = GameState.BeforeLevel;
        ResetScoreTracking();
        UpdateRotationData(true);     
        currentLevelID = PersistantManager.instance == null ? 0 : PersistantManager.instance.ReturnLevelID();
        levelReachedID = currentLevelID > levelReachedID ? currentLevelID : levelReachedID;
        loadedLevel = levelManager.storedLevels[currentLevelID];
        belowMenu.UpdateMenuContent(loadedLevel, gameState);

        
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

    public void ResumePlay()
    {
        OnPlayStart(rotationData);
    }

    public void QuitToMenu()
    {
        PersistantManager.instance.SetMenuStateToMain();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Place player at start
    /// </summary>
    /// <param name="_position"></param>
    public void PlacePlayer(Vector3 _position)
    {
        currentLevelStartPos = _position;
        player.transform.position = _position;
        rotation.AttachToTempTarget(false);
    }

    public void PlayerFell()
    {
        StartCoroutine(ResetPlayer());
        rotation.AttachToTempTarget(true);
    }

    IEnumerator ResetPlayer()
    {
        yield return new WaitForSeconds(2);

        gameState = GameState.BeforeLevel;
        rotationData.gameState = gameState;
        rotation.TriggerRotation(-90, "x");
        player.GetComponent<FixedPlayerMovement>().Reset();
        PlacePlayer(currentLevelStartPos);
        InitLevel();
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

        CalculateScore(ref data);
        // inform levelmanager
        levelManager.OnLevelComplete(data, PersistantManager.instance != null ? PersistantManager.instance.ReturnNextLevelID() : ++currentLevelID);
        

        // onPlayPause(rotationData);
        gameState = GameState.AfterLevel;
        belowMenu.UpdateMenuContent(loadedLevel, gameState);

        rotationData.gameState = gameState;
        rotation.TriggerRotation(-90, "x");
        --flipValue; 
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

    void CalculateScore(ref LevelCompletionData _data)
    { 
        ScoreRequirements required = levelManager.storedLevels[currentLevelID].scoreRequirements;
        int stars = 0;
        if (_data.timeTaken < required.maxTime)
            ++stars;
        if (_data.totalFlips < required.maxFlips)
            ++stars;
        if (_data.totalSteps < required.maxSteps)
            ++stars;

        _data.stars = stars;
    }

    void ResetScoreTracking()
    {
        flipValue = 0;
        stepsValue = 0;
        timerValue = 0;
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
        bool wasPaused = false;
        if (rotationData.currentState == CameraState.Below)
            wasPaused = true;
        rotationData.currentState = cameraState;
        rotationData.gameState = gameState;
        postRotation(rotationData, isInit);

        if(gameState == GameState.AfterLevel)
            levelManager.RemoveLevel();


        if (cameraState == CameraState.Below) // entering pause
        {
            gameState = gameState == GameState.Play ? GameState.Pause : gameState;
            rotationData.gameState = gameState;
            onPlayPause(rotationData);
        }
        else if (wasPaused) // leaving pause
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
