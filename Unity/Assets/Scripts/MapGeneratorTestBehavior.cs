using UnityEngine;
using System.Collections;

public class MapGeneratorTestBehavior : MonoBehaviour
{
    private void Start()
    {
        this.StartCoroutine(MapGenerator.Singleton.Generate(2, 2, 11, 11, "LevelChunk001"));
    }
}
