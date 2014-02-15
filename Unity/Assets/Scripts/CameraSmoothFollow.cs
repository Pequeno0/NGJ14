using UnityEngine;
using System.Linq;
using System.Collections;

public class CameraSmoothFollow : BaseMonoBehaviour
{
    private void Update()
    {
        if (this.GameStateController.CurrentGameState == GameState.Playing)
        {
            //var player = this.PlayerController.Players.First(p => p.NetworkPlayer == Network.player);
            var ped = this.PedController.Peds.First(p => p.Id == int.Parse(Network.player.ToString()));
            var current = Camera.main.transform.position;
            var target = new Vector3(
                Mathf.SmoothStep(current.x, ped.Transform.position.x, 0.1f),
                Mathf.SmoothStep(current.y, ped.Transform.position.y, 0.1f),
                current.z
                );
            Camera.main.transform.position = target;
        }
    }
}
