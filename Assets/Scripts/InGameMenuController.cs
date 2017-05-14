using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
public struct InitialiseCanvasContent
{
    public Text levelName;
}

[System.Serializable]
public struct PauseCanvasContent
{
    public Text levelName;
}

[System.Serializable]
public struct AfterLevelCanvasContent
{
    public Text levelName;
    public Text score;
}

public class InGameMenuController : GameActors
{
    private Rotation cameraRotate;

    [SerializeField] private CanvasGroup initLevelCanvas;
    [SerializeField] private CanvasGroup pauseCanvas;
    [SerializeField] private CanvasGroup afterLevelCanvas;
    [SerializeField] private GameObject panel;

    // Init canvas content
    [SerializeField] InitialiseCanvasContent initCanvasContent;
    [SerializeField] PauseCanvasContent pauseCanvasContent;
    [SerializeField] AfterLevelCanvasContent afterLevelContent;

    private void Awake()
    {
        ActivateMenu(false, GameState.BeforeLevel);
    }

    private void Start()
    {
        cameraRotate = GameManager.instance.rotation;
        InitDelegates();
    }

    /// <summary>
    /// Update content to do with the current level
    /// </summary>
    /// <param name="_level"></param>
    public void UpdateMenuContent(LevelDataScriptable _level, GameState _gameState)
    {
        if (_gameState == GameState.BeforeLevel)
        {
            initCanvasContent.levelName.text = _level.name;

        }
        else if (_gameState == GameState.Pause)
        {
            pauseCanvasContent.levelName.text = _level.name;


        }
        else if (_gameState == GameState.AfterLevel)
        {
            afterLevelContent.levelName.text = _level.name;
            afterLevelContent.score.text = "Score: " + _level.completionData.timeTaken.ToString("00:00");
        }

    }

    protected override void OnPlayPause(RotationData _rotationData)
    {
        ActivateMenu(true, _rotationData.gameState);
    }

    protected override void OnPlayStart(RotationData _rotationData)
    {
        ActivateMenu(false, _rotationData.gameState);
    }

    protected override void PreRotationLogic(RotationData _rotationData)
    {
        //if leaving pause 
        if (_rotationData.currentState == CameraState.Below)
            ActivateMenu(false, _rotationData.gameState);
    }

    public void ActivateMenu(bool _isActive, GameState _gamestate)
    {
        panel.SetActive(_isActive);

        if (_isActive)
        {
            if (_gamestate == GameState.BeforeLevel)
            {
                //enable initialise canvas
                initLevelCanvas.alpha = 1;
                initLevelCanvas.interactable = true;
                initLevelCanvas.blocksRaycasts = true;

                //disable pause canvas
                pauseCanvas.interactable = false;
                pauseCanvas.blocksRaycasts = false;
                pauseCanvas.alpha = 0;

                // disable afterLevelCanvas canvas
                afterLevelCanvas.interactable = false;
                afterLevelCanvas.blocksRaycasts = false;
                afterLevelCanvas.alpha = 0;

            }
            else if (_gamestate == GameState.Pause)
            {
                // enable pause canvas
                pauseCanvas.alpha = 1;
                pauseCanvas.interactable = true;
                pauseCanvas.blocksRaycasts = true;
                 
                // disable init canvas
                initLevelCanvas.interactable = false;
                initLevelCanvas.blocksRaycasts = false;
                initLevelCanvas.alpha = 0;

                // disable afterLevelCanvas canvas
                afterLevelCanvas.interactable = false;
                afterLevelCanvas.blocksRaycasts = false;
                afterLevelCanvas.alpha = 0;


            }
            else if(_gamestate == GameState.AfterLevel)
            {
                //enable initialise canvas
                afterLevelCanvas.alpha = 1;
                afterLevelCanvas.interactable = true;
                afterLevelCanvas.blocksRaycasts = true;

                //disable pause canvas
                pauseCanvas.interactable = false;
                pauseCanvas.blocksRaycasts = false;
                pauseCanvas.alpha = 0;

                // disable init canvas
                initLevelCanvas.interactable = false;
                initLevelCanvas.blocksRaycasts = false;
                initLevelCanvas.alpha = 0;

            }
        }
        else
        {
            // diable all
            initLevelCanvas.alpha = 0;
            pauseCanvas.alpha = 0;
            pauseCanvas.interactable = false;
            pauseCanvas.blocksRaycasts = false;
            initLevelCanvas.interactable = false;
            initLevelCanvas.blocksRaycasts = false;
            afterLevelCanvas.interactable = false;
            afterLevelCanvas.blocksRaycasts = false;
        }
    }

    public void BeginGame()
    {
        ActivateMenu(false, GameState.BeforeLevel);

        // begin the game
        cameraRotate.TriggerRotation(Direction.Up);
        GameManager.instance.BeginLevel();
    }

    public void ResumeGame()
    {
        cameraRotate.TriggerRotation(Direction.Up);
        GameManager.instance.ResumePlay();
    }


    public void InitNextLevel()
    {
        GameManager.instance.NextLevel();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitToMenu();
    }
}
