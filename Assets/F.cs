using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F : MonoBehaviour
{
    float x = 0, z = 0;
    bool tr;
    public GameObject me;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void time()
    {
        x = 0; z = 0;
            for (int i = 0; i < GameObject.Find("Cube").GetComponent<Genirator>().elements.Count; i++)
            {
                Debug.Log(GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position);
                if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                {
                    float y = GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.rotation.y;
                    switch ((int)y)
                    {
                        case 90:
                            {
                                z = 0.01f;
                                break;
                            }
                        case 180:
                            {
                                x = 0.01f;
                                break;
                            }
                        case 270:
                            {
                                z = -0.01f;
                                break;
                            }
                        case 0:
                            {
                                x = -0.01f;
                                break;
                            }
                    }
                    tr = true;

                }
            }
            if (tr == false)
            {
                GameObject.Find("Cube (2)").GetComponent<Delity_Must>().game.Add(me);
                //break;
            }
            else
            StartCoroutine("DoMessage");
  
    }

    void Message()
    {
        transform.position = new Vector3(transform.position.x+x, transform.position.y, transform.position.z+z);
    }

    IEnumerator DoMessage()
    {
        for (int b = 0; b < 100; b++)
        {
            Message();
            yield return new WaitForSeconds(0.01f);
        }
        time();
    }
}
