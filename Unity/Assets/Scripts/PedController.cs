using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PedController : SingletonMonoBehaviour<PedController>
{
    private readonly List<Ped> peds = new List<Ped>();
    private GameObject pedPrefab;

	private static float MOVEMENTSPEED = 4;

    public IEnumerable<Ped> Peds
    {
        get
        {
            return this.peds;
        }
    }

    protected override void Start()
    {
        base.Start();
        this.pedPrefab = Resources.Load<GameObject>("Ped");
    }

    public void AddPed(int id, Vector3 position, Vector3 rotation)
    {
        var gameObject = (GameObject)GameObject.Instantiate(this.pedPrefab, position, Quaternion.Euler(rotation));
        if (Network.isClient)
        {
            gameObject.rigidbody.detectCollisions = false;
            gameObject.collider.enabled = false;
        }
        var ped = new Ped()
        {
            Id = id,
            Transform = gameObject.transform,
        };
        this.peds.Add(ped);
        Debug.Log(string.Join(",", this.peds.Select(p => p.Id.ToString()).ToArray()));
    }

    public void UpdatePedFromServer(int id, Vector3 position, Vector3 rotation, Vector3 direction, bool trading, bool backstabbing)
    {
        var ped = this.peds.First(p => p.Id == id);
        ped.Transform.position = position;
        ped.Transform.rotation = Quaternion.Euler(rotation);
        CharacterAnimator pedCA = ped.Transform.GetChild(0).GetComponent<CharacterAnimator>();
        pedCA.walkingX = direction.x;
        pedCA.walkingY = direction.y;
        pedCA.isDisrupting = backstabbing;
        pedCA.isTrading = trading;
    }

    public void UpdatePedFromClient(NetworkPlayer np, Vector3 direction)
    {
        if(this.TradingController.IsTrading(np) && direction != Vector3.zero)
        {
            return;
        }

        int id = this.PlayerController.Players.First(d => d.NetworkPlayer == np).PedId;
        var ped = this.peds.First(p => p.Id == id);
        ped.Direction = direction;
        ped.Transform.rigidbody.velocity = ped.Direction * 1f * MOVEMENTSPEED;
        if (direction != Vector3.zero)
        {
            Quaternion rotationToLookAt = Quaternion.LookRotation(direction);
            ped.Transform.rotation = Quaternion.Lerp(this.transform.rotation, rotationToLookAt, Time.deltaTime);
        }

    }

    public void FixedUpdate()
    {
        if (Network.isServer)
        {
            foreach (Ped ped in peds)
            {
                //if (ped.Transform.position.z < -0.7f && ped.Transform.position.z > -0.6f)
                //    ped.Transform.position = new Vector3(ped.Transform.position.x, ped.Transform.position.y, -0.7f);
                ped.Transform.rigidbody.velocity = ped.Direction * 1f * MOVEMENTSPEED;


                if (ped.Transform.rigidbody.velocity.magnitude != 0f || ped.LastPosSent == Vector3.zero || Vector3.Distance(ped.LastPosSent, ped.Transform.position) > 0.1f || !ped.DirectionZeroSent)
                {
                    if (ped.Transform.rigidbody.velocity.normalized == Vector3.zero)
                        ped.DirectionZeroSent = true;
                    else
                        ped.DirectionZeroSent = false;
                    this.NetworkMessageController.UpdatePed(ped.Id, ped.Transform.position, ped.Transform.eulerAngles, ped.Transform.rigidbody.velocity.normalized, ped.IsTrading, ped.IsBackstabbing);
                    ped.LastPosSent = ped.Transform.position;
                }
            }
        }
    }

}

