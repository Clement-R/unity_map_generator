using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerEffects : MonoBehaviour {
    public Sprite[] soldiers;

    int index;

	void Update () {
        SpriteRenderer player = GameObject.Find("Soldier_0_1").GetComponent<SpriteRenderer>();

		if(Input.GetKeyDown(KeyCode.Space)) {
            player.sprite = soldiers[index];
            index = Mathf.Clamp(index + 1, 0, soldiers.Length - 1);
        }
	}
}
