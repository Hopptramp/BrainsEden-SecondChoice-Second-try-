using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block_Start : BlockData {

    public override void Initialise()
    {
        GameManager.instance.PlacePlayer(transform.position + Vector3.up);
        base.Initialise();
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        base.BlockLandedOn(_player);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
