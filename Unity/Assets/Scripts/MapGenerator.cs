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

        var border = Resources.Load("InvisibleBorder", typeof(Transform)) as Transform;
        if (border != null)
        {
            var offsetX = (horiontalChunkCount * chunkWidth) / 2  + 1;
            var offsetY = (verticalChuckCount * chunkHeight) / 2  + 1;
            Debug.Log(offsetX + " " + offsetY);
            var tempX = horiontalChunkCount * chunkWidth;
            var tempY = verticalChuckCount * chunkHeight;

            var borderParent = new GameObject().transform;
            borderParent.name = "Border";

            var scale = new Vector3(tempX + 2, 1, 1);
            border.localScale = scale;

            var temp = Transform.Instantiate(border, new Vector3(0, offsetY, -.5f), border.rotation) as Transform;
            temp.name = "Border top";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(0, -offsetY, -.5f), border.rotation) as Transform;
            temp.parent = borderParent;
            temp.name = "Border bottom";

            scale = new Vector3(1, tempY + 2, 1);
            border.localScale = scale;
            
            temp = Transform.Instantiate(border, new Vector3(offsetX, 0, -.5f), border.rotation) as Transform;
            temp.name = "Border right";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(-offsetX, 0, -.5f), border.rotation) as Transform;
            temp.name = "Border left";
            temp.parent = borderParent;
        }
    }
}
