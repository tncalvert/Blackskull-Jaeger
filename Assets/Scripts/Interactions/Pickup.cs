/* Module      : Pickup
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles picking up mushrooms and gummies when the player is looking at them
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Handles picking up valid objects
/// </summary>
public class Pickup : MonoBehaviour {

	/// <summary>
	/// The object that was hit by the collision (what we're looking at)
	/// </summary>
	private GameObject hitObject;
	
	private Camera cam;

	/// <summary>
	/// The ghoulies' handler
	/// </summary>
	public GhoulieHandler ghoulies;

	void Start() {
		if(networkView.isMine) {
			ghoulies = GameObject.Find ("GhoulieHandler").GetComponent<GhoulieHandler> ();
			cam = gameObject.transform.FindChild("Player Camera").GetComponent<Camera>();
		}
	}

	/// <summary>
	/// Finds the closest object in front, and then if it is a valid object
	/// listen for key presses to pickup/eat it
	/// </summary>
	void Update() {
		if(networkView.isMine) {
			// Cast small sphere from center of view
			// Get the mushroom or gummy it hit and send it a message to highlight
			Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
			RaycastHit[] hits = Physics.SphereCastAll (ray, 0.15f, 3.0f,
			                                  ((1 << LayerMask.NameToLayer ("Mushrooms")) |
			 								   (1 << LayerMask.NameToLayer ("Gummies")) |
			 								   (1 << LayerMask.NameToLayer ("Walls"))));
			
			if (hits.Length != 0) {
				if(hits.All (h => LayerMask.LayerToName(h.collider.gameObject.layer) == "Walls")) {
					hitObject = null;
					return;  // All walls, so input won't do us any good, return
				}
	
				// Remove walls and pick order by distance
				hits = hits.Where (h => LayerMask.LayerToName(h.collider.gameObject.layer) != "Walls")
					.OrderBy(h => (h.transform.position - this.transform.position).sqrMagnitude)
					.ToArray();
	
				if(hits.Length == 0)
					hitObject = null;
				else
					hitObject = hits.First().collider.gameObject;
	
				if(hitObject != null && hitObject.tag != "Pickup") {
					// No good, empty the variable
					hitObject = null;
				}
			}
	
			// Handle input
			if (hitObject != null) {
				if (Input.GetButtonDown ("PickMushroom")) {
					if (LayerMask.LayerToName (hitObject.layer) == "Mushrooms") {
						// Good
						hitObject.transform.parent.gameObject.SendMessage("PickedUp", gameObject.GetComponent<MushroomHandler>(), SendMessageOptions.DontRequireReceiver);
						ghoulies.SendMessage("PickedUp",
						                     new GhoulieMushroom {
						                     	type = hitObject.transform.parent.gameObject.GetComponent<MushroomTypeHandler>().GetMushroomType(),
												playerPosition = gameObject.transform.position
											 },
						                     SendMessageOptions.DontRequireReceiver);
						Network.Destroy(hitObject.transform.parent.gameObject);
					}
				} else if (Input.GetButtonDown ("EatGummy")) {
					if (LayerMask.LayerToName (hitObject.layer) == "Gummies") {
						Network.Destroy(hitObject);
					}
				}
			}
		}
	}

	public void RemotePickedUp(MushroomType type, Vector3 pos) {
		ghoulies.SendMessage("PickedUp",
		                     new GhoulieMushroom {
			type = type,
			playerPosition = pos
		},
		SendMessageOptions.DontRequireReceiver);
	}
}

/// <summary>
/// Struct object to pass to Ghoulie Handler when picking up a mushroom
/// Contains the info needed to send ghoulies to the appropriate place
/// </summary>
public class GhoulieMushroom {

	public MushroomType type;
	public Vector3 playerPosition;

}
