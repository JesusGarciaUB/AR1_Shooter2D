using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocity;

    private Rigidbody2D rb;
    private PhotonView pv;
    public bool facing = false;
    public float damage = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        object[] data = pv.InstantiationData;
        facing = (bool)data[0];
        damage = (float)data[1];
    }

    private void Start()
    {
        rb.velocity = new Vector2(facing ? -velocity : velocity, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Character>().Damage(damage);
            pv.RPC("NetworkDestroy", RpcTarget.All);
        }

        if (collision.CompareTag("Ground"))
        {
            pv.RPC("NetworkDestroy", RpcTarget.All);
        }
    }

    [PunRPC]
    public void NetworkDestroy()
    {
        Destroy(this.gameObject);
    }
}
