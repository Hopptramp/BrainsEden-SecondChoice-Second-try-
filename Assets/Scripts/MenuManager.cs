using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject levelMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] DataHolder dHolder;

    GameObject activeMenu = null;

	// Use this for initialization
	void Start ()
    {
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
        activeMenu = mainMenu;
	}

    // main menu buttons
    public void StartGame()
    {
        DataHolder.instance.currentLevel = 2;
        SceneManager.LoadScene(2);
    }
    public void ActivateLevelMenu()
    {
        levelMenu.SetActive(true);
        activeMenu.SetActive(false);
        activeMenu = levelMenu;
    }
    public void ActivateOptionsMenu()
    {
        optionsMenu.SetActive(true);
        activeMenu.SetActive(false);
        activeMenu = optionsMenu;
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    // return to main menu
    public void ReturnToMain()
    {
        activeMenu.SetActive(false);
        mainMenu.SetActive(true);
        activeMenu = mainMenu;
    }

    public void StartLevel (int _levelNumber)
    {
        DataHolder.instance.currentLevel = _levelNumber;
        SceneManager.LoadScene(_levelNumber);
    }
    
    
}
