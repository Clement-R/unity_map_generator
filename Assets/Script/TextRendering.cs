using UnityEngine;
using System.Collections;

public class TextRendering : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Text";
    }
}
