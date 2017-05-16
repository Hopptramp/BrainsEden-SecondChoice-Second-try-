using UnityEngine;
using System.Collections;

public class PersistentButtonListener : MonoBehaviour {

    PersistantManager persistentManager;
	// Use this for initialization
	void Start () {
        persistentManager = PersistantManager.instance;
	}

    public void SetMenuStateToMain()
    {
        persistentManager.SetMenuStateToMain();
    }
    public void SetMenuStateToOptions()
    {
        persistentManager.SetMenuStateToOptions();
    }
    public void SetMenuStateToLevels()
    {
        persistentManager.SetMenuStateToLevels();
    }
    public void Quit()
    {
        persistentManager.Quit();
    }

    public void SelectLevel(int _level)
    {
        persistentManager.SelectLevel(_level);
    }

    public void ChangeLevelPage(bool _isNext)
    {
        persistentManager.ChangeLevelPage(_isNext);
    }

    public void PlayLevel()
    {
        persistentManager.PlayLevel();
    }
}
