using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    public static Player_Stats stats;
    public string PlayerName = "";
    public int DuckRace = 0;
    public int Health = 0;
    public int Damage = 0;
    public int MoveSpeed = 0;
    public int JumpForce = 0;
    public int ShootSpeed = 0;

    private void Awake()
    {
        if (stats == null)
        {
            stats = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
}
