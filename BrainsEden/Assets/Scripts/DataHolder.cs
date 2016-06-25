using UnityEngine;
using System.Collections;

public class DataHolder : MonoBehaviour {

    static DataHolder m_instance;
    static public DataHolder instance { get { return m_instance; } }

    public int currentLevel = 0;

	// Use this for initialization
	void Start () {

        m_instance = this;
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
