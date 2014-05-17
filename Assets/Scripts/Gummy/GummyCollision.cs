/* Module      : GummyCollision
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles thown gummies being removed on collision with anything but ghoulies
 *
 * Date        : 2014/04/05
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles collisions with scene objects for gummies
/// </summary>
public class GummyCollision : MonoBehaviour {

	/// <summary>
	/// Indicates that the gummy was thrown
	/// </summary>
	public bool thrown;

	private bool beingDestroyed = false;

	/// <summary>
	/// Handles collisions
	/// </summary>
	/// <param name="other">The other object</param>
	void OnCollisionEnter(Collision other) {
		if (networkView.isMine && !beingDestroyed) {
			if (!thrown || other.gameObject.tag == "Ghoulie")
				return;

			beingDestroyed = true;

			Network.Destroy (gameObject);
		}
	}
}
