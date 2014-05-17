/* Module      : NetworkManager
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Master control for server
 *
 * Date        : 2014/04/25
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the server/client relationship
/// </summary>
public class NetworkManager : MonoBehaviour {

	private string typeName = "Blackskull-Jaegar";
	private int maxConnections = 1;
	private int port = 25000;
	public HostData[] hostList;
	
	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	// Server

	public void StartServer(string gameName, bool hidden, string comment="") {
		Network.InitializeServer(maxConnections, port, !Network.HavePublicAddress());
		if(!hidden)
			MasterServer.RegisterHost (typeName, gameName, comment);
	}

	public void StopServer() {
		Network.Disconnect ();
		MasterServer.UnregisterHost ();
	}

	// Called when server is initialized
	void OnServerInitialized() {
		Debug.Log ("Server up");
	}

	public void RefreshHostList() {
		MasterServer.RequestHostList (typeName);
	}

	// Responds to events from master server
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList ();
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}

	// Player

	public bool JoinServer(HostData host) {
		if (host.connectedPlayers >= host.playerLimit)
			return false;

		Network.Connect (host);
		return true;
	}

	// called on connection to server
	void OnConnectedToServer()
	{
		Debug.Log("Connected to Server");
	}

	// called when failed to connect
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Failed to connect to server: " + error.ToString());
	}
}


public enum NetworkGroups {
	
	Ghoulies=0,
	Coins,
	Gummies,
	Mushrooms,
	Player,
	Orbs,
	Boss,
	NewCell
	
}
