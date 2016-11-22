using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ElevationNoise : MonoBehaviour {
    public int pixWidth;
    public int pixHeight;
    // public float frequency = 1.0F;
    public Vector2[] octaves;

    [System.Serializable]
    public class Biome {
        public float treshold;
        public Color color;
    }
    public Biome[] biomes;

    private Texture2D noiseTex;
    private Color[] pix;

    void Start() {
        noiseTex = new Texture2D(pixWidth, pixHeight);
        noiseTex.name = "Procedural Texture";
        pix = new Color[noiseTex.width * noiseTex.height];
        CalcNoise();
        GetComponent<Renderer>().material.mainTexture = noiseTex;
        GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
    }

    void CalcNoise() {
        float y = 0.0F;
        while (y < noiseTex.height) {
            float x = 0.0F;
            while (x < noiseTex.width) {
                float xCoord = x / noiseTex.width;
                float yCoord = y / noiseTex.height;

                float sample = 0.0f;
                if (octaves.Length > 0) {
                    foreach (var octave in octaves) {
                        sample += octave.x * Mathf.PerlinNoise(xCoord * octave.y, yCoord * octave.y);
                    }
                } else {
                    sample = Mathf.PerlinNoise(xCoord, yCoord);
                }

                Color col = new Color();
                foreach (var biome in biomes) {
                    if(sample <= biome.treshold) {
                        col = biome.color;
                        break;
                    }
                }

                // pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
                pix[(int)(y * noiseTex.width + x)] = col;

                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update() {
        CalcNoise();
    }
}
