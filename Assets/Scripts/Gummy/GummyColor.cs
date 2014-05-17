/* Module      : GummyColor
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Holds the color value of the gummy
 *
 * Date        : 2014/04/04
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Just used to hold the color value of the gummy
/// </summary>
public class GummyColor : MonoBehaviour {

	/// <summary>
	/// The gummy's color.
	/// </summary>
	public GhoulieColor color;

	/// <summary>
	/// The owner of the dagger, 1 for player 1, 2 for player 2
	/// </summary>
	public int owner;

	[RPC]
	public void SetOwner(int owner) {
		this.owner = owner;
	}
}
