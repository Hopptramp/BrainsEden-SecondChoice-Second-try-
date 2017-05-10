using UnityEngine;
using System.Collections;

public class RegisterTouchInput : MonoBehaviour {

    [HideInInspector] public PlayerController controller;
    public Direction direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnMouseDown()
    {
        switch (direction)
        {
            case Direction.Up:
                controller.MoveVertical(true);
                break;
            case Direction.Down:
                controller.MoveVertical(false);
                break;
            case Direction.Left:
                controller.MoveHorizontal(true);
                break;
            case Direction.Right:
                controller.MoveHorizontal(false);
                break;
            default:
                break;
        }
    }
}
