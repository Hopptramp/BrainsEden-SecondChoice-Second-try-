using UnityEngine;
using System.Collections;

public class Block_Pushable : BlockData {

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        base.BlockLandedOn(_player);
    }      

    public bool CanBePushed(Vector3 _direction, LayerMask _obstructionObjects)
    {
        if (Physics.Raycast(transform.position, _direction, 1, _obstructionObjects))
        {
            //Obstruction
            return false;
        }
        //Drop
        if (!Physics.Raycast(transform.position + _direction, Vector3.down, 1, _obstructionObjects))
            return false;

        //No obstruction
        return true;
    }
}
