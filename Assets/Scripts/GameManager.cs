using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

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
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    public GameObject player;
    public GameObject camera;

    public float killHeight = -5;
    [SerializeField] bool ResetLevel = true;
    [SerializeField] GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject UpButton, DownButton;


    
    
    void Awake()
    {
        m_instance = this;
       // m_CameraState = CameraState.Above;
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
        camera.GetComponent<Rotation>().player = player;
        UpdateCameraState();
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

        if (m_CameraState != CameraState.Above && m_CameraState != CameraState.Below)
        {
            UpButton.GetComponent<Button>().interactable = false;
            DownButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            UpButton.GetComponent<Button>().interactable = true;
            DownButton.GetComponent<Button>().interactable = true;
        }

        BlockManager.instance.UpdateActiveBlocks(m_CameraState);
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
