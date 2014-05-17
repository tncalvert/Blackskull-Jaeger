/* Module      : BossTrigger
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Controls moving the player to the boss room
 *
 * Date        : 2014/04/22
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class BossTrigger : MonoBehaviour {

	/// <summary>
	/// The level changer.
	/// </summary>
	private LevelChanger levelChanger;

	void Start() {
		try {
			levelChanger = GameObject.Find ("LevelChanger").GetComponent<LevelChanger> ();
		}
		catch {/* nothing */}
	}

	void OnTriggerEnter(Collider other) {
		if (levelChanger == null || (other.gameObject.name != "Hero(Clone)") && (other.gameObject.name != "HeroNetwork(Clone)"))
				return;
		
		other.networkView.RPC ("LoadBossLevel", RPCMode.Others);
		levelChanger.NetworkLoadBoss();

	}
}
