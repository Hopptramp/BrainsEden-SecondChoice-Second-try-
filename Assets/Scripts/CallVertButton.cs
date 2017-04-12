using UnityEngine;
using System.Collections;

public class CallVertButton : MonoBehaviour {

    private bool pressed = false;
    [Range (-1,1)]
    public int MoveVal;
    Player_Movement player;
	// Use this for initialization
	void Start () {

        player = GameManager.instance.player.GetComponent<Player_Movement>();
	
	}
    public void SetPressed(bool _b)
    {
        pressed = _b; 
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (pressed)
            player.VerticalInput(MoveVal);
	}
}
