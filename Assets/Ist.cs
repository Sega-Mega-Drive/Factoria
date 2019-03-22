using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ist : MonoBehaviour {

    public GameObject p;
    public GameObject me;
    public int index;

    // Use this for initialization
    void Start () {

        switch (index)
        {
            case 0:
                {
                    p = GameObject.Find("Cube").GetComponent<Genirator>().k_1;
                    break;
                }
            case 1:
                {
                    p = GameObject.Find("Cube").GetComponent<Genirator>().k_2;
                    break;
                }
            case 2:
                {
                    p = GameObject.Find("Cube").GetComponent<Genirator>().k_3;
                    break;
                }
            case 3:
                {
                    p = GameObject.Find("Cube").GetComponent<Genirator>().k_4;
                    break;
                }
        }

        StartCoroutine("DoMessage");
    }

    IEnumerator DoMessage()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            if (GameObject.Find("Cube").GetComponent<Genirator>().scr == false) continue;
            Message();
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
                n.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                n.AddComponent<F>();
                n.GetComponent<F>().me = n;
                GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(index);
                n.GetComponent<F>().x = -1;
                n.GetComponent<F>().z = 0;
                n.GetComponent<F>().StartCoroutine("DoMessage");
            }

                if (transform.position.x + 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                {
                    n = Instantiate(p);
                    n.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                    n.AddComponent<F>();
                    n.GetComponent<F>().me = n;
                    GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                    GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(index);
                    n.GetComponent<F>().x = 1;
                    n.GetComponent<F>().z = 0;
                    n.GetComponent<F>().StartCoroutine("DoMessage");
                }
                    if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z - 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                    {
                        n = Instantiate(p);
                        n.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                        n.AddComponent<F>();
                        n.GetComponent<F>().me = n;
                        GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                        GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(index);
                        n.GetComponent<F>().x = 0;
                        n.GetComponent<F>().z = -1;
                        n.GetComponent<F>().StartCoroutine("DoMessage");
                    }
                        if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z + 1 == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                        {
                            n = Instantiate(p);
                            n.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                            n.AddComponent<F>();
                            n.GetComponent<F>().me = n;
                            GameObject.Find("Cube").GetComponent<Genirator>().product.Add(n);
                            GameObject.Find("Cube").GetComponent<Genirator>().product_index.Add(index);
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
