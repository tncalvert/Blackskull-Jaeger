/* Module      : Cheats
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Cheats
 *
 * Date        : 2014/04/15
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cheats.
/// </summary>
public class Cheats : MonoBehaviour {

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
	
	// Update is called once per frame
	void Update () {

		if (levelChanger == null)
						return;

		if(Input.GetKeyDown(KeyCode.Quote)) {
			GameObject.Find ("Hero(Clone)").networkView.RPC ("LoadBossLevel", RPCMode.Others);
			levelChanger.NetworkLoadBoss();
		}
	
	}
}
