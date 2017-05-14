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

    [SerializeField] StoredCompletionData levelData;
    

    void LoadLevels()
    {
        float count = Resources.LoadAll("").Length;
        print(count);
        for (int i = 0; i < count; ++i)
        {
            string path = "Level-" + i;
            storedLevels.Add((LevelDataScriptable)Resources.Load(path));
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


    public void UpdateChangesToCompletionData(List<LevelDataScriptable> _storedLevels)
    {
        LevelCompletionData[] completion = new LevelCompletionData[_storedLevels.Count];

        for(int i = 0; i < completion.Length; ++i)
        {
            completion[i] = _storedLevels[i].completionData;
        }
        levelData.storedCompletionData = completion;
        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
    }

    public void LoadCompletionData()
    {
        levelData = new StoredCompletionData();
        if (PlayerPrefs.HasKey("StoredLevelData"))
        {
            levelData = JsonUtility.FromJson<StoredCompletionData>(PlayerPrefs.GetString("StoredLevelData"));

            if (levelData.storedCompletionData != null)
            {
                for (int i = 0; i < storedLevels.Count; ++i)
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
        SceneManager.LoadScene("Tutorial 2");

    }



    #endregion
}
