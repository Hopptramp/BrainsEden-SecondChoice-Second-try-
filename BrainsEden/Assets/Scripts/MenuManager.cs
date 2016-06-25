﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject levelMenu;
    [SerializeField] GameObject optionsMenu;

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
        SceneManager.LoadScene(1);
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
        SceneManager.UnloadScene(0);
    }

    // return to main menu
    public void ReturnToMain()
    {
        activeMenu.SetActive(false);
        mainMenu.SetActive(true);
        activeMenu = mainMenu;
    }


    // Loading the levels
    public void StartLevelOne()
    {
        SceneManager.LoadScene(1);
    }
    public void StartLevelTwo()
    {
        SceneManager.LoadScene(2);
    }
    public void StartLevelThree()
    {
        SceneManager.LoadScene(3);
    }
    
    
}