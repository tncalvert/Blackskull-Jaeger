/* Module      : Highlighter
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles tracking objects in front of the player
 * 				 and sending messages for them to highlight
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows the player to highlight objects when looking at them
/// </summary>
public class Highlighter : MonoBehaviour {

	private Camera cam;
	
	void Start() {
		cam = gameObject.transform.FindChild("Player Camera").GetComponent<Camera>();
	}

	/// <summary>
	/// Finds objects in front and sends a message to highlight
	/// </summary>
	void Update() {
		// Cast small sphere from center of view
		// Get the mushroom or gummy it hit and send it a message to highlight
		Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		RaycastHit hit = new RaycastHit();
		bool didHit = Physics.SphereCast (ray, 0.15f, out hit, 3.0f,
               ((1 << LayerMask.NameToLayer ("Mushrooms")) | (1 << LayerMask.NameToLayer ("Gummies"))));

		if (didHit) {
			hit.collider.gameObject.SendMessage("StartHighlighting", SendMessageOptions.DontRequireReceiver);
		}
	}
}
