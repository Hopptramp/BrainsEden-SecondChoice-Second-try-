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
    public int levelReachedID { get; private set; }

    [SerializeField] string gameScene = "MainGame";

    [SerializeField] StoredCompletionData levelData;
    

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
        DisplayLevel();
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
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeLevel(bool _isNext)
    {
        // iterate through levels  
        levelSelectedID += _isNext ? 1 : -1;

        // clamp value
        levelSelectedID = Mathf.Clamp(levelSelectedID, 0, storedLevels.Count - 1);
        DisplayLevel();
    }

    void DisplayLevel()
    {
        levelName.text = storedLevels[levelSelectedID].name;

        LevelCompletionData data = storedLevels[levelSelectedID].completionData;
        levelStats.text = "flips: " + data.totalFlips + "\n" + "steps: " + data.totalSteps + "\n" + "time: " + data.timeTaken.ToString("00:00") + "\n";
    }

    public void PlayLevel()
    {
        SceneManager.LoadScene(gameScene);

    }



    #endregion
}
