using UnityEngine;
using System.Collections;

public class ColorPalette : MonoBehaviour
{
    Material material;

    public Color[] Colors;

    public void UpdateMaterial(Color color, int index)
    {
        Colors[index] = color;
        material.SetColorArray("_Colors", Colors);
    }

    void Awake()
    {
        var renderer = GetComponent<Renderer>();
        material = renderer.material;
        Colors = new Color[6];

        Colors[0]  = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Colors[1]  = new Color(1.0f, 0.949f, 0.0f, 1.0f);
        Colors[2]  = new Color(0.247f, 0.282f, 0.8f, 1.0f);
        Colors[3]  = new Color(0.6f, 0.851f, 0.918f, 1.0f);
        Colors[4]  = new Color(0.929f, 0.110f, 0.141f, 1.0f);
        Colors[5]  = new Color(0.710f, 0.902f, 0.151f, 1.0f);

        material.SetColorArray("_Colors", Colors);
    }
}
