using UnityEngine;
using System.Collections;

public class Block_Default : BlockData {

    private BoxCollider col;

    public override void Initialise()
    {

        col = GetComponent<BoxCollider>();
        col.enabled = ActiveInPerspective(CameraState.Front);
        base.Initialise();
    }

    protected override void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        GetComponent<BoxCollider>().enabled = ActiveInPerspective(_rotationData.intendedState);

        base.PostRotationLogic(_rotationData, _isInit);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
