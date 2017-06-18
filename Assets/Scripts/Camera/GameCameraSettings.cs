using UnityEngine;

public class GameCameraSettings : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        CameraSwitch _camSettings = FindObjectOfType<CameraSwitch>();
        _camSettings.LoadLevelCheck();
	}
}
