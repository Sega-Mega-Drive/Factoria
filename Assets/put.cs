using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class put : MonoBehaviour {
    Vector3 vec;
    float rot;
    int count;
	// Use this for initialization
	void Start () {
        vec = transform.position;
        rot = transform.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
        float[] x = new float[2];
        x[0] = transform.position.x;
        x[1] = transform.position.z;
        if (vec != transform.position)
        {
                for (int i = 0; i < 2; i++)
                {
                    int tmp = (int)(x[i] * 100);
                    if (tmp % 10 < 5)
                    {
                        tmp /= 10;
                    } else
                    {
                        tmp = tmp / 10 + 1;
                    }
                    x[i] = (float)tmp / 10;
                        /*x[i] = x[i] * 10;
                        if (x[i] - (Convert.ToInt32(x[i])) != 0)
                        {
                              if (Math.Abs(x[i] - (x[i] + 1)) < Math.Abs(x[i] - (x[i] - 1)))
                              {
                                x[i]++;
                              }
                        }
                      x[i] = x[i] / 10; */
                
                }
            float y = Math.Abs(transform.eulerAngles.y%360);
            if (y % 90 != 0)
            {
                if (y > 315)
                {
                    y = 0;
                }
                else
                {
                    if(y>225)
                    {
                        y = 270;
                    }
                    else
                    {
                        if(y>135)
                        {
                            y = 180;
                        }
                        else
                        {
                            if(y > 45)
                            {
                                y = 90;
                            }
                            else
                            {
                                y = 0;
                            }
                        }
                    }
                }

            }
                rot = y;
        }
        transform.eulerAngles = new Vector3(0, rot, 0);
        transform.position = new Vector3(x[0], 0.5f, x[1]);
        vec = transform.position;
    }

}
