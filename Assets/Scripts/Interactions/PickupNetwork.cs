/* Module      : PickupNetwork
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Does pickup actions for clients
 *
 * Date        : 2014/05/06
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Pickup actions for clients
/// </summary>
public class PickupNetwork : MonoBehaviour {

	/// <summary>
	/// The object that was hit by the collision (what we're looking at)
	/// </summary>
	private GameObject hitObject;

	/// <summary>
	/// The cam.
	/// </summary>
	private Camera cam;

	/// <summary>
	/// The hero pickup.
	/// </summary>
	private Pickup heroPickup;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		if(networkView.isMine) {
			cam = gameObject.transform.FindChild("Player Camera").GetComponent<Camera>();
		}
		heroPickup = GameObject.Find ("Hero(Clone)").GetComponent<Pickup>();
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
						networkView.RPC ("PickedUp", RPCMode.Others,
						                 (int)(hitObject.transform.parent.gameObject.GetComponent<MushroomTypeHandler>().GetMushroomType()),
						                 this.transform.position);
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

	/// <summary>
	/// Pickes up RPC
	/// </summary>
	/// <param name="mushroomType">Mushroom type.</param>
	/// <param name="targetPosition">Target position.</param>
	[RPC]
	public void PickedUp(int mushroomType, Vector3 targetPosition) {
		heroPickup.RemotePickedUp ((MushroomType)mushroomType, targetPosition);
	}

}
