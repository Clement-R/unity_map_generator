using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public int numberOfPlayers;
    public GameObject playerBase;
    public GameObject soldier;
    [HideInInspector]
    public Tile focusedTile = null;
    public int cameraSpeed = 10;

    private bool isWorldReady = false;
    private bool isGameReady = false;
    private bool playersReady = false;
    private bool soldiersSpawned = false;
    private bool gameInitialized = false;
    private Player[] players = null;
    private int turn = 0;
    private int tileSize;

    private class Player {
        public int index;
        public bool baseIsPlaced = false;
        public Position basePosition = new Position(0, 0);
        
        public List<GameObject> unites = new List<GameObject>();

        public Player(int index) {
            this.index = index;
        }
    } 

    public class Position {
        public int x;
        public int y;

        public Position(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

	void Start () {
        isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
        this.tileSize = GameObject.Find("NoiseGenerator").GetComponent<ElevationNoise>().tileSize;
    }
	
	void Update () {

        cameraMovement();

        if (isWorldReady && gameInitialized) {
            playersReady = true;
            foreach (var player in players) {
                if (!player.baseIsPlaced) {
                    playersReady = false;
                }
            }
        }
        
        if (isWorldReady && !gameInitialized) {
            // Initialize game (players instances)
            InitializePlayers();
            gameInitialized = true;
        } else if (isWorldReady && !playersReady) {
            // Let players choose their base, one by one
            Player activePlayer = players[turn];
            if(activePlayer.baseIsPlaced) {
                turn++;
            } else {
                if(focusedTile != null) {
                    activePlayer.basePosition = new Position((int) focusedTile.position.x, (int) focusedTile.position.y);
                    Instantiate(playerBase, new Vector3((int)focusedTile.position.x * this.tileSize, (int)focusedTile.position.y * this.tileSize, 0), Quaternion.identity);
                    activePlayer.baseIsPlaced = true;
                    focusedTile = null;
                }
            }
        } else if(playersReady && !soldiersSpawned) {
            Debug.Log("All players base are ready");
            // Spawn soldiers for each player
            foreach (var player in players) {
                player.unites.Add(Instantiate(soldier, new Vector3(player.basePosition.x * this.tileSize, (player.basePosition.y * this.tileSize) + 16, 0), Quaternion.identity) as GameObject);
            }
            soldiersSpawned = true;
        } else if (soldiersSpawned) {
            Debug.Log("Game Begin");
        } else {
            isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
        }
	}

    void InitializePlayers() {
        players = new Player[numberOfPlayers];

        for (int i = 1; i <= numberOfPlayers; i++) {
            players[i - 1] = new Player(i);
        }
    }

    void cameraMovement() {
        if(Input.GetKey(KeyCode.UpArrow)) {
            Camera.main.transform.Translate(new Vector3(0, cameraSpeed, 0));
        } else if(Input.GetKey(KeyCode.DownArrow)) {
            Camera.main.transform.Translate(new Vector3(0, -cameraSpeed, 0));
        } else if(Input.GetKey(KeyCode.LeftArrow)) {
            Camera.main.transform.Translate(new Vector3(-cameraSpeed, 0, 0));
        } else if(Input.GetKey(KeyCode.RightArrow)) {
            Camera.main.transform.Translate(new Vector3(cameraSpeed, 0, 0));
        }
    }
}
