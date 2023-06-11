using UnityEngine;

public class PerlinNoise
{
    public float scale = 20.0f;
    public float xOffset = 0f;
    public float zOffset = 0f;

    Texture2D GenerateTexture()
    {
        int width = 256, height = 256;
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                float perlinValue = CalculateNoise(x, y, width, height);
                Color color = new Color(perlinValue, perlinValue, perlinValue);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public float CalculateNoise(int x, int y, int xSize, int ySize)
    {
        float xCord = (float)x / xSize * scale + xOffset;
        float yCord = (float)y / ySize * scale + zOffset;

        float perlinValue = Mathf.PerlinNoise(xCord,yCord);
        return perlinValue;
    }

    public void SetRandomOffset(int seed)
    {
        System.Random random = new System.Random(seed);
        xOffset = random.Next(-100000, 100000);
        zOffset = random.Next(-100000, 100000);
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
    }
}
