﻿using UnityEngine;
using System.Collections;


[System.Serializable]
public class Block_End : BlockData {

    public override void Initialise()
    {
        base.Initialise();

        ParticleSystem particles = Instantiate(designHolder.particles, transform) as ParticleSystem;
        particles.transform.localPosition = new Vector3(0,1.5f,0);
    }

    public override void BlockLandedOn(FixedPlayerMovement _player)
    {        
        base.BlockLandedOn(_player);

        GameManager.instance.CompleteLevel();
    }
}
