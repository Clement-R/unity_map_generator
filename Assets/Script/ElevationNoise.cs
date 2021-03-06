﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ElevationNoise : MonoBehaviour {
    public int pixWidth;
    public int pixHeight;
    public int tileSize = 16;
    public int seed = 0;
    public Vector2[] octaves;
    public Biome[] biomes;

    [System.Serializable]
    public class Biome {
        public float treshold;
        public Color color;
        public GameObject tile;
    }
    
    private Texture2D noiseTex;
    private Color[] pix;
    private GameObject[] tilemap;
    private int index = 0;

    void Start() {
        noiseTex = new Texture2D(pixWidth, pixHeight);
        noiseTex.name = "Procedural Texture";
        pix = new Color[noiseTex.width * noiseTex.height];
        tilemap = new GameObject[noiseTex.width * noiseTex.height];
        GetComponent<Renderer>().material.mainTexture = noiseTex;
        GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");

        StartCoroutine("recalc");
    }

    IEnumerator recalc() {
        CalcNoise();
        // Can't enable realtime refresh of tilemap because it's causing terrible CPU consumption
        // GameObject.Find("TileMap").GetComponent<TilemapManager>().worldGenerated = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine("recalc");
    }

    void CalcNoise() {
        float y = 0.0F;
        index = 0;

        while (y < noiseTex.height) {
            float x = 0.0F;
            while (x < noiseTex.width) {
                float xCoord = x / noiseTex.width;
                float yCoord = y / noiseTex.height;

                float sample = 0.0f;
                if (octaves.Length > 0) {
                    foreach (var octave in octaves) {
                        sample += octave.x * Mathf.PerlinNoise(this.seed + (xCoord * octave.y), this.seed + (yCoord * octave.y));
                    }
                } else {
                    sample = Mathf.PerlinNoise(this.seed + xCoord, this.seed + yCoord);
                }

                Color col = Color.black;
                GameObject tile = null;

                foreach (var biome in biomes) {
                    if(sample <= biome.treshold) {
                        col = biome.color;
                        tile = biome.tile;
                        break;
                    }
                }

                if(col == Color.black) {
                    col = biomes[biomes.Length - 1].color;
                }

                pix[(int)(y * noiseTex.width + x)] = col;
                tilemap[index] = tile;

                x++;
                index++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    public GameObject[] GetMap() {
        return tilemap;
    }
}
