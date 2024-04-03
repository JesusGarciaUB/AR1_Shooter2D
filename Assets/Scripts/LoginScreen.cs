using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI failedRegister;
    [SerializeField] private TextMeshProUGUI validRegister;
    [SerializeField] private GameObject unknownUser;
    [Header("Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button RegisterButton;
    [Header("Login input fields")]
    [SerializeField] private Text loginText;
    [SerializeField] private Text passwordText;
    [SerializeField] private Text registerUsernameText;
    [SerializeField] private Text registerPasswordText;
    [SerializeField] private Dropdown duckRace;

    [Header("Scenes")]
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject registerScreen;

    private void Awake()
    {
        loginButton.onClick.AddListener(Clicked);
        goToRegisterButton.onClick.AddListener(Register);
        BackButton.onClick.AddListener(Back);
        RegisterButton.onClick.AddListener(TryToRegister);
    }

    private void Clicked()
    {
        Network_Manager._NETWORK_MANAGER.ConnectToServer(loginText.text.ToString(), passwordText.text.ToString());
    }

    private void Register()
    {
        loginText.text = "";
        passwordText.text = "";
        registerUsernameText.text = "";
        registerPasswordText.text = "";
        failedRegister.gameObject.SetActive(false);
        validRegister.gameObject.SetActive(false);
        unknownUser.SetActive(false);
        loginScreen.SetActive(false);
        registerScreen.SetActive(true);
    }

    private void Back()
    {
        failedRegister.gameObject.SetActive(false);
        validRegister.gameObject.SetActive(false);
        registerScreen.SetActive(false);
        loginScreen.SetActive(true);
    }

    private void TryToRegister()
    {
        if (registerUsernameText.text == "" || registerPasswordText.text == "")
        {
            failedRegister.gameObject.SetActive(true);
            failedRegister.text = "Some fields are empty";
        }
        else
        {
            Network_Manager._NETWORK_MANAGER.RegisterNewUser(registerUsernameText.text.ToString(), registerPasswordText.text.ToString(), duckRace.value.ToString());
        }
    }
}
