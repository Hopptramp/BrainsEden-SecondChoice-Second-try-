using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {


	// Use this for initialization
	void Start () 
	{
		Invoke ("NextLevel", 1.5f);

	
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void NextLevel()
	{
		SceneManager.LoadScene (1);
	}
}
