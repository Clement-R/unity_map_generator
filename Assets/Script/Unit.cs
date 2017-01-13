using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
    public int health = 5;
    public int range = 3;
    public int playerIndex = 0;
    public Position position;
    public bool rangeDrawn = false;

    private bool isFocused = false;

    private GameManager god;

	void Start () {
        god = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {
        CastRay();
        if (Input.GetMouseButtonDown(0) && isFocused && god.activePlayerIndex == this.playerIndex) {
            god.toggleRange(position, this.range, this);
        } else if(Input.GetMouseButtonDown(0) && !isFocused) {
            if (rangeDrawn) {
                god.clearRange(this);
            }
        }
    }

    void CastRay() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null) {
            if (this.name == hit.collider.name) {
                isFocused = true;
            }
            else {
                isFocused = false;
            }
        }
    }
}
