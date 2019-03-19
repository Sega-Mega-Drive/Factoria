using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delite_Componeny : MonoBehaviour {

    public GameObject me;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x > 5.15 && transform.position.x < 6.15 && transform.position.z > -0.2 && transform.position.z < 2.8)
        {
            GameObject.Find("Cube (2)").GetComponent<Delity_Must>().game = me;
        }
	}
}
