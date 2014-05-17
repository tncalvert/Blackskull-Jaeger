/* Module      : GhoulieMovement
 * Author      : Josh Morse, Tim Calvert
 * Email       : jbmorse@wpi.edu, tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles the godview minimap generation
 *
 * Date        : 2014/04/01
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class TraversalMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Upon traversal into a new cell, delete
	/// the ceiling to reveal the shape.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "NewCell") {
			GameObject.Destroy(other.gameObject);
		}
		
	}
}
