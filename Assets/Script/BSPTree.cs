using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSPTree : MonoBehaviour {
    public int maxLeafSize = 20;
    public int minSize = 8;
    public GameObject square;
    
    public int mapHeight = 20;
    public int mapWidth = 20;

	void Start () {
        List<Leaf> leafs = new List<Leaf>();

        Leaf l = new Leaf(0, 0, mapWidth, mapHeight, "");

        leafs.Add(l);

        bool canSplit = true;
        int index = 0;

        while (canSplit || index < leafs.Count) {
            canSplit = false;

            if(index < leafs.Count) {
                Leaf leaf = leafs[index];
                if (leaf.splited == false) {
                    if (leaf.width * leaf.height > maxLeafSize) {
                        if (leaf.Split()) {
                            leafs.Add(leaf.leftChild);
                            leafs.Add(leaf.rightChild);
                            canSplit = true;
                        }
                    }
                }

                if(canSplit) {
                    index = 0;
                } else {
                    index++;
                }
            }
        }

        for (int k = 0; k < leafs.Count ; k++) {
            Leaf leaf = leafs[k];

            if(leaf.splited == false) {
                GameObject go = new GameObject();

                for (int i = 0; i < leaf.height; i++) {
                    for (int j = 0; j < leaf.width; j++) {
                        GameObject sq = Instantiate(square, new Vector3(leaf.x + j, leaf.y + i, 0), Quaternion.identity) as GameObject;
                        sq.transform.parent = go.transform;
                        sq.GetComponentInChildren<TextMesh>().text = leaf.index;
                        sq.GetComponent<SpriteRenderer>().color = leaf.color;
                    }
                }
            }
        }
    }
}

public class Leaf : MonoBehaviour{
    public Color color;

    public int x;
    public int y;
    public int width;
    public int height;

    public Leaf leftChild = null;
    public Leaf rightChild = null;

    public int minSize = GameObject.Find("TerrainGenerator").GetComponent<BSPTree>().minSize;

    public bool splited = false;

    public string index = "";

    public Leaf (int x, int y, int width, int height, string index) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = new Color(Random.Range(0.0f, 1.1f),
                               Random.Range(0.0f, 1.1f),
                               Random.Range(0.0f, 1.1f),
                               255);
        this.index = index;
    }

    public bool Split() {
        // Check if this leaf has already been splited
        if(this.leftChild != null && this.rightChild != null) {
            return false;
        }

        // Choose a direction
        bool splitH = false;
        int direction = Random.Range(0, 2);
        if (direction == 0) {
            splitH = false;
        }
        else {
            splitH = true;
        }

        // Check the maximum size depending of the previously random direction
        int max = (splitH ? height : width) - minSize;

        if(max <= minSize) {
            // We can't split anymore on the selected axis, we try the other one
            max = (splitH ? width : height) - minSize;
            if (max <= minSize) {
                return false;
            } else {
                splitH = !splitH;
            }
        }

        int split = Random.Range(minSize, max + 1);

        if(splitH) {
            this.leftChild = new Leaf(x, y, width, split, this.index + ".1");
            this.rightChild = new Leaf(x, y + split, width, height - split, this.index + ".2");
        } else {
            this.leftChild = new Leaf(x, y, split, height, this.index + ".1");
            this.rightChild = new Leaf(x + split, y, width - split, height, this.index + ".2");
        }

        this.splited = true;
        return true;
    }
}