/* Module      : CameraOperations
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles switching between the minimap being on or off.
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the change to godview camera
/// </summary>
public class CameraOperations : MonoBehaviour {

	/// <summary>
	/// The godview camera
	/// </summary>
	public Camera gvCamera;

	/// <summary>
	/// Flag to indicate if we are in godview
	/// </summary>
	private bool inGodView = false;

	/// <summary>
	/// Watch for keypresses to toggle the minimap
	/// </summary>
	void Update() {
		if (networkView.isMine) {
				if (Input.GetKeyDown (KeyCode.O)) {
						if (inGodView) {
								gvCamera.depth = 0;
						} else {
								gvCamera.depth = 2;
						}

						inGodView = !inGodView;
				}
		}

	}
}

