using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
   [SerializeField] GameObject mainMenu;
   [SerializeField] GameObject optionsMenu;
    GameObject activeMenu;


	public void ResumeGame()
    {
        //tween down
        gameObject.SetActive(false);
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
