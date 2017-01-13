using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public GameManager god;
    public Position position;
    public bool isWalkable = true;
    public int index = 0;
    public int weight = 0;
    public Tile parent;

    public int gCost = 0;
    public int hCost = 0;

    public bool isFocused = false;
    public bool showingRange = false;
    public bool showingPath = false;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

	void Start () {
        god = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {
        if(isFocused) {
            GetComponent<SpriteRenderer>().color = Color.black;
        } else if(showingRange) {
            GetComponent<SpriteRenderer>().color = Color.blue;
        } else if(showingPath) {
            GetComponent<SpriteRenderer>().color = Color.red;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        CastRay();
        if (Input.GetMouseButtonDown(0) && isFocused) {
            god.lastTileClicked = this;
        }
    }

    void CastRay() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        foreach (var hit in hits) {
            if (hit.collider != null) {
                if (this.name == hit.collider.name) {
                    isFocused = true;
                    god.focusedTile = this;
                }
                else {
                    isFocused = false;
                }
            }
        }
    }
}
