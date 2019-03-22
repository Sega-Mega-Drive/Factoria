using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class F : MonoBehaviour
{
    public float x = 0, z = 0;
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
        
            for (int i = 0; i < GameObject.Find("Cube").GetComponent<Genirator>().elements.Count; i++)
            {

                x = (float)(((float)((int)(transform.position.x * 10))) / 10);
                z = (float)(((float)((int)(transform.position.z * 10))) / 10);
                transform.position = new Vector3(Convert.ToSingle(x), transform.position.y, Convert.ToSingle(z));
                x = 0; z = 0;
                if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                {
                    float y = GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.eulerAngles.y;
                    switch ((int)y)
                    {
                        case 90:
                            {
                                z = -1f;
                                break;
                            }
                        case 180:
                            {
                                x = -1f;
                                break;
                            }
                        case 270:
                            {
                                z = 1f;
                                break;
                            }
                        case 0:
                            {
                                x = 1f;
                                break;
                            }
                    }
                    tr = true;
                    break;
                }
            }
            if (tr == false)
            {
            me.AddComponent<Rigidbody>();
                GameObject.Find("Cube (2)").GetComponent<Delity_Must>().game.Add(me);
                //break;
            }
            else
                tr = false;
            StartCoroutine("DoMessage");
  
    }

    void Message()
    {
        transform.position = new Vector3(transform.position.x+x, transform.position.y, transform.position.z+z);
        Debug.Log(transform.position);
    }

    public IEnumerator DoMessage()
    {
        bool tram = false;
        for (int b = 0; b < 2; b++)
        {
            if (tram == true) break;
            yield return new WaitForSeconds(1f);
            if (GameObject.Find("Cube").GetComponent<Genirator>().scr == false) continue;
            Message();
            for (int i = 0; i < GameObject.Find("Cube").GetComponent<Genirator>().elements.Count; i++)
            {
                float lol = (float)(((float)((int)(transform.position.x * 10))) / 10);
                float lol2 = (float)(((float)((int)(transform.position.z * 10))) / 10);
                transform.position = new Vector3(Convert.ToSingle(lol), transform.position.y, Convert.ToSingle(lol2));
                if (transform.position.x == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.x && transform.position.z == GameObject.Find("Cube").GetComponent<Genirator>().elements[i].transform.position.z)
                {
                    Debug.Log("true");
                    tram = true;
                }
            }
        }
        time();
    }
}
