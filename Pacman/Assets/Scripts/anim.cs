using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Material newMat1 = Resources.Load("v1", typeof(Material)) as Material;
        Material newMat2 = Resources.Load("v2", typeof(Material)) as Material;
        Material newMat3 = Resources.Load("v3", typeof(Material)) as Material;
        GetComponent<MeshRenderer>().material = newMat1;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
