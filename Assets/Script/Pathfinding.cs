using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {
    public List<Tile> path;
    private TilemapManager tilemapManager;
    private GameObject[][] tilemap = null;

    void Awake() {
        tilemapManager = GameObject.Find("TileMap").GetComponent<TilemapManager>();
    }

    void Update() {
        if(tilemap == null) {
            tilemap = tilemapManager.tilemap;
        }
    }

	public void FindPath (Position startPos, Position targetPos) {
        Tile startTile = tilemap[startPos.y][startPos.x].GetComponent<Tile>();
        Tile targetTile = tilemap[targetPos.y][targetPos.x].GetComponent<Tile>();

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);
        
        while (openSet.Count > 0) {
            Tile currentTile = openSet[0];

            for (int i = 1; i < openSet.Count; i++) {
                // If the tile is "closer" (f cost) to the target or the tile has the same distance to the target but is closer to the starting node
                if(openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost) {
                    if (openSet[i].hCost < currentTile.hCost)
                        currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == targetTile) {
                path = RetracePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in tilemapManager.getNeighbours(currentTile)) {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentTile.gCost + getDistance(currentTile, neighbour) + neighbour.weight;

                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, targetTile);
                    neighbour.parent = currentTile;

                    if(!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    List<Tile> RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while(currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        path.Reverse();
        return path;
    }

    int getDistance(Tile start, Tile end) {
        int dstX = Mathf.Abs(end.position.x - start.position.x);
        int dstY = Mathf.Abs(end.position.y - start.position.y);

        return dstX + dstY;
    }
}
