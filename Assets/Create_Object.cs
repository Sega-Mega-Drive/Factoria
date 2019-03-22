using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Create_Object : MonoBehaviour {

    public int index;
    public int index2;
    public GameObject game;
    Vector3 vec;
    Quaternion q;

	// Use this for initialization
	void Start () {
        vec = transform.position;
        q = transform.rotation;
	}

    // Update is called once per frame
    void Update()
    {
        if (vec != transform.position)
        {
            transform.position = vec;
            transform.rotation = q;
            List<GameObject> g = GameObject.Find("Cube").GetComponent<Genirator>().elements;
            switch (index)
            {
                case 1:
                    {

                        g.Add(Instantiate(game));
                        GameObject.Find("Cube").GetComponent<Genirator>().elements_index.Add(1);
                        g[g.Count - 1].AddComponent<Ist>();
                        g[g.Count - 1].GetComponent<Ist>().index = index2;
                        g[g.Count - 1].GetComponent<Ist>().me = g[g.Count - 1];
                        break;
                    }
                case 2:
                    {
                        g.Add(Instantiate(game));
                        GameObject.Find("Cube").GetComponent<Genirator>().elements_index.Add(2);
                        break;
                    }
                case 3:
                    {
                        g.Add(Instantiate(game));
                        GameObject.Find("Cube").GetComponent<Genirator>().elements_index.Add(3);
                        break;
                    }
                case 4:
                    {
                        g.Add(Instantiate(game));
                        GameObject.Find("Cube").GetComponent<Genirator>().elements_index.Add(4);
                        g[g.Count - 1].AddComponent<Sk>();
                        break;
                    }

            }
            g[g.Count - 1].transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
            g[g.Count - 1].transform.eulerAngles = new Vector3(0, 0, 0);
            g[g.Count - 1].AddComponent<BoxCollider>();
            g[g.Count - 1].GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            if (GameObject.Find("Cube").GetComponent<Genirator>().elements_index[GameObject.Find("Cube").GetComponent<Genirator>().elements_index.Count - 1] != 2)
            {
                g[g.Count - 1].GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            }
            else
            {
                g[g.Count - 1].GetComponent<BoxCollider>().size = new Vector3(5, 5, 5);
            }
            g[g.Count - 1].AddComponent<put>();
            g[g.Count - 1].AddComponent<Zahvat>();
            g[g.Count - 1].AddComponent<Interactable>();
            g[g.Count - 1].AddComponent<Delite_Componeny>();
            g[g.Count - 1].GetComponent<Delite_Componeny>().me = g[g.Count - 1];
            transform.position = vec;
            transform.rotation = q;

        }
    }
}
