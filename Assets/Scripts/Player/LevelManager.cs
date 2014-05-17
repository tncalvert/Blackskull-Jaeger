/* Module      : LevelManager
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Level manager so we can network the boss level load
 *
 * Date        : 2014/05/06
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Level manager
/// </summary>
public class LevelManager : MonoBehaviour {

	/// <summary>
	/// Loads the boss level.
	/// </summary>
	[RPC]
	public void LoadBossLevel() {
		try {
			LevelChanger levelChanger = GameObject.Find ("LevelChanger").GetComponent<LevelChanger> ();
			levelChanger.NetworkLoadBoss();
		} catch { /* nothing */ }
	}
}
