using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PedController : SingletonMonoBehaviour<PedController>
{
    private readonly List<Ped> peds = new List<Ped>();
    private GameObject pedPrefab;

    protected override void Start()
    {
        base.Start();
        this.pedPrefab = Resources.Load<GameObject>("Ped");
    }

    public void AddPed(int id, Vector3 position, Vector3 rotation)
    {
        var gameObject = (GameObject)GameObject.Instantiate(this.pedPrefab, position, Quaternion.Euler(rotation));
        var ped = new Ped()
        {
            Id = id,
            Transform = gameObject.transform,
        };
        this.peds.Add(ped);
    }

    public void UpdatePed(int id, Vector3 position, Vector3 rotation)
    {
        var ped = this.peds.First(p => p.Id == id);
        ped.Transform.position = position;
        ped.Transform.rotation = Quaternion.Euler(rotation);
    }

    public void Update()
    {
        if (Network.isServer)
        {
            foreach (Ped ped in peds)
            {
                this.NetworkMessageController.UpdatePed(ped.Id, ped.Transform.position, ped.Transform.eulerAngles);
            }
        }
    }
    
}

