using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sk : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        List<GameObject> mass = GameObject.Find("Cube").GetComponent<Genirator>().product;
        for (int i = 0; i< mass.Count; i++)
        {
            if (transform.position.x == mass[i].transform.position.x && transform.position.z == mass[i].transform.position.z)
            {
                Destroy(mass[i]);
                GameObject.Find("Cube").GetComponent<Genirator>().product_summ[GameObject.Find("Cube").GetComponent<Genirator>().product_index[i]]++;
                GameObject.Find("Cube").GetComponent<Genirator>().product_index.RemoveAt(i);
                GameObject.Find("Cube").GetComponent<Genirator>().product.RemoveAt(i);
            }
        }
	}
}
