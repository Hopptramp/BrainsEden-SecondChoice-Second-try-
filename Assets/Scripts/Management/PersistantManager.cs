using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private int levelSelectedID;
    private int levelPageActive = 0;
    public int levelReachedID; /*{ get; private set; }*/

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
        SortLevels();
    }

    void SortLevels()
    {
        LevelDataScriptable[] temp = new LevelDataScriptable[storedLevels.Count];

        for(int i = 0; i < temp.Length; ++i)
        {
            temp[i] = storedLevels[i];
        }

        System.Array.Sort(temp, delegate (LevelDataScriptable x, LevelDataScriptable y) { return x.levelID.CompareTo(y.levelID); });

        for (int i = 0; i < temp.Length; ++i)
        {
            storedLevels[i] = temp[i];
        }

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
        levelReachedID = levelData.levelReachedID;
        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
    }

    public void LoadCompletionData()
    {
        levelData = new StoredCompletionData();
        if (PlayerPrefs.HasKey("StoredLevelData"))
        {
            levelData = JsonUtility.FromJson<StoredCompletionData>(PlayerPrefs.GetString("StoredLevelData"));
            levelReachedID = levelData.levelReachedID;

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
        levelSelectedID = _level;

    }

    //void DisplayLevel()
    //{
    //    levelName.text = storedLevels[levelSelectedID].name;

    //    LevelCompletionData data = storedLevels[levelSelectedID].completionData;
    //    levelStats.text = "flips: " + data.totalFlips + "\n" + "steps: " + data.totalSteps + "\n" + "time: " + data.timeTaken.ToString("00:00") + "\n";
    //}

    public void PlayLevel()
    {
        if(levelSelectedID <= levelReachedID + 1)
          SceneManager.LoadScene(gameScene);
    }

    public void ResetSavedData()
    {
        levelData.levelReachedID = 0;

        Object[] levels = AssetDatabase.LoadAllAssetsAtPath("");
        for(int i = 0; i < levelData.storedCompletionData.Length; ++i)
        {
            levelData.storedCompletionData[i] = new LevelCompletionData();
            LevelDataScriptable scriptable = (LevelDataScriptable)levels[i];
            scriptable.completionData = new LevelCompletionData();
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(scriptable);
        }
        

        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
    }

    //AssetDatabase.CreateAsset(asset, "Assets/Resources/" + _fileName + ".asset");

    //    // APPLY VARIABLES HERE
    //    //asset.levelID = _id;
    //    asset.storedBlocks = _stored;
    //    AssetDatabase.SaveAssets();
    //    EditorUtility.SetDirty(asset);


#endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(PersistantManager))]
public class PersistantManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PersistantManager level = (PersistantManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset level completion data"))
        {
            level.ResetSavedData();
        }
    }
}

#endif