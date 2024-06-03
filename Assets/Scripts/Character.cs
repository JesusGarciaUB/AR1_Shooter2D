using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using TMPro;

public class Character : MonoBehaviourPun, IPunObservable
{
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private float shootSpeed;
    private float PlayerHealth;
    private float PlayerDamage;

    private Rigidbody2D rb;
    private Vector2 desiredMovementAxis;

    private PhotonView pv;
    private Vector3 enemyPosition = Vector3.zero;
    private bool isFlipped = false;

    private Animator anim;

    [SerializeField] private TextMeshProUGUI playername;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool onGround;
    private float toreceive = 0;
    private string enemyName = "";
    private float attackSpeedTimer = 1f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        speed = Player_Stats.stats.MoveSpeed;
        jumpForce = Player_Stats.stats.JumpForce;
        shootSpeed = Player_Stats.stats.ShootSpeed;
        PlayerHealth = Player_Stats.stats.Health;
        PlayerDamage = Player_Stats.stats.Damage;

        playername.text = Player_Stats.stats.PlayerName;

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (speed == 0) 
            { 
                Network_Manager._NETWORK_MANAGER.AskForStats(Player_Stats.stats.PlayerName);

                speed = Player_Stats.stats.MoveSpeed;
                jumpForce = Player_Stats.stats.JumpForce;
                shootSpeed = Player_Stats.stats.ShootSpeed;
                PlayerHealth = Player_Stats.stats.Health;
                PlayerDamage = Player_Stats.stats.Damage;

                playername.text = Player_Stats.stats.PlayerName;
            }
            attackSpeedTimer += Time.deltaTime * shootSpeed;
            CheckInputs();
        } else
        {
            SmoothReplicate();
        }

        bool isMovingTemp = false;
        if (rb.velocity.x < -0.05 || rb.velocity.x > 0.05) isMovingTemp = true;
        anim.SetBool("moving", isMovingTemp);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(desiredMovementAxis.x * speed, rb.velocity.y);
    }

    private void CheckInputs()
    {
        desiredMovementAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (desiredMovementAxis.x > 0.01f) GetComponent<SpriteRenderer>().flipX = false;
        if (desiredMovementAxis.x < -0.01f) GetComponent<SpriteRenderer>().flipX = true;

        isFlipped = GetComponent<SpriteRenderer>().flipX;

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
        {
            Shoot();
        }

        onGround = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(3.77f, 0.54f), CapsuleDirection2D.Horizontal, 0, groundLayer);

        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Shoot()
    {
        if (attackSpeedTimer >= 1f)
        {
            PhotonNetwork.Instantiate("bullet", transform.position + new Vector3(isFlipped ? -1.2f : 1f, -0.8f, 0), Quaternion.identity, 0, new object[] { isFlipped, PlayerDamage });
            attackSpeedTimer = 0f;
        }
    }

    public void Damage(float damage)
    {
        PlayerHealth -= damage;
        if(PlayerHealth<= 0) pv.RPC("NetworkDamage", RpcTarget.All);
    }

    [PunRPC]
    public void NetworkDamage()
    {
        Destroy(this.gameObject);
    }

    private void SmoothReplicate()
    {
        transform.position = Vector3.Lerp(transform.position, enemyPosition, Time.deltaTime * 20);
        GetComponent<SpriteRenderer>().flipX = isFlipped;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(isFlipped);
            stream.SendNext(Player_Stats.stats.PlayerName);
        }
        else if (stream.IsReading)
        {
            enemyPosition = (Vector3)stream.ReceiveNext();
            isFlipped = (bool)stream.ReceiveNext();
            enemyName = (string)stream.ReceiveNext();
            playername.text = enemyName;
        }
    }
}
