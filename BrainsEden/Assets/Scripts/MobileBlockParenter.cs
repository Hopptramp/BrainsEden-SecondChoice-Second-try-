using UnityEngine;
using System.Collections;

public class MobileBlockParenter : MonoBehaviour
{
	[SerializeField]
	private Transform block;

	private Transform player;

	private void Start()
	{
		if (!block)
		{
			block = transform.parent;
		}
	}

	private void OnTriggerEnter(Collider otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player")
		{
			player = otherCollider.gameObject.transform;

			player.SetParent (block);
		}
	}

	private void OnTriggerExit(Collider otherCollider)
	{
		if (otherCollider.gameObject.transform == player)
		{
			player.SetParent (null);
		}
	}
}
