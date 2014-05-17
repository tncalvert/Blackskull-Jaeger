/* Module      : GhoulieColorMatching
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Records the appeasement pattern for the ghoulie
 * 				 and tracks attempts to hit them
 *
 * Date        : 2014/04/04
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Tracks ghoulie color and hits
/// </summary>
public class GhoulieColorMatching : MonoBehaviour {

	/// <summary>
	/// Array (length 3) of colors needed to appease the ghoulie
	/// </summary>
	private GhoulieColor[] colors;

	/// <summary>
	/// The index of the color we are looking to match
	/// </summary>
	private int colorIdx;

	/// <summary>
	/// The helmet renderer.
	/// </summary>
	private ParticleSystem smokeRenderer;

	/// <summary>
	/// Set up colors
	/// </summary>
	void Start () {
		colorIdx = 0;
		colors = new GhoulieColor[3];

		smokeRenderer = particleSystem; 

		generateNewColors ();
		setGhoulieColor ();

		if (!networkView.isMine) {
			networkView.RPC ("RemoteGetColors", RPCMode.Others);
		}
	}

	/// <summary>
	/// Called when something collides with the ghoulie (i.e., a gummy)
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if (networkView.isMine) {

			bool gotHit = false;

			GameObject g = other.gameObject;
			if (g.layer != LayerMask.NameToLayer ("Gummies"))
				return;
			// else its gummy

			GhoulieColor gummyColor = g.GetComponent<GummyColor> ().color;

			if (colors [colorIdx] == gummyColor) {
				// Good hit
				++colorIdx;
				Network.Destroy (g);
				gotHit = true;
			}
			// else ignore for now

			if (colorIdx >= 3) {
				Network.Destroy (gameObject);
				return;
			}

			if(gotHit)
				networkView.RPC ("HitByDagger", RPCMode.Others);

			setGhoulieColor ();
		}
	}

	/// <summary>
	/// Generates a new set of colors
	/// </summary>
	private void generateNewColors() {
		colors [0] = (GhoulieColor)Random.Range (0, 3);
		colors [1] = (GhoulieColor)Random.Range (0, 3);
		colors [2] = (GhoulieColor)Random.Range (0, 3);
	}

	private void setGhoulieColor() {
		if (colorIdx < 0 || colorIdx >= 3)
			return;

		if (colors [colorIdx] == GhoulieColor.Red)
			smokeRenderer.startColor = new Color(.55f, .04f, .04f, .5f);
		else if (colors [colorIdx] == GhoulieColor.Blue)
			smokeRenderer.startColor = new Color(.04f, .16f, .55f, .5f);
		else if (colors [colorIdx] == GhoulieColor.Green)
			smokeRenderer.startColor = new Color(.04f, .55f, .04f, .5f);
	}

	[RPC]
	public void RemoteGetColors() {
		Vector3 colors = new Vector3 ();
		colors.x = (int)this.colors [0];
		colors.y = (int)this.colors [1];
		colors.z = (int)this.colors [2];
		networkView.RPC ("RemoteSetColors", RPCMode.Others, colors);
	}

	[RPC]
	public void RemoteSetColors(Vector3 colors) {
		this.colors [0] = (GhoulieColor)((int)colors.x);
		this.colors [1] = (GhoulieColor)((int)colors.y);
		this.colors [2] = (GhoulieColor)((int)colors.z);
		setGhoulieColor ();
	}

	[RPC]
	public void HitByDagger() {
		++colorIdx;
		setGhoulieColor ();
	}
}

/// <summary>
/// Enum denoting an appeasement color
/// </summary>
public enum GhoulieColor {
	Red = 0,
	Green = 1,
	Blue = 2
}
