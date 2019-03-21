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
            for (int i = 0; i < GameObject.Find("Cube").GetComponent<Genirator>().product.Count; i++)
            {
                if (GameObject.Find("Cube").GetComponent<Genirator>().product[i] == game[0])
                {
                    GameObject.Find("Cube").GetComponent<Genirator>().product_index.RemoveAt(i);
                    GameObject.Find("Cube").GetComponent<Genirator>().product.RemoveAt(i);
                }
            }
            Destroy(game[0]);
            game.RemoveAt(0);
            }
	}
}
