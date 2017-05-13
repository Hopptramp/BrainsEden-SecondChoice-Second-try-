using UnityEngine;
using System.Collections;

public class HUDBlock : GameActors
{
    [SerializeField]
    Transform target;

    [SerializeField]
    GameObject blockObject;

	// Use this for initialization
	void Start () {
        InitDelegates();
	}

    private void LateUpdate()
    {
        transform.position = target.position;
    }

    #region Inherited delegates

    protected override void OnPlayPause(RotationData _rotationData)
    {
        blockObject.SetActive(false);

        // base.OnPlayPause(_rotationData);
    }

    protected override void OnPlayStart(RotationData _rotationData)
    {
        blockObject.SetActive(true);
        //base.OnPlayStart(_rotationData);
    }

    #endregion
}
