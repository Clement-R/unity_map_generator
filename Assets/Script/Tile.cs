using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public GameManager god;
    public Vector2 position;
    public bool isWalkable = true;
    public int index = 0;
    public int aStarWeight = 0;

    public bool isFocused = false;

	void Start () {
        god = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {
        if(isFocused) {
            GetComponent<SpriteRenderer>().color = Color.black;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        CastRay();
        if (Input.GetMouseButtonDown(0) && isFocused) {
            god.focusedTile = this;
        }
    }

    void CastRay() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null) {
            if(this.name == hit.collider.name) {
                isFocused = true;
            } else {
                isFocused = false;
            }
        }
    }
}
