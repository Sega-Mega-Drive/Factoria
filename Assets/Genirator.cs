using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Valve.VR.InteractionSystem;

public class Genirator : MonoBehaviour
{
    public List<GameObject> elements = new List<GameObject>();
    public GameObject p1_1;
    public GameObject p1_2;
    public GameObject p1_3;
    public GameObject p1_4;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject k;
    public int sim;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
        count = 1;
        Generator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generator()
    {
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
                            if (rot == 1) rot = 0;
                            else
                            {
                                if (rot == -1) rot = 180;
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
                            if (rot == 1) rot = 0;
                            else
                            {
                                if (rot == -1) rot = 180;
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
        GameObject g = Instantiate(k);
        g.transform.position = new Vector3(3.5f, 1.5f, 4.5f);
        g.AddComponent<F>();
        g.GetComponent<F>().time();

    }

    void Load(float x, float z, int name, int[] dat, float rot)
    {
        switch(name)
        {
            case 1:
                {
                    GameObject buf = p1_1;
                    switch(dat[0])
                    {
                        case 1:
                            {
                                buf = p1_1;
                                break;
                            }
                        case 2:
                            {
                                buf = p1_2;
                                break;
                            }
                        case 3:
                            {
                                buf = p1_3;
                                break;
                            }
                        case 4:
                            {
                                buf = p1_4;
                                break;
                            }
                    }
                    elements.Add(Instantiate(buf));
                    break;
                }
            case 2:
                {
                    elements.Add(Instantiate(p2));
                    break;
                }
            case 3:
                {
                    elements.Add(Instantiate(p3));
                    break;
                }
            case 4:
                {
                    elements.Add(Instantiate(p4));
                    break;
                }
               
        }
        elements[elements.Count - 1].transform.position = new Vector3(x, 0.5f, z);
        elements[elements.Count - 1].transform.rotation = new Quaternion(0, rot, 0, 1);
        elements[elements.Count - 1].AddComponent<put>();
        elements[elements.Count - 1].AddComponent<Zahvat>();
        elements[elements.Count - 1].AddComponent<Interactable>();
        elements[elements.Count - 1].AddComponent<Delite_Componeny>();
        elements[elements.Count - 1].GetComponent<Delite_Componeny>().me = elements[elements.Count - 1];

    }
}
