using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour
{
	static Compass m_instance;
	static public Compass instance { get { return m_instance; } }

	public GameObject compassOrigin;

	void Awake ()
	{
		m_instance = this;
	}
}
