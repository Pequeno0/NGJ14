using UnityEngine;
using System.Collections;

public class PrePlayingController : SingletonMonoBehaviour<PrePlayingController>
{
    protected override void Start()
    {
        base.Start();
        this.StartCoroutine(this.Process());
    }

    private IEnumerator Process()
    {
        while (Application.isPlaying)
        {
            if (this.GameStateController.CurrentGameState == GameState.PrePlaying)
            {
                if (Network.isServer)
                {
                    // set up map
                    var playerCount = this.PlayerController.PlayerCount;
                    var tileCount = Mathf.CeilToInt(playerCount * 0.25f);
                    MapGenerator.Singleton.Generate(tileCount, tileCount, 11, 11, "LevelChunk001", "LevelChunk002");
                    //var enumerator = MapGenerator.Singleton.Generate(tileCount, tileCount, 11, 11, "LevelChunk001", "LevelChunk002");
                    //while(enumerator.MoveNext())
                    //{
                    //    yield return null;
                    //}

                    // set up pedestrians
                    var id = 0;
                    foreach (var player in this.PlayerController.Players)
                    {
                        player.PedId = id;
                        this.NetworkMessageController.AddPed(id, Vector3.zero, Vector3.zero);
                        id++;
                        yield return null;

                        // and then one for the bot
                        this.NetworkMessageController.AddPed(id, Vector3.zero, Vector3.zero);
                        id++;
                        yield return null;
                    }

                    this.GameStateController.Play();
                }
            }
            yield return null;
        }
    }
}
