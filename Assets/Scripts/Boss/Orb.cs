/* Module      : Orb
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Deals with the orbs that are used to hurt the boss
 *
 * Date        : 2014/04/15
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {

	public GhoulieColor color;

	public BossActions bossActions;

	/// <summary>
	/// Add the orb to the list of orbs the boss controls.
	/// </summary>
	void Start() {
		if (networkView.isMine) {
			bossActions = GameObject.Find ("Boss2(Clone)").GetComponent<BossActions> ();
			bossActions.addOrb (this);
		}
	}

	/// <summary>
	/// If hit by a dagger, delete the orb if correct color.
	/// also damages the boss.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		GameObject g = other.gameObject;
		if (g.layer != LayerMask.NameToLayer ("Gummies"))
				return;

		GhoulieColor gummyColor = g.GetComponent<GummyColor> ().color;
		if (gummyColor != color)
				return;

		if (bossActions != null) {
			bossActions.currHealth -= 1;
			bossActions.SwitchTargetsOnOrbHit (g.GetComponent<GummyColor> ().owner);
		} else {
			GameObject.Find ("Boss2(Clone)").networkView.RPC ("SwitchTargets", RPCMode.Others, g.GetComponent<GummyColor>().owner);
		}	
		networkView.RPC ("Disable", RPCMode.Others);
		gameObject.SetActive (false);
	}

	[RPC]
	public void Disable() {
		gameObject.SetActive (false);
	}

	[RPC]
	public void Enable() {
		gameObject.SetActive (true);
	}
	
}
