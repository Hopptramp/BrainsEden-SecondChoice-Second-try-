using UnityEngine;
using System.Collections;

public class Block_Default : BlockData {

    private BoxCollider col;

    public override void Initialise()
    {
        base.Initialise();
        rend = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider>();
        ToggleActive(ActiveInPerspective(CameraState.Front));
        
    }

    protected override void PreRotationLogic(RotationData _rotationData)
    {
        base.PreRotationLogic(_rotationData);
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        if (ActiveInPerspective(_rotationData.fromState) != ActiveInPerspective(_rotationData.intendedState))
            ToggleActive(ActiveInPerspective(_rotationData.intendedState));   
        base.PostRotationLogic(_rotationData, _isInit);
    }


    void ToggleActive( bool _active)
    {
        col.enabled = _active;
        rend.material = _active? designHolder.material: GameManager.instance.levelManager.GetInactiveBlockMaterial();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
