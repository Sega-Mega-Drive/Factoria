using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ist : MonoBehaviour {

    public GameObject p;
    public GameObject me;
    int index;

    // Use this for initialization
    void Start () {
		for(int i = 0; i<GameObject.Find("Cube").GetComponent<Genirator>().elements.Count; i++)
        {
            if(GameObject.Find("Cube").GetComponent<Genirator>().elements[i]==me)
            {
                index = i;
            }
        }
        StartCoroutine("DoMessage");
    }

    IEnumerator DoMessage()
    {
        while(true)
        {
            Message();
            yield return new WaitForSeconds(2f);
            if (GameObject.Find("Cube").GetComponent<Genirator>().scr == false) break;
        }
    }

    void Message()
    {
        for (int i = 0; i < GameObject.Find("Cube").GetComponent<Genirator>().elements.Count; i++)
        {
            GameObject n;
            if (transform.position.x - 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
            {
                n = Instantiate(p);
                n.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                n.AddComponent<F>();
                n.GetComponent<F>().me = n;
                GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(1);
                n.GetComponent<F>().x = -1;
                n.GetComponent<F>().z = 0;
                n.GetComponent<F>().StartCoroutine("DoMessage");
            }

                if (transform.position.x + 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                {
                    n = Instantiate(p);
                    n.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                    n.AddComponent<F>();
                    n.GetComponent<F>().me = n;
                    GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                    GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(1);
                    n.GetComponent<F>().x = 1;
                    n.GetComponent<F>().z = 0;
                    n.GetComponent<F>().StartCoroutine("DoMessage");
                }
                    if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z - 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                    {
                        n = Instantiate(p);
                        n.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                        n.AddComponent<F>();
                        n.GetComponent<F>().me = n;
                        GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                        GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(1);
                        n.GetComponent<F>().x = 0;
                        n.GetComponent<F>().z = -1;
                        n.GetComponent<F>().StartCoroutine("DoMessage");
                    }
                        if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z + 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                        {
                            n = Instantiate(p);
                            n.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                            n.AddComponent<F>();
                            n.GetComponent<F>().me = n;
                            GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                            GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(1);
                            n.GetComponent<F>().x = 0;
                            n.GetComponent<F>().z = 1;
                            n.GetComponent<F>().StartCoroutine("DoMessage");
                        }
                
            
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
