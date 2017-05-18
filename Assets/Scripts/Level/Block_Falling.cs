using UnityEngine;
using System.Collections;

public class Block_Falling : BlockData {

    public int startingHealth = 3;
    private int currentHealth;

    public override void Initialise()
    {
        gameObject.SetActive(true);
        currentHealth = startingHealth;
        base.Initialise();
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        base.PostRotationLogic(_rotationData, _isInit);
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {
        currentHealth--;
        if (currentHealth <= 0)
            StartCoroutine(RemoveBlock(_player, 1.5f));
        base.BlockLandedOn(_player);
    }

    IEnumerator RemoveBlock(FixedPlayerMovement _player, float _length)
    {

        float t = 0;
        while (t <= _length)
        {

            t += Time.deltaTime;
            yield return null;
        }
        yield return null;
        gameObject.SetActive(false);
        _player.OnMovementComplete();
    }

}
