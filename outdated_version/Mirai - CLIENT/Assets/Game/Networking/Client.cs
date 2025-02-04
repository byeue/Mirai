﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//client login, implement mvc in future?

public class Client : MonoBehaviour
{
    //public InputField usernameInput, passwordInput;

    //public GameObject Username_field, Password_field;

    private static Client _instance;
    public static Client Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new System.Exception();
            }
            return _instance;
        }
    }

    //public InputField usernameInputField, passwordInputField;

    //NetworkClient _client;
    public NetworkClient networkClient;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            networkClient = new NetworkClient();
            DontDestroyOnLoad(this);
            Application.runInBackground = true;
            //Connect();
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }


    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraController.Instance.CameraRight();
        }
    }*/

    public void Connect()
    {
        ClientScene.RegisterPrefab(PlayerInfo.Instance.playerPrefab);
        RegisterHandlers();
        networkClient.Connect("localhost", 7777);

    }

    void RegisterHandlers()
    {
        networkClient.RegisterHandler(MessageType.Connect, OnConnectedToServer); //official
        networkClient.RegisterHandler(MessageType.Disconnect, OnDisconnectedFromServer);
        networkClient.RegisterHandler(MessageType.ChatMessage, OnChatMessageReceivedFromServer);
        networkClient.RegisterHandler(MessageType.LoginRequest, OnLoginRequestReceivedFromServer);
        networkClient.RegisterHandler(MessageType.KickPlayer, OnKickPlayerFromServer);
        networkClient.RegisterHandler(MessageType.MovePlayer, OnUpdatePlayerPositionFromServer);
        networkClient.RegisterHandler(MessageType.ChangeRoom, OnChangeRoomFromServer);
    }

    void OnConnectedToServer(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");

        GameObject userInput = GameObject.Find("Username_Login_Field");
        InputField userInputField = userInput.GetComponent<InputField>();
        GameObject passInput = GameObject.Find("Password_Login_Field");
        InputField passInputField = passInput.GetComponent<InputField>();

        LoginModel model = new LoginModel();
        model.Username = userInputField.text.ToLower();
        model.Password = passInputField.text;
        model.Login();

        /*ChatMessage msg = new ChatMessage();
        msg.target = null;
        msg.message = "helo, server!";

        msg.HandleMessage();*/
    }

    void OnDisconnectedFromServer(NetworkMessage netMsg)
    {
        Debug.Log("Dropped from server");
        Destroy(GameObject.Find("Player(Clone)"));
        SceneManager.LoadScene("Main_Menu");
    }

    void OnChatMessageReceivedFromServer(NetworkMessage netMsg)
    {
        ChatMessage replyFromServer = netMsg.ReadMessage<ChatMessage>();
        replyFromServer.HandleMessageReceivedFromServer();
    }

    void OnLoginRequestReceivedFromServer(NetworkMessage netMsg)
    {
        LoginRequest user = netMsg.ReadMessage<LoginRequest>();
        user.ProcessServerResponse();

    }

    void OnKickPlayerFromServer(NetworkMessage netMsg)
    {
        Debug.Log("Kicked from server.");
        Client.Instance.networkClient.Disconnect();
        Destroy(GameObject.Find("Player(Clone)"));
        SceneManager.LoadScene("Main_Menu");
    }

    void OnUpdatePlayerPositionFromServer(NetworkMessage netMsg)
    {
        MovePlayer user = netMsg.ReadMessage<MovePlayer>();
        PlayerInfo.Instance.playerPrefab.transform.position = user.playerPosition;
    }


    void OnChangeRoomFromServer(NetworkMessage netMsg)
    {
        ChangeRoom user = netMsg.ReadMessage<ChangeRoom>();
        SceneManager.LoadScene(user.newLocation);
        //user.HandleTransition();
    }
}
