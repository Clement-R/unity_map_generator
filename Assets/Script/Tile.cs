using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public bool isWalkable = true;
    public int index = 0;
    public int aStarWeight = 0;

    public bool isFocused = false;

	void Start () {
	}
	
	void Update () {
        if(isFocused) {
            // GetComponent<SpriteRenderer>().color = Color.black;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (Input.GetMouseButton(0)) {
            CastRay();
        }
    }

    void CastRay() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null) {
            if(this.name == hit.collider.name) {
                this.GetComponent<SpriteRenderer>().color = Color.black;
            } else {
                this.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
