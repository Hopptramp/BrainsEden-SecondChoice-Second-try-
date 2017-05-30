using UnityEngine;
using System.Collections;

public class Block_Pushable : BlockData {

    private Coroutine moving = null;
    public bool isMoving { get { return moving != null; } }

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
    
    public void MoveBlock(Vector3 _direction)
    {
        if (moving == null)
        {
            moving = StartCoroutine(Move(_direction, 0.5f));
        }
    }
     
    IEnumerator Move(Vector3 _direction, float _duration)
    {

        Vector3 newPos = transform.position + _direction;
        Vector3 origin = transform.position;
        float t = 0;

        while (t< _duration)
        {
            transform.position = Vector3.Lerp(origin, newPos, t / _duration);
            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        moving = null;
        yield return null;
    }

}
