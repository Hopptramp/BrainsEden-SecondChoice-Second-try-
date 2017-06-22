using UnityEngine;
using System.Collections;

public class Block_Pushable : BlockData {

    private Coroutine moving = null;
    public bool isMoving { get { return moving != null; } }
    LayerMask obstructionObjects;

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
        if (obstructionObjects != _obstructionObjects)
            obstructionObjects = _obstructionObjects;

        if (Physics.Raycast(transform.position, _direction, 1, obstructionObjects))
        {
            //Obstruction
            return false;
        }
        //Drop
        if (!Physics.Raycast(transform.position + _direction, Vector3.down, 1, obstructionObjects))
            return true;

        //No obstruction
        return true;
    }
    public bool CheckObjectBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1,obstructionObjects))
            return true;
        else
            return false;          
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

        if (!CheckObjectBelow())        
            moving = StartCoroutine(Fall(0.2f));
        else
            moving = null;
        yield return null;
    }
    IEnumerator Fall(float _duration,  float _overflowTime = 0)
    {
        Vector3 origin = transform.position;
        Vector3 newPos = transform.position + Vector3.down;
        float t = _overflowTime;
        
        while(t< _duration)
        {
            transform.position = Vector3.Lerp(origin, newPos, t / _duration);
            yield return null;
            t += Time.deltaTime;
        }
        transform.position = newPos;
        if (!CheckObjectBelow())
            moving = StartCoroutine(Fall(0.2f, t - _duration));
        else
            moving = null;
                
        yield return null;
    }

}
