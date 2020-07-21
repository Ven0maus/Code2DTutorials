using UnityEngine;

namespace Assets.Helpers
{
    public class Noise
    {
        public static float[] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[] noiseMap = new float[mapWidth * mapHeight];

            var random = new System.Random(seed);

            // We need atleast one octave
            if (octaves < 1)
            {
                octaves = 1;
            }

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + offset.x;
                float offsetY = random.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0f)
            {
                scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // When changing noise scale, it zooms from top-right corner
            // This will make it zoom from the center
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int x = 0, y; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    // Define base values for amplitude, frequency and noiseHeight
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    // Calculate noise for each octave
                    for (int i = 0; i < octaves; i++)
                    {
                        // We sample a point (x,y)
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        // Use unity's implementation of perlin noise
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // noiseHeight is our final noise, we add all octaves together here
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    // We need to find the min and max noise height in our noisemap
                    // So that we can later interpolate the min and max values between 0 and 1 again
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    // Assign our noise
                    noiseMap[y * mapWidth + x] = noiseHeight;
                }
            }

            for (int x = 0, y; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    // Returns a value between 0f and 1f based on noiseMap value
                    // minNoiseHeight being 0f, and maxNoiseHeight being 1f
                    noiseMap[y * mapWidth + x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * mapWidth + x]);
                }
            }
            return noiseMap;
        }

        public static float[] GenerateIslandGradientMap(int mapWidth, int mapHeight)
        {
            float[] map = new float[mapWidth * mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Value between 0 and 1 where * 2 - 1 makes it between -1 and 0
                    float i = x / (float)mapWidth * 2 - 1;
                    float j = y / (float)mapHeight * 2 - 1;

                    // Find closest x or y to the edge of the map
                    float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));

                    // Apply a curve graph to have more values around 0 on the edge, and more values >= 3 in the middle
                    float a = 3;
                    float b = 2.2f;
                    float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

                    // Apply gradient in the map
                    map[y * mapWidth + x] = islandGradientValue;
                }
            }
            return map;
        }
    }
}
