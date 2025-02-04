﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
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

    public string ipAddress;
    public int port;
    public GameObject playerPrefab;
    public List<LoginRequest> connectedPlayers = new List<LoginRequest>();

    void Awake()
    {
        _instance = this;
        Application.runInBackground = true;
        InitializeServer(ipAddress, port);
    }

    void InitializeServer(string ipAddress, int port)
    {
        RegisterHandlers();
        NetworkServer.Listen(port);
        Debug.Log("Server On: " + NetworkServer.active);
    }

    void RegisterHandlers()
    {
        NetworkServer.RegisterHandler(MessageType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MessageType.Disconnect, OnDisconnected);
        NetworkServer.RegisterHandler(MessageType.ChatMessage, OnChatMessageReceived);
        NetworkServer.RegisterHandler(MessageType.LoginRequest, OnLoginRequestReceived);
        NetworkServer.RegisterHandler(MessageType.AddPlayer, OnServerAddPlayer);
        NetworkServer.RegisterHandler(MessageType.LogoutRequest, OnLogoutRequestReceived);
        NetworkServer.RegisterHandler(MessageType.MovePlayer, OnMovePlayerReceived);
        NetworkServer.RegisterHandler(MessageType.ChangeRoom, OnChangeRoomReceived);
    }

    void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Client connected: " + netMsg.conn.connectionId);

    }

    void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Client dropped connection");

        //forcefuly quit
        NetworkServer.DestroyPlayersForConnection(netMsg.conn);

        int index = connectedPlayers.FindIndex(user => user.connectionID == netMsg.conn.connectionId);
        connectedPlayers.RemoveAt(index);
        PlayerController.players.RemoveAt(index);
        Debug.Log("Connected Players Count: " + connectedPlayers.Count);
    }

    void OnChatMessageReceived(NetworkMessage netMsg)
    {
        ChatMessage sender = netMsg.ReadMessage<ChatMessage>();
        sender.connectionID = netMsg.conn.connectionId;
        sender.HandleMessage();
    }

    void OnLoginRequestReceived(NetworkMessage netMsg)
    {
        Debug.Log("OnLoginRequestReceived::ConnectionId: " + netMsg.conn.connectionId);
        LoginRequest checkUser = netMsg.ReadMessage<LoginRequest>();
        checkUser.connectionID = netMsg.conn.connectionId;
        checkUser.HandleRequest();
    }

    public void OnServerAddPlayer(NetworkMessage netMsg)
    {
        //init player, get info from db, sned daata to pleyer
        Debug.Log("OnServerAddPlayer()");
        PlayerController.SpawnPlayer(netMsg.conn);
        //GameObject player = Instantiate(playerPrefab);
        //player.transform.position = new Vector2(Random.Range(-5,5), Random.Range(-5,5));
        //NetworkServer.AddPlayerForConnection(netMsg.conn, player, 0);

    }

    public void OnLogoutRequestReceived(NetworkMessage netMsg)
    {
        var receivedLogoutRequest = netMsg.ReadMessage<LogoutRequest>();
        receivedLogoutRequest.HandleRequest();
    }

    public void OnMovePlayerReceived(NetworkMessage netMsg)
    {
        MovePlayer playerMoved = netMsg.ReadMessage<MovePlayer>();
        playerMoved.connectionID = netMsg.conn.connectionId;


        PlayerController.MovePlayer(netMsg.conn, playerMoved.keyPressed);

        //player.GetComponent<Rigidbody2D>().transform.position = new Vector2(-2, -2);

    }
    public void OnChangeRoomReceived(NetworkMessage netMsg)
    {
        ChangeRoom user = netMsg.ReadMessage<ChangeRoom>();
        //user.connectionID = netMsg.conn.connectionId;
        user.SwitchRoom();
        //add player, then add client side,
        //PlayerController.SpawnPlayer(netMsg.conn);
    }
}
