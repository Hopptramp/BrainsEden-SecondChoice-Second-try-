using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject PauseButton;
    GameObject activeMenu;

    bool active = false;

    public void Pause()
    {
        mainMenu.SetActive(!active);
        //active = !active;
        PauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        //tween down
        mainMenu.SetActive(false);
        PauseButton.SetActive(true);
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        activeMenu.SetActive(false);
        activeMenu = optionsMenu;
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        activeMenu.SetActive(false);
        activeMenu = optionsMenu;
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
