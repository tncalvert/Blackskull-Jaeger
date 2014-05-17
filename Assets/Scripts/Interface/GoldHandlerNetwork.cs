/* Module      : GoldHandlerNetwork
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles gold for clients
 *
 * Date        : 2014/05/06
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Gold handler for networked clients
/// </summary>
public class GoldHandlerNetwork : MonoBehaviour, IGoldHandler {

	/// <summary>
	/// The hero gold handler.
	/// </summary>
	private GoldHandler heroGoldHandler;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		heroGoldHandler = GameObject.Find ("Hero(Clone)").GetComponent<GoldHandler> ();
	}

	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if (networkView.isMine) {
			if (other.gameObject.tag == "Coin") {
				Network.Destroy(other.gameObject);
				heroGoldHandler.points++;
				networkView.RPC ("RemoteCoinPickup", RPCMode.Others);
			}
		}
	}

	/// <summary>
	/// Loses the coins.
	/// </summary>
	void LoseCoins() {
		if (networkView.isMine) {
			heroGoldHandler.networkView.RPC ("RemoteLoseCoins", RPCMode.Others);
		}
	}

	/// <summary>
	/// Gets the points.
	/// </summary>
	/// <returns>The points.</returns>
	public int GetPoints() {
		return heroGoldHandler.points;
	}

	/// <summary>
	/// Coin pickup for RPC messages
	/// </summary>
	[RPC]
	public void RemoteCoinPickup() {
		heroGoldHandler.points++;
	}

	/// <summary>
	/// Lost coins for RPC messages
	/// </summary>
	/// <param name="amount">Amount.</param>
	[RPC]
	public void RemoteLoseCoins(int amount) {
		heroGoldHandler.points -= amount;
	}
	
	
}