using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Network_Manager : MonoBehaviour
{
    public static Network_Manager _NETWORK_MANAGER;

    [SerializeField] private TextMeshProUGUI loginCheck;
    [SerializeField] private List<GameObject> toDeactivate;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject validRegister;
    [SerializeField] private TextMeshProUGUI failedRegister;

    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private bool connected = false;

    const string host = "10.40.1.178";
    const int port = 6346;

    private void Awake()
    {
        if (_NETWORK_MANAGER != null && _NETWORK_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _NETWORK_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);
        }

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            connected = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            connected = false;
            throw;
        }
    }

    private void ManageData(string data)
    {
        if (data == "Ping")
        {
            Debug.Log("Recibo ping");
            writer.WriteLine("1");
            writer.Flush();
        }

        if (data == "LoginTRUE")
        {
            foreach(GameObject g in toDeactivate)
            {
                g.SetActive(false);
            }

            main.SetActive(true);
        }

        if (data == "LoginFALSE")
        {
            validRegister.SetActive(false);
            loginCheck.gameObject.SetActive(true);
        }

        if (data == "RegisterTRUE")
        {
            failedRegister.gameObject.SetActive(false);
            toDeactivate[1].SetActive(false);
            toDeactivate[0].SetActive(true);
            validRegister.SetActive(true);
        }

        if (data == "RegisterFALSE")
        {
            failedRegister.gameObject.SetActive(true);
            failedRegister.text = "Username already exists";
        }
    }

    private void Update()
    {
        if (connected)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    ManageData(data);
                }
            }
        }
    }

    public void ConnectToServer(string nick, string password)
    {
        try
        {
            writer.WriteLine("0" + "/" + nick + "/" + password);
            writer.Flush();

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            connected = false;
            throw;
        }
    }

    public void RegisterNewUser(string user, string password, string race)
    {
        Debug.Log(user + " " + password + " " + race);
        try
        {
            writer.WriteLine("2" + "/" + user + "/" + password + "/" + race);
            writer.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            connected = false;
            throw;
        }
    }
}
