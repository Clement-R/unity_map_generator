using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public int numberOfPlayers;
    public GameObject playerBase;
    public GameObject soldier;
    public GameObject rangeDisplay;
    [HideInInspector]
    public Tile lastTileClicked = null;
    public Tile focusedTile = null;
    public Tile previousFocusedTile = null;
    public int cameraSpeed = 10;
    public int activePlayerIndex = 0;
    public int tileSize;
    public bool rangeDrawn = false;

    private bool isWorldReady = false;
    private bool isGameReady = false;
    private bool playersReady = false;
    private bool soldiersSpawned = false;
    private bool gameInitialized = false;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private bool turnBegin = true;
    private int turn = 0;
    private Pathfinding pathfinder;
    private List<Position> tilesInRange = new List<Position>();
    private TilemapManager tilemapManager;
    private List<Tile> lastPath;
    private List<Unit> units = new List<Unit>();
    private Dictionary<int, GameObject> bases = new Dictionary<int, GameObject>();

    private class Player {
        public int index;
        public bool baseIsPlaced = false;
        public Position basePosition = new Position(0, 0);
        public bool dead = false;

        public List<GameObject> units = new List<GameObject>();
        
        public Player(int index) {
            this.index = index;
        }
    }

	void Start () {
        isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
        this.tileSize = GameObject.Find("NoiseGenerator").GetComponent<ElevationNoise>().tileSize;
        tilemapManager = GameObject.Find("TileMap").GetComponent<TilemapManager>();
        pathfinder = GetComponent<Pathfinding>();
    }

    bool deg = false;

    void Update() {
        // DEBUG
        if (isWorldReady && !deg) {
            /*
            foreach (var position in getRange(new Position(10, 10), 3)) {
                Instantiate(playerBase, new Vector3((int)position.x * this.tileSize, (int)position.y * this.tileSize, 0), Quaternion.identity);
            }
            
            pathfinder.FindPath(new Position(3, 17), new Position(3, 21));
            Debug.Log(pathfinder.path.Count);

            foreach (var tile in pathfinder.path) {
                tile.showingPath = true;
            }
            */
            deg = true;
        }
        // End of DEBUG

        cameraMovement();

        if (isWorldReady && gameInitialized) {
            playersReady = true;
            foreach (var player in players) {
                if (!player.Value.baseIsPlaced) {
                    playersReady = false;
                }
            }
        }

        // Choose wich player must play
        Player activePlayer = null;
        if (gameInitialized) {
            activePlayerIndex = turn % numberOfPlayers;
            activePlayer = players[activePlayerIndex];
        }

        if (isWorldReady && !gameInitialized) {
            // Initialize game (players instances)
            InitializePlayers();
            gameInitialized = true;
        }
        else if (isWorldReady && !playersReady) {
            // Let players choose their base, one by one
            if (activePlayer.baseIsPlaced) {
                turn++;
            } else {
                if (lastTileClicked != null) {
                    activePlayer.basePosition = new Position((int)lastTileClicked.position.x, (int)lastTileClicked.position.y);
                    bases.Add(activePlayerIndex, Instantiate(playerBase, new Vector3((int)lastTileClicked.position.x * this.tileSize, (int)lastTileClicked.position.y * this.tileSize, 0), Quaternion.identity) as GameObject);
                    activePlayer.baseIsPlaced = true;
                    lastTileClicked = null;
                }
            }
        }
        else if (playersReady && !soldiersSpawned) {
            Debug.Log("All players base are ready");
            // Spawn soldiers for each player
            foreach (var player in players) {
                spawnUnit(soldier, player.Value.basePosition, player.Value);
            }
            soldiersSpawned = true;
            turn++;
        }
        else if (soldiersSpawned) {

            if (!activePlayer.dead) {
                // Select its first unit and center camear on it
                GameObject firstUnit = activePlayer.units[0];
                if (turnBegin) {
                    Camera.main.transform.position = new Vector3(firstUnit.transform.position.x,
                                                             firstUnit.transform.position.y,
                                                             -10);
                    turnBegin = false;
                }

                if (rangeDrawn) {
                    if (previousFocusedTile != focusedTile) {
                        foreach (var tile in tilesInRange) {
                            tilemapManager.tilemap[tile.y][tile.x].GetComponent<Tile>().showingPath = false;
                        }

                        if (isTileInRange(focusedTile)) {
                            pathfinder.FindPath(firstUnit.GetComponent<Unit>().position, focusedTile.position);
                            lastPath = pathfinder.path;
                            foreach (var tile in pathfinder.path) {
                                tile.showingPath = true;
                            }
                        }
                    }

                    if (Input.GetMouseButtonDown(0)) {
                        if (isTileInRange(focusedTile)) {
                            firstUnit.GetComponent<Unit>().position = focusedTile.position;
                            firstUnit.transform.position = new Vector2(focusedTile.position.x * tileSize, focusedTile.position.y * tileSize);

                            // Check if an unit is under the targeted position
                            foreach (var unit in units) {
                                if (unit.playerIndex != activePlayerIndex) {
                                    if (unit.position == focusedTile.position) {
                                        // We keep the index of the attacked player
                                        int attackedPlayerIndex = unit.playerIndex;

                                        // Destroy the unit
                                        Debug.Log(attackedPlayerIndex);
                                        players[attackedPlayerIndex].units.Remove(unit.gameObject);
                                        units.Remove(unit);
                                        Destroy(unit.gameObject);

                                        // We check if it was the last unit of the player
                                        if (players[attackedPlayerIndex].units.Count == 0) {
                                            Destroy(bases[attackedPlayerIndex]);
                                            players[attackedPlayerIndex].dead = true;
                                        }

                                        break;
                                    }
                                }
                            }

                            turnEnd();
                        }
                    }
                }
                else {
                    if (lastPath != null) {
                        foreach (var tile in lastPath) {
                            tile.showingPath = false;
                        }
                        lastPath = null;
                    }
                }
            }
            else {
                turnEnd();
            }


            if (Input.GetMouseButtonDown(1)) {
                turnEnd();
            }
        }
        else {
            isWorldReady = GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated;
        }

        /*
        if (lastTileClicked != null)
            Debug.Log(lastTileClicked);
        */

        previousFocusedTile = focusedTile;
    }

    void spawnUnit(GameObject unit, Position position, Player player) {
        GameObject newUnit = Instantiate(unit, new Vector3(position.x * this.tileSize, (position.y * this.tileSize) + 16, 0), Quaternion.identity) as GameObject;
        newUnit.name = "Soldier_" + player.index + "_1";
        newUnit.GetComponent<Unit>().playerIndex = player.index;
        newUnit.GetComponent<Unit>().position = new Position(position.x, position.y + 1);

        units.Add(newUnit.GetComponent<Unit>());

        player.units.Add(newUnit);
    }

    void InitializePlayers() {
        for (int i = 0; i < numberOfPlayers; i++) {
            players.Add(i, new Player(i));
        }
    }

    void turnEnd() {
        turn++;
        turnBegin = true;
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

    public List<Position> getRange(Position soldierPos, int range) {
        List<Position> validPositions = new List<Position>();

        int x = soldierPos.x + range;
        int y = 0;

        for (int i = 0; i < (range * 2) + 1; i++) {

            if (y == 0) {
                // If we're on the first or last cell
                if (x >= 0 && y >= 0 && x < tilemapManager.mapWidth && y < tilemapManager.mapHeight)
                    validPositions.Add(new Position(x, soldierPos.y));
            } else {
                if(x != soldierPos.x && y != soldierPos.y && x >= 0 && y >= 0 && x < tilemapManager.mapWidth && y < tilemapManager.mapHeight)
                    validPositions.Add(new Position(x, soldierPos.y));
                
                for (int j = 1; j <= y; j++) {
                    if(x >= 0 && y >= 0 && x < tilemapManager.mapWidth && y < tilemapManager.mapHeight)
                        validPositions.Add(new Position(x, soldierPos.y + j));
                }

                for (int j = y; j > 0; j--) {
                    if (x >= 0 && y >= 0 && x < tilemapManager.mapWidth && y < tilemapManager.mapHeight)
                        validPositions.Add(new Position(x, soldierPos.y - j));
                }
            }

            x--;
            if(x >= soldierPos.x) {
                y++;
            } else {
                y--;
            }   
        }

        return validPositions;
    }

    public void toggleRange(Position position, int range, Unit unit) {
        tilesInRange = getRange(position, range);

        if (!unit.rangeDrawn) {
            // Draw range
            GameObject container = new GameObject("range");
            foreach (var tile in tilesInRange) {
                if(tilemapManager.isWalkable(tile)) {
                    container.transform.parent = unit.transform;
                    Instantiate(rangeDisplay, new Vector3((int)tile.x * tileSize, (int)tile.y * tileSize, 0), Quaternion.identity, container.transform);
                }
            }
            unit.rangeDrawn = true;
            rangeDrawn = true;
        } else {
            Destroy(unit.transform.Find("range").gameObject);
            unit.rangeDrawn = false;
            rangeDrawn = false;
        }
    }

    public void clearRange(Unit unit) {
        if (unit.rangeDrawn) {
            Destroy(unit.transform.Find("range").gameObject);
            unit.rangeDrawn = false;
            rangeDrawn = false;
        }
    }

    public bool isTileInRange(Tile tile) {
        bool isIn = false;
        foreach (var pos in tilesInRange) {
            if(tile.position.x == pos.x && tile.position.y == pos.y)
                isIn = true;
        }
        return isIn;
    }
}
