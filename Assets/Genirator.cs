using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Valve.VR.InteractionSystem;

public class Genirator : MonoBehaviour
{
    public List<GameObject> elements = new List<GameObject>();
    public List<int> elements_index = new List<int>();
    public List<int[]> elements_dat = new List<int[]>();
    public List<GameObject> product = new List<GameObject>();
    public List<int> product_index = new List<int>();
    public int[] product_summ = new int[14];
    public GameObject p1_1;
    public GameObject p1_2;
    public GameObject p1_3;
    public GameObject p1_4;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject k_1;
    public GameObject k_2;
    public GameObject k_3;
    public GameObject k_4;
    public int sim;
    public int count;
    public bool scr;
    // Start is called before the first frame update
    void Start()
    {
        scr = true;
        count = 1;
        Generator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generator()
    {
        scr = true;
        GameObject.Find("New Text (1)").GetComponent<TextMesh>().text = "True";
        string s;
        if (count < 10) s = "0" + count;
        else s = "" + count;
        FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\input" + s +".txt", FileMode.Open);
        StreamReader reader = new StreamReader(file);
        for (int b = 0; b < 4; b++)
        {
            int r = Convert.ToInt32(reader.ReadLine());
            for (int i = 0; i < r; i++)
            {
                string[] dat = reader.ReadLine().Split(' ');
                int[] arr;
                switch(b)
                {
                    case 0:
                        {
                            arr = new int[1];
                            arr[0] = Convert.ToInt32( dat[2]);
                            Load(Convert.ToSingle(dat[0]), Convert.ToSingle(dat[1]), 1, arr, 0);
                            break;
                        }
                    case 1:
                        {
                            arr = new int[3];
                            arr[0] = Convert.ToInt32(dat[2]);
                            arr[1] = Convert.ToInt32(dat[3]);
                            arr[2] = Convert.ToInt32(dat[4]);
                            Debug.Log(Convert.ToInt32(Convert.ToSingle(dat[5])));
                            int rot = Convert.ToInt32(Convert.ToSingle(dat[5]));
                            if (rot == 1) rot = 180;
                            else
                            {
                                if (rot == -1) rot = 0;
                                else
                                {
                                    rot = Convert.ToInt32(Convert.ToSingle(dat[6]));
                                    if (rot == 1) rot = 90;
                                    else
                                    {
                                        rot = 270;
                                    }
                                }
                            }
                            Load(Convert.ToSingle(dat[0]), Convert.ToSingle(dat[1]), 2, arr, rot);
                            break;
                        }
                    case 2:
                        {
                            int rot = Convert.ToInt32(Convert.ToSingle(dat[2]));
                            if (rot == 1) rot = 180;
                            else
                            {
                                if (rot == -1) rot = 0;
                                else
                                {
                                    rot = Convert.ToInt32(Convert.ToSingle(dat[3]));
                                    if (rot == 1) rot = 90;
                                    else
                                    {
                                        rot = 270;
                                    }
                                }
                            }
                            Load(Convert.ToSingle(dat[0]), Convert.ToSingle(dat[1]), 3, null, rot);
                            break;
                        }
                    case 3:
                        {
                            Load(Convert.ToSingle(dat[0]), Convert.ToSingle(dat[1]), 4, null, 0);
                            break;
                        }
                }
            }
        }

        sim = Convert.ToInt32(reader.ReadLine());
        reader.Close();
        file.Close();
        StartCoroutine("DoMessage");
    }

    IEnumerator DoMessage()
    {
        if (sim != -1)
        {
            yield return new WaitForSeconds(sim);
            scr = false;
            GameObject.Find("New Text (1)").GetComponent<TextMesh>().text = "False";
        }
    }


    void Load(float x, float z, int name, int[] dat, float rot)
    {
        switch(name)
        {
            case 1:
                {
                    int index = 0;
                    GameObject buf = p1_1;
                    index = dat[0];
                    switch (dat[0])
                    {
                        case 0:
                            {
                                buf = p1_1;
                                break;
                            }
                        case 1:
                            {
                                buf = p1_2;
                                break;
                            }
                        case 2:
                            {
                                buf = p1_3;
                                break;
                            }
                        case 3:
                            {
                                buf = p1_4;
                                break;
                            }
                    }
                    elements.Add(Instantiate(buf));
                    elements_index.Add(1);
                    elements[elements.Count - 1].AddComponent<Ist>();
                    elements[elements.Count - 1].GetComponent<Ist>().index = index;
                    elements[elements.Count - 1].GetComponent<Ist>().me = elements[elements.Count - 1];
                    break;
                }
            case 2:
                {
                    elements.Add(Instantiate(p2));
                    elements_index.Add(2);
                    break;
                }
            case 3:
                {
                    elements.Add(Instantiate(p3));
                    elements_index.Add(3);
                    break;
                }
            case 4:
                {
                    elements.Add(Instantiate(p4));
                    elements_index.Add(4);
                    elements[elements.Count - 1].AddComponent<Sk>();
                    break;
                }

        }
        elements_dat.Add(dat);
        elements[elements.Count - 1].transform.position = new Vector3(x, 0.5f, z);
        elements[elements.Count - 1].transform.eulerAngles = new Vector3(0, rot, 0);
        elements[elements.Count - 1].AddComponent<BoxCollider>();
        elements[elements.Count - 1].GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
        if (elements_index[elements_index.Count - 1] != 2)
        {
            elements[elements.Count - 1].GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
        }
        else
        {
            elements[elements.Count - 1].GetComponent<BoxCollider>().size = new Vector3(5, 5, 5);
        }
        elements[elements.Count - 1].AddComponent<put>();
        elements[elements.Count - 1].AddComponent<Zahvat>();
        elements[elements.Count - 1].AddComponent<Interactable>();
        elements[elements.Count - 1].AddComponent<Delite_Componeny>();
        elements[elements.Count - 1].GetComponent<Delite_Componeny>().me = elements[elements.Count - 1];

    }
}
