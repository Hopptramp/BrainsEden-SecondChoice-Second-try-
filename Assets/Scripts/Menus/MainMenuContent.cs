using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct LevelThumbnailData
{
    public string levelName;
    //public string levelStats; // to be changed to "Star scoring" system
    public Sprite[] stars;
    public int levelID;
    public bool isComplete;
    public bool hasContent;
}

[System.Serializable]
public struct LevelThumbnailPhysical
{
    public Text levelName;
    //public Text levelStats;
    public Image[] stars;
    public GameObject icon;
}




[System.Serializable] // for debugging
public struct LevelPage
{
    public LevelThumbnailData[] levelThumbnails;
}

public class MainMenuContent : MonoBehaviour
{
    public GameObject[] canvases;
    [SerializeField] LevelPage[] levelPages;
    [SerializeField] LevelThumbnailPhysical[] physicalThumbnails;
    public int maxPageNumber = 0;
    [SerializeField] Color[] activeStateColour;
    [SerializeField] Sprite[] starState;

    [SerializeField] LevelThumbnailPhysical selectedThumbnail;

    public static bool persepectiveView = true;
    public static bool fancyFast = true;

    public GameObject ambient;

    private void Start()
    {
        PersistantManager.instance.MenuInit(this);
        persepectiveView = true;
    }

    /// <summary>
    /// Generate the total number of levels + assign content
    /// </summary>
    /// <param name="_levelCount"></param>
    public void GenerateLevelPages(List<LevelDataScriptable> storedLevels)
    {
        if (levelPages.Length > 0)
            return;

        levelPages = new LevelPage[Mathf.FloorToInt((storedLevels.Count / 9) + 1)]; // probably wrong
        maxPageNumber = levelPages.Length - 1;

        for(int j = 0; j < levelPages.Length; ++j)
        {
            levelPages[j].levelThumbnails = new LevelThumbnailData[9];

            for(int i = 0; i < levelPages[j].levelThumbnails.Length; ++i)
            {
                int levelCount = j * 9 + i;
                if (storedLevels.Count <= levelCount)
                {
                    levelPages[j].levelThumbnails[i].hasContent = false;
                    break;
                }
                levelPages[j].levelThumbnails[i].levelID = storedLevels[levelCount].levelID;

                LevelCompletionData data = storedLevels[levelCount].completionData;
                levelPages[j].levelThumbnails[i].levelName = storedLevels[levelCount].name;
                levelPages[j].levelThumbnails[i].isComplete = data.hasCompleted;
                levelPages[j].levelThumbnails[i].hasContent = true;

                levelPages[j].levelThumbnails[i].stars = new Sprite[3];
                for(int k = 1; k < 4; ++k)
                {
                    levelPages[j].levelThumbnails[i].stars[k - 1] = starState[data.stars >= k ? 0 : 1];
                }
            }
        }
    }

 

    /// <summary>
    /// Fill the physical thumnails with the content of the levelpage passed in
    /// </summary>
    /// <param name="_pageNumber"></param>
    public void FillLevelPage(int _pageNumber)
    {
        for(int i = 0; i < physicalThumbnails.Length; ++i)
        {
            if (levelPages[_pageNumber].levelThumbnails[i].hasContent)
            {
                physicalThumbnails[i].levelName.text = levelPages[_pageNumber].levelThumbnails[i].levelName;
                for (int k = 0; k < 3; ++k)
                {
                    physicalThumbnails[i].stars[k].sprite = levelPages[_pageNumber].levelThumbnails[i].stars[k];
                }
                physicalThumbnails[i].icon.SetActive(true);

                if(levelPages[_pageNumber].levelThumbnails[i].levelID <= PersistantManager.instance.levelReachedID)
                    physicalThumbnails[i].icon.GetComponentInChildren<Image>().color = activeStateColour[0];
                else
                    physicalThumbnails[i].icon.GetComponentInChildren<Image>().color = activeStateColour[1];                

            }
            else
                physicalThumbnails[i].icon.SetActive(false);
            
        }
    }

   

    /// <summary>
    /// fill the physical thumbnail of the selected level (top side of the cube)
    /// </summary>
    /// <param name="_pageNumber"></param>
    /// <param name="_level"></param>
    public void FillSelectedLevelData(int _pageNumber, int _level)
    {
        selectedThumbnail.levelName.text = levelPages[_pageNumber].levelThumbnails[_level].levelName;
        for (int k = 0; k < 3; ++k)
        {
            selectedThumbnail.stars[k].sprite = levelPages[_pageNumber].levelThumbnails[_level].stars[k];
        }
    }

    /// <summary>
    /// Return the data of the specific level 
    /// </summary>
    /// <param name="_pageNumber"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public LevelThumbnailData GetLevelDataFromCurrentPage(int _pageNumber, int _level)
    {
        return levelPages[_pageNumber].levelThumbnails[_pageNumber];
    }


}
