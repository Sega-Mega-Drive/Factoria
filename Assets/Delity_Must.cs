using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delity_Must : MonoBehaviour {

    public List<GameObject> game;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
            while (game.Count != 0)
            {
                Destroy(game[0]);
            game.RemoveAt(0);
            }
	}
}
