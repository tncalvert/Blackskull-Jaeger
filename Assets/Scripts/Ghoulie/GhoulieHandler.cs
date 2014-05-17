/* Module      : GhoulieHandler
 * Author      : Josh Morse, Tim Calvert
 * Email       : jbmorse@wpi.edu, tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles sending messages to the ghoulies throughout the world
 *
 * Date        : 2014/04/11
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global Ghoulie handler that dictates ghoulie movement based on mushroom pickups.
/// </summary>
public class GhoulieHandler : MonoBehaviour {

	/// <summary>
	/// The ghoulies.
	/// </summary>
	public List<GhoulieMovement> ghoulies = new List<GhoulieMovement>();

	/// <summary>
	/// Adds the ghoulie.
	/// </summary>
	/// <param name="g">The ghoulie.</param>
	public void addGhoulie(GhoulieMovement g) {
		ghoulies.Add (g);
	}

	/// <summary>
	/// Removes the ghoulie.
	/// </summary>
	/// <param name="g">The ghoulie.</param>
	public void removeGhoulie(GhoulieMovement g) {
		ghoulies.Remove (g);
	}

	/// <summary>
	/// Called via sendmessage when the player picks up a mushroom
	/// </summary>
	public void PickedUp(GhoulieMushroom gm) {

		foreach(GhoulieMovement g in ghoulies) {
			g.PickedUp(gm.type, gm.playerPosition);
		}
	
	}


}
