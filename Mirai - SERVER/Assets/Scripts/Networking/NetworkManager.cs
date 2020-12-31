﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkManager : MonoBehaviour
{
    public string ipAddress;
    public int port;
    public GameObject playerPrefab;

    void Start()
    {
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
    }

    void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Client connected: " + netMsg.conn.connectionId);

    }

    void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Client dropped connection");
        NetworkServer.DestroyPlayersForConnection(netMsg.conn);
    }

    void OnChatMessageReceived(NetworkMessage netMsg)
    {
        ChatMessage sender = netMsg.ReadMessage<ChatMessage>();
        sender.connectionID = netMsg.conn.connectionId;
        sender.HandleMessage();
    }

    void OnLoginRequestReceived(NetworkMessage netMsg)
    {
        LoginRequest checkUser = netMsg.ReadMessage<LoginRequest>();
        checkUser.connectionID = netMsg.conn.connectionId;
        checkUser.HandleRequest();
    }

    public void OnServerAddPlayer(NetworkMessage extraMessageReader)
    {
        //init player, get info from db, sned daata to pleyer
        Debug.Log("OnServerAddPlayer()");

        GameObject player = Instantiate(playerPrefab) as GameObject;
        player.transform.position = new Vector2(Random.Range(-3, 4), Random.Range(-2, 2));

        NetworkServer.AddPlayerForConnection(extraMessageReader.conn, player, 0);
    }
}