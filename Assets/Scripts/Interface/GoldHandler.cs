/* Module      : GoldHandler
 * Author      : Josh Morse
 * Email       : jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Listens for collision with gold coins, then handles
 * 				 tracking and displaying the number of coins collected.
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Watches for collisions and updates and displays gold count
/// </summary>
public class GoldHandler : MonoBehaviour, IGoldHandler {
	
	/// <summary>
	/// prefab for gold
	/// </summary>
	public GameObject coinPrefab;
	
	/// <summary>
	/// The number of points the player currently has
	/// </summary>
	public int points;
	
	
	/// <summary>
	/// The coins lost whenever collided with a ghoulie
	/// </summary>
	private int lostCoins = 3;
	
	/// <summary>
	/// The number of points the player starts with
	/// </summary>
	public int startingPoints = 0;
	
	/// <summary>
	/// The maze gameObject (useful for getting maze information)
	/// </summary>
	private MazeGeneration maze;
	
	/// <summary>
	/// Initialize the starting gold points
	/// </summary>
	void Start () {
		if (networkView.isMine) {
			points = startingPoints;
			maze = GameObject.Find ("MazeGenerator").GetComponent<MazeGeneration> ();
		} else {
			networkView.RPC ("RequestCoinCount", RPCMode.Others);
		}
	}
	
	/// <summary>
	/// Handle collisions with gold pieces, removing them and
	/// incrementing the gold points.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if(networkView.isMine) {
			if (other.gameObject.tag == "Coin") {
				Network.Destroy(other.gameObject);
				points++;
				networkView.RPC ("RemoteCoinPickup", RPCMode.Others);
			}
		}
	}
	
	/// <summary>
	/// Upon collision with a ghoul, lose coins.
	/// </summary>
	public void LoseCoins() {
		if(networkView.isMine) {
			int coinAmount = lostCoins;
			
			if (points < coinAmount) {
				coinAmount = points;
			}
			
			points -= coinAmount;
			
			replaceCoins (coinAmount);

			networkView.RPC ("SendCoinCount", RPCMode.Others, points);
		}
	}
	
	/// <summary>
	/// Replaces the coins lost.
	/// </summary>
	/// <param name="amount">Amount.</param>
	void replaceCoins(int amount) {
		
		int randomX = Mathf.FloorToInt (Random.Range (0, maze.maze.width));
		int randomY = Mathf.FloorToInt (Random.Range (0, maze.maze.height));
		
		for (int i = 0; i < amount; i++) {
			
			float offsetX = Random.Range (-1f,1f);
			float offsetY = Random.Range (-1f,1f);
			
			Network.Instantiate(coinPrefab, new Vector3((3f * (float)randomX) + (offsetX), 0.2f, (3f * (float)randomY) + (offsetY)), Quaternion.identity, (int)NetworkGroups.Coins);
			
		}
		
	}
	
	/// <summary>
	/// Gets the points.
	/// </summary>
	public int GetPoints() {
		return points;
	}
	
	[RPC]
	public void RemoteCoinPickup() {
		points++;
	}
	
	[RPC]
	public void RemoteLoseCoins() {
		LoseCoins ();
	}

	[RPC]
	public void RequestCoinCount() {
		networkView.RPC ("SendCoinCount", RPCMode.Others, points);
	}

	[RPC]
	public void SendCoinCount(int count) {
		points = count;
	}
	
}