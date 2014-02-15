using UnityEngine;
using System.Collections;

public class MapGenerator
{
    public static readonly MapGenerator Singleton = new MapGenerator();

    public void Generate(int horiontalChunkCount, int verticalChuckCount, int chunkWidth, int chunkHeight, params string[] availableChunkNames)
    {
        var networkMessageController = GameObject.Find("NetworkMessageController").GetComponent<NetworkMessageController>();

        for (var y = 0; y < verticalChuckCount; y++)
        {
            for (var x = 0; x < horiontalChunkCount; x++)
            {
                var rnd = Random.Range(0, availableChunkNames.Length);
                var chunkName = availableChunkNames[rnd];
                //Application.LoadLevelAdditive(chunkName);
                //yield return null;
                //var chunk = GameObject.Find(chunkName);
                //chunk.name = string.Concat("Level chunk (", x, ", ", y, " [", chunk.name, "])");
                //chunk.transform.position = new Vector3(x * chunkWidth, y * chunkHeight, 0.0f);

                //rnd = Random.Range(0, 3);
                //chunk.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rnd * 90.0f);

                var position = new Vector3(x * chunkWidth, y * chunkHeight, 0.0f);
                var rotation = Quaternion.Euler(0.0f, 0.0f, rnd * 90.0f);
                var levelChunkName = string.Concat("Level chunk (", x, ", ", y, " [", chunkName, "])");
                
                networkMessageController.Reliable.RPC("CreateMapChunk", RPCMode.All, chunkName, levelChunkName, position, rotation);
            }
        }

        // Create the invisible borders surrounding the map
        networkMessageController.Reliable.RPC("CreateMapBorder", RPCMode.All, horiontalChunkCount, verticalChuckCount, chunkWidth, chunkHeight);
    }
}
