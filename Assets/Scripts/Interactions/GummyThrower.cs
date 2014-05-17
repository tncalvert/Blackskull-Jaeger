/* Module      : GummyThrower
 * Author      : Josh Morse
 * Email       : jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Controls spawning gummies on player input, and either
 * 				 dropping or throwing them as necessary.
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles placing gummies on player input
/// </summary>
public class GummyThrower : MonoBehaviour {

	/// <summary>
	/// Gummie prefab
	/// </summary>
	public GameObject gummyRedPrefab, gummyGreenPrefab, gummyBluePrefab;
	
	private Camera cam;

	private bool canThrow = true;

	private int ownerNum = 1;

	// Distance from your player
	float horizontalDistance = 1f;
	// Vertical Distance from the player
	float verticalDistance = 0.125f;
	// Thrown force from the player
	float throwForce = 750f;
	
	void Start() {
		cam = gameObject.transform.FindChild("Player Camera").GetComponent<Camera>();
		if (gameObject.name == "HeroNetwork(Clone)")
			ownerNum = 2;
	}

	/// <summary>
	/// Check for execute any gummy throw commands.
	/// </summary>
	void Update () {
		if(networkView.isMine) {
			GummyColor c;
			if (Input.GetButtonDown("ThrowRed") && canThrow) {
				Vector3 dropPos = transform.TransformPoint((Vector3.forward * 0.025f) + (Vector3.up * verticalDistance));
				if (!Input.GetButton("Shift")) {
					StartCoroutine(ThrowGummy(dropPos, gummyRedPrefab));
				} else {
					c = (Network.Instantiate(gummyRedPrefab, dropPos * horizontalDistance, gameObject.transform.rotation, (int)NetworkGroups.Gummies) as GameObject).GetComponent<GummyColor>();
					c.owner = ownerNum;
				}
			}
			else if (Input.GetButtonDown("ThrowGreen") && canThrow) {
				Vector3 dropPos = transform.TransformPoint((Vector3.forward * 0.025f) + (Vector3.up * verticalDistance));
				if (!Input.GetButton("Shift")) {
					StartCoroutine(ThrowGummy(dropPos, gummyGreenPrefab));
				} else {
					c = (Network.Instantiate(gummyGreenPrefab, dropPos * horizontalDistance, gameObject.transform.rotation, (int)NetworkGroups.Gummies) as GameObject).GetComponent<GummyColor>();
					c.owner = ownerNum;
				}
			}
			else if (Input.GetButtonDown("ThrowBlue") && canThrow) {
				Vector3 dropPos = transform.TransformPoint((Vector3.forward * 0.025f) + (Vector3.up * verticalDistance));
				if (!Input.GetButton("Shift")) {
					StartCoroutine(ThrowGummy(dropPos, gummyBluePrefab));
				} else {
					c = (Network.Instantiate(gummyBluePrefab, dropPos * horizontalDistance, gameObject.transform.rotation, (int)NetworkGroups.Gummies) as GameObject).GetComponent<GummyColor>();
					c.owner = ownerNum;
				}
			}
		}
	}

	/// <summary>
	/// Applies force to a gummy that should be "thrown"
	/// </summary>
	/// <param name="pos">The gummy's position in the world.</param>
	/// <param name="gummy">The gummy object instantiated</param>
	IEnumerator ThrowGummy (Vector3 pos, GameObject gummyPrefab) {
		canThrow = false;
		yield return new WaitForSeconds (0.5f);
		GameObject gummy = (GameObject) Network.Instantiate(gummyPrefab, pos * horizontalDistance, gameObject.transform.rotation, (int)NetworkGroups.Gummies);
		gummy.GetComponent<GummyCollision> ().thrown = true;
		gummy.networkView.RPC ("SetOwner", RPCMode.All, ownerNum);
		Vector3 direction = cam.transform.forward;
		direction.Normalize();
		gummy.rigidbody.AddForce (direction * throwForce);
		yield return new WaitForSeconds (0.6f);
		canThrow = true;
	}
}
