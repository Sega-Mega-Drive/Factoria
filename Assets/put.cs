using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class put : MonoBehaviour {
    Vector3 vec;
    Quaternion rot;
    int count;
	// Use this for initialization
	void Start () {
        vec = transform.position;
        rot = transform.rotation;
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
                        x[i] = x[i] * 10;
                        if (x[i] - (Convert.ToInt32(x[i])) != 0 && Math.Abs(x[i] - (x[i] + 1)) < Math.Abs(x[i] - (x[i] - 1)))
                        {
                            x[i]++;
                        }
                        else
                        {
                            x[i]--;
                        }
                        x[i] = x[i] / 10;
                }
        }
        transform.rotation = rot;
        transform.position = new Vector3(x[0], 0.5f, x[1]);
        vec = transform.position;
    }

}
