using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    static ScoreManager m_instance;
    static public ScoreManager instance { get { return m_instance; } }

    public float timerValue = 0;
    public float jumpValue = 0;
    public float flipValue = 0;

    public Text timerText;
    public Text jumpText;
    public Text flipText;


    public bool runUpdate = true;

	// Use this for initialization
	void Start () 
    {
        m_instance = this;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (runUpdate)
        {
            timerValue += 1 * Time.deltaTime;
            timerText.text = "Time - " + timerValue.ToString("F2");
        }
	}

    public void TimerReset()
    {
        timerValue = 0;
    }

    public void JumpsReset()
    {
        
    }

    public void IncreaseJumps()
    {
        ++jumpValue;
        jumpText.text = "Jumps " + jumpValue.ToString();
    }

    public void IncreaseFlips()
    {
        ++flipValue;
        //flipText.text = "Flips " + flipValue.ToString();
    }
}
