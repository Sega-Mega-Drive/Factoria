using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delity_Must : MonoBehaviour {

    public GameObject game;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (game != null)
        {
            Destroy(game);
        }
	}
}
