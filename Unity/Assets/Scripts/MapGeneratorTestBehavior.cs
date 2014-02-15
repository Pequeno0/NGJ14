using UnityEngine;
using System.Collections;

public class MapGeneratorTestBehavior : MonoBehaviour
{
    private void Start()
    {
        this.StartCoroutine(MapGenerator.Singleton.Generate(1, 1, 11, 11, "LevelChunk001", "LevelChunk002"));
    }
}
