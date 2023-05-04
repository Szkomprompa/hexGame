using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20.0f;
    public float xOffset = 0f;
    public float yOffset = 0f;

    private void Start()
    {
        xOffset = Random.Range(0f, 99999f);
        yOffset = Random.Range(0f, 99999f);
    }

    private void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Color color = CalculateColor(x,y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        float xCord = (float)x / width * scale + xOffset;
        float yCord = (float)y / height * scale + yOffset;

        float perlinValue = Mathf.PerlinNoise(xCord,yCord);
        return new Color(perlinValue, perlinValue, perlinValue);
    }
}
