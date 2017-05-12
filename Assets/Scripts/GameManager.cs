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

#endregion

/// <summary>
/// Contains any necessary information about the camera rotation
/// </summary>
public struct RotationData
{
    public CameraState currentState;
    public CameraState intendedState;
    public TransitionState transitionState;
}

public class GameManager : MonoBehaviour
{
    static GameManager m_instance;
    static public GameManager instance { get { return m_instance; } }

    public CameraState cameraState;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    public GameObject player;
    public GameObject mainCamera;

    public float killHeight = -5;
    [SerializeField] bool ResetLevel = true;
    [SerializeField] GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject UpButton, DownButton;

    ///Pre and post rotation delegate events
    public delegate void RotationEvents(RotationData _rotationData);
    public RotationEvents postRotation = PostRotationLogic;
    public RotationEvents preRotation = PreRotationLogic;

    public static RotationData rotationData;


    void Awake()
    {
        m_instance = this;
        rotationData = new RotationData();
        rotationData.currentState = cameraState;
        rotationData.intendedState = CameraState.None;
        rotationData.transitionState = TransitionState.None;
    }

    // Use this for initialization
    void Start ()
    {
        //DontDestroyOnLoad(gameObject);
        InitGame();

    }

    void InitGame()
    {
        player.transform.position = spawnPoint.position;
        //player.GetComponent<PlayerOcclusionDetection>().mainCam = camera.transform;
        mainCamera.GetComponent<Rotation>().player = player;
        UpdateCameraState();
    }

    public void UpdateCameraState()
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
        postRotation(rotationData);
        print("transition: " + rotationData.transitionState + "  - new State: " + rotationData.currentState);
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PostRotationLogic(RotationData _rotationData)
    {
        
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    static void PreRotationLogic(RotationData _rotationData)
    {
        rotationData = _rotationData;
    }


    // Update is called once per frame
    void Update ()
    {
       if (player.transform.position.y <= killHeight)
        {
            EndLevel(null);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseMenu.SetActive(true);
        }
    }

    public void PrepEndLevel()
    {
        endMenu.SetActive(true);
        ScoreManager.instance.runUpdate = false;
        endMenu.GetComponent<EndLevelMenu>().SetEndValues(ScoreManager.instance.timerValue, (int)ScoreManager.instance.flipValue, (int)ScoreManager.instance.jumpValue);
    }

    public void EndLevel(bool? overwriteReset)
    {
        if (overwriteReset.HasValue ? (bool)overwriteReset : ResetLevel)
        {
            Destroy(gameObject);
#if UNITY_5_3_OR_NEWER               
            SceneManager.LoadScene(DataHolder.instance.currentLevel);
#else
            Application.LoadLevel(DataHolder.instance.currentLevel);
#endif
        }
        else
        {
#if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(++DataHolder.instance.currentLevel > 11 ? 0 : DataHolder.instance.currentLevel);
#else
            Application.LoadLevel(++DataHolder.instance.currentLevel > 11 ? 0 : DataHolder.instance.currentLevel);
#endif
            InitGame();
        }
    }
}
