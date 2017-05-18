using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block_Moving: BlockData {

    public Vector3 destination = Vector3.zero;
    public float moveSpeed = 1;

    public override void Initialise()
    {
        StartCoroutine(MoveBlock());
        base.Initialise();        
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        base.PostRotationLogic(_rotationData, _isInit);
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        _player.transform.parent = transform;

        base.BlockLandedOn(_player);
    }

    /// <summary>
    /// Recursive function that moves from initial position to a target position and back.
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveBlock()
    {
        transform.position = localPosition;
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        while (transform.position != localPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, localPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(MoveBlock());

    }


}
