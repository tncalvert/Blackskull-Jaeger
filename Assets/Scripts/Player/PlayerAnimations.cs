/* Module      : PlayerAnimations
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles player animations
 *
 * Date        : 2014/05/06
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player animations
/// </summary>
public class PlayerAnimations : MonoBehaviour {

	/// <summary>
	/// The animator.
	/// </summary>
	private Animator animator;

	/// <summary>
	/// The second idle timeout.
	/// </summary>
	private float secondIdleTimeout = 10.0f;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		if (networkView.isMine)
			animator = GetComponent<Animator> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		if (networkView.isMine) {

			if((Input.GetAxis ("ThrowRed") != 0 || Input.GetAxis ("ThrowGreen") != 0 || Input.GetAxis ("ThrowBlue") != 0) && Input.GetAxis ("Shift") == 0) {

				animator.SetInteger("State", 3);

			}else if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

				animator.SetInteger("State", 2);

			} else {
				secondIdleTimeout -= Time.deltaTime;

				if(secondIdleTimeout <= 0.0f || secondIdleTimeout >= 10.0f) {
					animator.SetInteger("State", 1);
					if(secondIdleTimeout <= 0.0f)
						secondIdleTimeout = 11.0f;
				} else {
					animator.SetInteger("State", 0);
				}

			}
		}
	}
}
