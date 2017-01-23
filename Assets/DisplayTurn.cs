using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTurn : MonoBehaviour {
    GameManager gm;
    Text text;

	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        text = GetComponent<Text>();
	}
	
	void Update () {
        text.text = "Player " + (gm.activePlayerIndex + 1);
	}
}
