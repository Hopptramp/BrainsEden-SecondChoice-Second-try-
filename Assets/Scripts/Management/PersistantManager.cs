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
    [SerializeField] Text fpsText;
    MenuState menuState;
    [SerializeField] private int levelSelectedID = 0;
    public int levelPageActive = 0;
    public int levelReachedID = 0; /*{ get; private set; }*/

    public string gameScene = "MainGame";

    [SerializeField] StoredCompletionData levelData;
    [HideInInspector] public MainMenuContent menuContent;

    private GameObject[] canvases;
    

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
        if (instance)
        {
            if (instance != this)
            {
                Destroy(this);
            }
            return;
        }
        else
            instance = this;

        DontDestroyOnLoad(this);
        LoadLevels();
        LoadCompletionData();
        //DisplayLevel();
        menuContent.GenerateLevelPages(storedLevels);
    }

    private void Update()
    {
        fpsText.text = "PFS : " + (1 / Time.deltaTime).ToString("f1");
    }

    public void MenuInit(MainMenuContent _content)
    {
        menuContent = _content;
        menuContent.GenerateLevelPages(storedLevels);
        menuContent.FillLevelPage(levelPageActive);
    }

    public bool IsMainScene()
    {
        return SceneManager.GetActiveScene().name == gameScene;
    }

    #region loading/saving

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
        levelData.levelReachedID = levelReachedID == -1 ? levelData.levelReachedID : levelReachedID;
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

    #endregion

    #region Level

    public int ReturnLevelID()
    {
        return levelSelectedID;
    }

    public int ReturnNextLevelID()
    {
        ++levelSelectedID;
        if (levelReachedID < levelSelectedID)
            levelReachedID = levelSelectedID;
        if (levelSelectedID == storedLevels.Count)
            return -1;
        return levelSelectedID;
    }



    #endregion

    #region Button Input


    public void SelectLevel(int _level)
    {
        LevelThumbnailData levelData = menuContent.GetLevelDataFromCurrentPage(levelPageActive, _level);
        menuContent.FillSelectedLevelData(levelPageActive, _level);
        levelSelectedID = _level;
        ActivateGraphicRaycasters(3);
    }

    public void SetMenuStateToMain()
    {
        menuState = MenuState.Main;
        levelPageActive = 0;
        menuContent.GenerateLevelPages(storedLevels);
        ActivateGraphicRaycasters(0);
    }
    public void SetMenuStateToOptions()
    {
        menuState = MenuState.Options;
        ActivateGraphicRaycasters(1);
    }
    public void SetMenuStateToLevels()
    {
        menuState = MenuState.Levels;
        ActivateGraphicRaycasters(2);
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

    /// <summary>
    /// Activate/Deactivate graphics raycaster components on world space
    /// canvases. This will improve frame rate during event system use (buttons)
    /// </summary>
    /// <param name="_id"> 0 = Main, 1 = Options, 2 = Levels, 3 = Selected Level </param>
    public void ActivateGraphicRaycasters(int _id)
    {
        if (canvases == null)
            canvases = menuContent.canvases;

        for(int i = 0; i < canvases.Length; ++i)
        {
            if(i == _id)
                canvases[i].SetActive(true);
            else
                canvases[i].SetActive(false);
        }
    }

    public void PlayLevel()
    {
        if(levelSelectedID <= levelReachedID)
          SceneManager.LoadScene(gameScene);
    }

    public void ResetMobileSavedData()
    {
        levelData.storedCompletionData = new LevelCompletionData[storedLevels.Count];
        levelData.levelReachedID = 0;
        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
        menuContent.GenerateLevelPages(storedLevels);
        storedLevels.Clear();
        Resources.UnloadUnusedAssets();
        LoadLevels();
    }

#if UNITY_EDITOR
    public void ResetSavedData()
    {
        levelData.levelReachedID = 0;

        LevelDataScriptable[] levels = (LevelDataScriptable[])Resources.LoadAll<LevelDataScriptable>("");
        for(int i = 0; i < levels.Length; ++i)
        {
            levels[i].completionData = new LevelCompletionData();
            levels[i].completionData = new LevelCompletionData();
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(levels[i]);
        }
        

        PlayerPrefs.SetString("StoredLevelData", JsonUtility.ToJson(levelData));
    }
#endif
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