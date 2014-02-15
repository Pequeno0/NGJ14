using UnityEngine;
using System.Collections;

public class MapGenerator
{
    public static readonly MapGenerator Singleton = new MapGenerator();

    public IEnumerator Generate(int horiontalChunkCount, int verticalChuckCount, int chunkWidth, int chunkHeight, params string[] availableChunkNames)
    {
        for (var y = 0; y < verticalChuckCount; y++)
        {
            for (var x = 0; x < horiontalChunkCount; x++)
            {
                var rnd = Random.Range(0, availableChunkNames.Length);
                var chunkName = availableChunkNames[rnd];
                Application.LoadLevelAdditive(chunkName);
                yield return null;
                var chunk = GameObject.Find(chunkName);
                chunk.name = string.Concat("Level chunk (", x, ", ", y, " [", chunk.name, "])");
                chunk.transform.position = new Vector3(x * chunkWidth, y * chunkHeight, 0.0f);

                rnd = Random.Range(0, 3);
                chunk.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rnd * 90.0f);
            }
        }
    }
}
