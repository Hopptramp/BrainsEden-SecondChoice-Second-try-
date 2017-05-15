using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MenuState
{
    Main,
    Options,
    Levels
}

[System.Serializable]
public class StoredCompletionData
{
    public int levelReachedID;
    public LevelCompletionData[] storedCompletionData;
}

public class PersistantManager : MonoBehaviour
{
    public static PersistantManager instance { get; private set; }
    public List<LevelDataScriptable> storedLevels;
    [SerializeField] Text levelName;
    [SerializeField] Text levelStats;
    MenuState menuState;
    private int levelSelectedID;
    private int levelPageActive = 0;
    public int levelReachedID { get; private set; }

    [SerializeField] string gameScene = "MainGame";

    [SerializeField] StoredCompletionData levelData;
    [SerializeField] MainMenuContent menuContent;
    

    void LoadLevels()
    {
        //float count = Resources.LoadAll("").Length;
        Object[] levels = Resources.LoadAll("");
        for (int i = 0; i < levels.Length; ++i)
        {
            storedLevels.Add((LevelDataScriptable)levels[i]);
        }

        //sort them in order of ID
    }

	// Use this for initialization
	void Awake ()
    {
        instance = this;
        DontDestroyOnLoad(this);
        LoadLevels();
        LoadCompletionData();
        //DisplayLevel();
        menuContent.GenerateLevelPages(storedLevels);
    }
	
    public List<LevelDataScriptable> ReturnStoredLevels()
    {
        return storedLevels;
    }


    public void UpdateChangesToCompletionData(List<LevelDataScriptable> _storedLevels, int _levelReachedID)
    {
        LevelCompletionData[] completion = new LevelCompletionData[_storedLevels.Count];

        for(int i = 0; i < completion.Length; ++i)
        {
            completion[i] = _storedLevels[i].completionData;
        }
        levelData.storedCompletionData = completion;
        levelData.levelReachedID = _levelReachedID == -1 ? levelData.levelReachedID : _levelReachedID;
        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
    }

    public void LoadCompletionData()
    {
        levelData = new StoredCompletionData();
        if (PlayerPrefs.HasKey("StoredLevelData"))
        {
            levelData = JsonUtility.FromJson<StoredCompletionData>(PlayerPrefs.GetString("StoredLevelData"));


            for (int i = 0; i < storedLevels.Count; ++i)
            {
                if (i < levelData.storedCompletionData.Length)
                {
                    storedLevels[i].completionData = levelData.storedCompletionData[i];
                }
            }
        }
    }

    public int ReturnLevelID()
    {
        return levelSelectedID;
    }

    public int ReturnNextLevelID()
    {
        ++levelSelectedID;
        if (levelSelectedID == storedLevels.Count)
            return -1;
        return levelSelectedID;
    }

    #region Button Input

    public void SetMenuStateToMain()
    {
        menuState = MenuState.Main;
    }
    public void SetMenuStateToOptions()
    {
        menuState = MenuState.Options;
    }
    public void SetMenuStateToLevels()
    {
        menuState = MenuState.Levels;
        menuContent.FillLevelPage(levelPageActive);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeLevelPage(bool _isNext)
    {
        // iterate through levels  
        levelPageActive += _isNext ? 1 : -1;

        // clamp value
        levelPageActive = Mathf.Clamp(levelPageActive, 0, menuContent.maxPageNumber);

        menuContent.FillLevelPage(levelPageActive);
    }

    public void SelectLevel(int _level)
    {
        print(_level);
        LevelThumbnailData levelData = menuContent.GetLevelDataFromCurrentPage(levelPageActive, _level);
        menuContent.FillSelectedLevelData(levelPageActive, _level);
        levelSelectedID = levelData.levelID;

    }

    //void DisplayLevel()
    //{
    //    levelName.text = storedLevels[levelSelectedID].name;

    //    LevelCompletionData data = storedLevels[levelSelectedID].completionData;
    //    levelStats.text = "flips: " + data.totalFlips + "\n" + "steps: " + data.totalSteps + "\n" + "time: " + data.timeTaken.ToString("00:00") + "\n";
    //}

    public void PlayLevel()
    {
        SceneManager.LoadScene(gameScene);

    }



    #endregion
}
