/* Module      : MushroomTypeHandler
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Simple script to deal with mushroom types and informing the player of the type when picked up
 *
 * Date        : 2014/04/01
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the type of a mushroom and informing the player of the type when picked up
/// </summary>
public class MushroomTypeHandler : MonoBehaviour {

	/// <summary>
	/// The type of mushroom, see <see cref="MushroomType"/> 
	/// </summary>
	private MushroomType type;

	void Start() {
		// Get correct type for mushrooms over the network
		if(!networkView.isMine)
			networkView.RPC ("RemoteGetType", RPCMode.Others);
	}

	/// <summary>
	/// Called via SendMessage when the player picks up a mushroom
	/// </summary>
	public void PickedUp(MushroomHandler player) {
		player.SetMushroomType (this.type);
	}

	/// <summary>
	/// Sets the type of mushroom
	/// </summary>
	/// <param name="type">The type of mushroom</param>
	public void SetMushroomType(MushroomType type) {
		this.type = type;
	}

	/// <summary>
	/// Gets the type of mushroom
	/// </summary>
	/// <returns>The type of mushroom</returns>
	public MushroomType GetMushroomType() {
		return type;
	}

	[RPC]
	public void RemoteGetType() {
		networkView.RPC ("RemoteSetType", RPCMode.Others, (int)type);
	}

	[RPC]
	public void RemoteSetType(int type) {
		this.type = (MushroomType)type;
	}

}

/// <summary>
/// Enum for mushroom type
/// </summary>
public enum MushroomType {
	None = 0,
	Good,
	Bad
}
