using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour {

    private Camera _cam;

    public GameObject particles, skybox;

    private Vector3 startScale;

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
    }

    void Orthographic()
    {
        _cam.orthographicSize = 2;
       // skybox.transform.localScale = new Vector3 (10,10,10);
    }


    public void CameraEffectsSwitch(Text button)
    {
        MainMenuContent.fancyFast = !MainMenuContent.fancyFast;
        if (MainMenuContent.fancyFast == true)
        {
            particles.SetActive(true);
            button.text = "Fancy";
        }
        else
        {
            particles.SetActive(false);
            button.text = "Fast";
        }
    }
}
