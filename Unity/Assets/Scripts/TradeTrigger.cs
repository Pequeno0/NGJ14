using UnityEngine;
using System.Collections;

public class TradeTrigger : BaseMonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        this.TradingController.CheckDistruptionAvailable(collider);
    }
}
