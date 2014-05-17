/* Module      : DaggerRotation
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Spins a dagger end over end when it's thrown
 *
 * Date        : 2014/04/11
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class DaggerRotation : MonoBehaviour {

	/// <summary>
	/// Copy of the thrown variable in GummyCollision
	/// Daggers will only spin if thrown
	/// </summary>
	private bool thrown;

	/// <summary>
	/// Get the thrown variable
	/// </summary>
	void Start () {
		thrown = gameObject.GetComponent<GummyCollision> ().thrown;
	}
	
	/// <summary>
	/// Apply the rotation over time
	/// </summary>
	void Update () {

		if (!thrown)
			return;
	
		rigidbody.AddRelativeTorque (15f, 0f, 0f, ForceMode.Impulse);

	}
}
