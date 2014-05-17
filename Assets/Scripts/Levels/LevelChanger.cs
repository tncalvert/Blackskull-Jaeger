/* Module      : LevelChanger
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles loading new level
 *
 * Date        : 2014/04/15
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the loading of new levels
/// </summary>
public class LevelChanger : MonoBehaviour {

	private OnLevelLoadMethod onLoad;

	private bool networkLoadBoss = false;

	public GameObject bossPrefab;
	public GameObject[] orbPrefabs;
	public Vector3[] orbPositions;


	/// <summary>
	/// Preserve through loads
	/// </summary>
	void Awake() {
		GameObject.DontDestroyOnLoad (gameObject);
		onLoad = null;
	}

	/// <summary>
	/// Raises the level was loaded event. Handles populating the level with necessary objects.
	/// </summary>
	/// <param name="level">Level.</param>
	void OnLevelWasLoaded(int level) {
		// Reset messages
		for(int i = 0; i < 8; ++i)
			Network.SetSendingEnabled (i, true);
		Network.isMessageQueueRunning = true;

		if (onLoad != null && !networkLoadBoss) {
			onLoad();
			onLoad = null;
		}else if (networkLoadBoss) {
			// What we do for loading boss over network

			if(Network.isServer) {
				// Server must instantiate everything

				Network.Instantiate(bossPrefab, new Vector3(-13.5559f, 0.1186281f, -5.200117f), Quaternion.Euler(0f, 90f, 0f), (int)NetworkGroups.Boss);
				for(int i = 0; i < 14; ++i) {
					Network.Instantiate(orbPrefabs[i], orbPositions[i], Quaternion.identity, (int)NetworkGroups.Orbs);
				}
			}

			// All instances must move characters appropriately
			GameObject player = GameObject.Find ("Hero(Clone)");
			player.transform.position = new Vector3(2.549501f, 0.5f, -5.16844f);
			player.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			GameObject otherPlayer = GameObject.Find ("HeroNetwork(Clone)");
			if(otherPlayer != null) {
				otherPlayer.transform.position = new Vector3(2.549501f, 0.5f, -5.16844f);
				otherPlayer.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			};

			networkLoadBoss = false;
		}
	}

	/// <summary>
	/// Loads the menu.
	/// </summary>
	public void LoadMenu(OnLevelLoadMethod m=null) {
		onLoad = m;
		//Network.SetLevelPrefix (levelPrefix++);
		Application.LoadLevel ("Menu");
	}

	/// <summary>
	/// Loads the maze.
	/// </summary>
	public void LoadMaze(OnLevelLoadMethod m=null) {
		onLoad = m;
		//Network.SetLevelPrefix (levelPrefix++);
		Application.LoadLevel ("maze");
	}

	/// <summary>
	/// Loads the boss.
	/// </summary>
	public void LoadBoss(OnLevelLoadMethod m=null) {
		onLoad = m;
		//Network.SetLevelPrefix (levelPrefix++);
		Application.LoadLevel ("boss_room");
	}

	/// <summary>
	/// Loads the boss level over the network
	/// </summary>
	/// <param name="m">M.</param>
	public void NetworkLoadBoss(OnLevelLoadMethod m=null) {
		onLoad = m;

		for(int i = 0; i < 8; ++i)  // We have 8 network groups
			Network.SetSendingEnabled (i, false);
		Network.isMessageQueueRunning = false;
		//Network.SetLevelPrefix (levelPrefix++);

		networkLoadBoss = true;
		Application.LoadLevel ("boss_room");

	}
}

/// <summary>
/// A delegate that will be called when a level is loaded. Passed
/// in by whatever calls the function.
/// </summary>
public delegate void OnLevelLoadMethod ();

