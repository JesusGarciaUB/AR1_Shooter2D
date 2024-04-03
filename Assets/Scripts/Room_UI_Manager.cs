using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room_UI_Manager : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Text lobbyText;

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    private void CreateRoom() { Photon_Manager._PHOTON_MANAGER.CreateRoom(lobbyText.text); }

    private void JoinRoom() { Photon_Manager._PHOTON_MANAGER.JoinRoom(lobbyText.text); }
}
