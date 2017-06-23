using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour {

    private Camera _cam;

    public GameObject ambient, skybox;

    private Vector3 startScale;

    public GameObject orthoHud, perspHud;

    public bool fancy, perspective;

    

	// Use this for initialization
	void Start () {
        _cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CameraSwitcher(Text button)
    {
        MainMenuContent.persepectiveView = !MainMenuContent.persepectiveView;
        if (MainMenuContent.persepectiveView == true)
        {
            _cam.orthographic = false;
            button.text = "Perspective";
            Perspective();
        }
        else
        {
            _cam.orthographic = true;
            button.text = "Orthographic";
            Orthographic();
        }
    }

    void Perspective()
    {
        // skybox.transform.localScale = startScale;
        perspective = true;
    }

    void Orthographic()
    {
        _cam.orthographicSize = 2;
        perspective = false;
       // skybox.transform.localScale = new Vector3 (10,10,10);
    }


    public void CameraEffectsSwitch(Text button)
    {
        MainMenuContent.fancyFast = !MainMenuContent.fancyFast;
        if (MainMenuContent.fancyFast == true)
        {
            ambient.SetActive(true);
            fancy = true;

            button.text = "Fancy";
        }
        else
        {
            ambient.SetActive(false);
            button.text = "Fast";
            fancy = false;
        }
    }

    public void LoadLevelCheck()
    {
        
        _cam = Camera.main;


        bool isMainScene = PersistantManager.instance.IsMainScene();

        if (isMainScene)
        {
            orthoHud = GameManager.instance._orthoHud;
            perspHud = GameManager.instance._perspHud;
            ambient = GameManager.instance.ambient;
        }
        else
            ambient = PersistantManager.instance.menuContent.ambient;

        if (perspective == true)
        {
            _cam.orthographic = false;
            if (isMainScene)
            {
                orthoHud.SetActive(false);
                perspHud.SetActive(true);
            }
        }
        else
        {
            _cam.orthographic = true;
            if (isMainScene)
            {
                orthoHud.SetActive(true);
                perspHud.SetActive(false);
            }
        }

        if (fancy == true)
        {
            ambient.SetActive(true);
        }
        else
        {
            ambient.SetActive(false);
        }
    }
}
