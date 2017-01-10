using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public int numberOfPlayers;

    private bool isWorldReady = false;
    private bool isGameReady = false;

	void Start () {
        isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
    }
	
	void Update () {
	    if(isWorldReady) {
            // TODO
            // Let players choose their base
            //
        } else {
            isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
        }
	}
}
