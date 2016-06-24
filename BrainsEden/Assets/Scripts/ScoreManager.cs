using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public float timerValue;
    public Text timerText;
    public Text numberOfJumps;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        timerValue += 1 * Time.deltaTime;
        timerText.text = "Time - " + timerValue.ToString("F2");
	}

    public void TimerReset()
    {
        timerValue = 0;
    }

    public void JumpsReset()
    {
 
    }

    public void IncreaseJumps(float jumps)
    {
        numberOfJumps.text = "Jumps" + jumps.ToString();
    }

}
