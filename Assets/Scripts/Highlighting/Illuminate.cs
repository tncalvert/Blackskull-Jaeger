/* Module      : Illuminate
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles highlighting an object when it recieves a message to do so.
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Used to illuminate objects that can be picked up
/// </summary>
public class Illuminate : MonoBehaviour {

	/// <summary>
	/// The normal shader.
	/// </summary>
	private Shader normalShader;

	/// <summary>
	/// A shader that looks like a highlight. Self-Illumin/VertexLit
	/// </summary>
	private Shader highlightShader;

	/// <summary>
	/// The time when the object was most recently looked at
	/// </summary>
	private float startTime;

	/// <summary>
	/// Initialize the start time
	/// </summary>
	void Awake() {
		// Set up defaults
		startTime = Time.realtimeSinceStartup;
		normalShader = renderer.material.shader;
		highlightShader = Shader.Find ("Self-Illumin/VertexLit");
	}

	/// <summary>
	/// Called via message to highlight the object
	/// Currently just sets renderer color to white
	/// </summary>
	void StartHighlighting() {
		renderer.material.shader = highlightShader;
		startTime = Time.realtimeSinceStartup;
	}

	/// <summary>
	/// Revert to original color when time is up
	/// </summary>
	void Update() {
		// If half a second has passed set the renderer color back to normal
		if(Time.realtimeSinceStartup - startTime > 0.2f) {
			renderer.material.shader = normalShader;
		}
	}
}
