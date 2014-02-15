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

        var border = Resources.Load("InvisibleBorder", typeof(Transform)) as Transform;
        if (border != null)
        {
            var offsetX = ((horiontalChunkCount - 1f) / 2f) * chunkWidth;
            var offsetY = ((verticalChuckCount - 1f) / 2f) * chunkHeight;
            Debug.Log(offsetX + " " + offsetY);

            var tempX = horiontalChunkCount * chunkWidth;
            var tempY = verticalChuckCount * chunkHeight;

            var left = - chunkWidth / 2 - 1;
            var right = chunkWidth * horiontalChunkCount - chunkWidth /2;
            var bottom = - chunkHeight / 2 - 1;
            var top = chunkHeight *verticalChuckCount - chunkHeight / 2;

            var borderParent = new GameObject().transform;
            borderParent.name = "Border";

            var scale = new Vector3(tempX + 2, 1, 1);
            border.localScale = scale;

            var temp = Transform.Instantiate(border, new Vector3(offsetX, top, -.5f), border.rotation) as Transform;
            temp.name = "Border top";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(offsetX, bottom, -.5f), border.rotation) as Transform;
            temp.parent = borderParent;
            temp.name = "Border bottom";

            scale = new Vector3(1, tempY + 2, 1);
            border.localScale = scale;

            temp = Transform.Instantiate(border, new Vector3(right, offsetY, -.5f), border.rotation) as Transform;
            temp.name = "Border right";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(left, offsetY, -.5f), border.rotation) as Transform;
            temp.name = "Border left";
            temp.parent = borderParent;
        }
    }
}
