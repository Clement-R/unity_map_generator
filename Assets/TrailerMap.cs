using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerMap : MonoBehaviour {
    ElevationNoise en;

	void Start () {
        en = GameObject.Find("NoiseGenerator").GetComponent<ElevationNoise>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space)) {
            en.seed += 1;
        }
	}
}
