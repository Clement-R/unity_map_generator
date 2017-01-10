using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {
    bool isWorldReady = false;
    
    void Start () {
        isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
    }
	
	void Update () {
	    if(isWorldReady) {

        }
	}
}
