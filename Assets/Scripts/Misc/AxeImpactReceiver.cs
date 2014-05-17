/* Module      : GhoulieMovement
 * Author      : Josh Morse, Tim Calvert
 * Email       : jbmorse@wpi.edu, tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles moving the character when hit by the axe
 *
 * Date        : 2014/05/04
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Axe impact receiver.
/// </summary>
public class AxeImpactReceiver: MonoBehaviour {

	/// <summary>
	/// The impact direction
	/// </summary>
	Vector3 impact = Vector3.zero;

	/// <summary>
	/// The character controller
	/// </summary>
	CharacterController character;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start(){
		character = GetComponent<CharacterController>();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){ 
		if (impact.magnitude > 0.2f){
			character.Move(impact * Time.deltaTime);
		}
		impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
	}

	/// <summary>
	/// Applies the specified force.
	/// </summary>
	/// <param name="force">Force.</param>
	public void Impact(Vector3 force) { 
		impact += force;
	}

	/// <summary>
	/// Indicates axe hit over network
	/// </summary>
	/// <param name="force">Force.</param>
	[RPC]
	public void HitByAxe(Vector3 force) {
		Impact (force);
	}

}

