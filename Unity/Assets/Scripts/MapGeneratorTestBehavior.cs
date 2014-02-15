using UnityEngine;
using System.Collections;

public class MapGeneratorTestBehavior : MonoBehaviour
{
    private void Start()
    {
        MapGenerator.Singleton.Generate(2, 2, 11, 11, "LevelChunk001", "LevelChunk002");
    }
}
