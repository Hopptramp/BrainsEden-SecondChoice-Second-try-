using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndLevelMenu : MonoBehaviour
{
    [SerializeField] Text jumps;
    [SerializeField] Text flips;
    [SerializeField] Text time;

    public void NextLevel()
    {
        GameManager.instance.EndLevel(false);
    }

    public void RestartLevel()
    {
        GameManager.instance.EndLevel(null);
    }

    public void SetEndValues(float _time, int _flips, int _jumps)
    {
        time.text = _time.ToString("F2") + " Time Taken";
        flips.text = _flips.ToString() + " Flips Flipped";
        jumps.text = _jumps.ToString() + " Jumps Jumped";
    }


}
