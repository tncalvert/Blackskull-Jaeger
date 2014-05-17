/* Module      : GUIHandler
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles displaying the GUI
 *
 * Date        : 2014/04/01
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Responsible for displaying the GUI
/// </summary>
public class GUIHandler : MonoBehaviour {

	/// <summary>
	/// The gold handler.
	/// </summary>
	private IGoldHandler goldHandler;

	/// <summary>
	/// The mushroom handler.
	/// </summary>
	private MushroomHandler mushroomHandler;

	/// <summary>
	/// A queue holding status messages
	/// </summary>
	private Queue<string> statusMessages;

	/// <summary>
	/// The current status message.
	/// </summary>
	private string currentStatusMessage;

	/// <summary>
	/// End time for message
	/// </summary>
	private float messageEndTime;

	/// <summary>
	/// If the players won
	/// </summary>
	private bool wonGame = false;

	void Awake() {
		GameObject.DontDestroyOnLoad (gameObject);
	}

	/// <summary>
	/// Set values
	/// </summary>
	void Start() {
		Camera fpsCamera = transform.FindChild ("Player Camera").GetComponent<Camera> ();
		if (networkView.isMine) {
			// Set camera for my player
			fpsCamera.enabled = true;
		} else {
			// Remove audio listener for the other player
			fpsCamera.enabled = false;
			fpsCamera.GetComponent<AudioListener>().enabled = false;
		}
		if (name == "Hero(Clone)") {
			goldHandler = gameObject.GetComponent<GoldHandler> ();
		} else {
			goldHandler = gameObject.GetComponent<GoldHandlerNetwork>();
		}
		mushroomHandler = gameObject.GetComponent<MushroomHandler> ();
		statusMessages = new Queue<string> ();
		currentStatusMessage = "";
		messageEndTime = 0f;
	
	}

	/// <summary>
	/// Draw the GUI
	/// </summary>
	void OnGUI() {
		if (networkView.isMine) {
			GUI.Label (new Rect (25, 25, 50, 25), "Gold: " + goldHandler.GetPoints ());
			GUI.Label (new Rect (25, 50, 100, 50), "Mushroom:\n" + "\tType: " + mushroomHandler.GetEffectType ()
					+ "\n\tTime Left: " + mushroomHandler.GetTimeRemaining ());

			if(wonGame) {
				int fontSize = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = 22;
				GUI.Label (new Rect(Screen.width / 2 - 75, Screen.height / 2 - 50, 150, 50), "YOU WON!!");
				GUI.skin.label.fontSize = fontSize;
			}

			if (currentStatusMessage != "" && messageEndTime != 0f) {
				GUIStyle prevStyle = new GUIStyle (GUI.skin.label);
				GUIStyle centeredStyle = new GUIStyle (GUI.skin.label);
				centeredStyle.alignment = TextAnchor.MiddleCenter;
				centeredStyle.fontSize = 18;
				GUI.skin.label = centeredStyle;
				GUI.Label (new Rect (Screen.width / 4f, Screen.height * 0.85f, Screen.width / 2f, Screen.height * 0.1f),
					currentStatusMessage);
				GUI.skin.label = prevStyle;
			}
		}
	}

	/// <summary>
	/// Handle tracking time
	/// </summary>
	void Update() {
		float currTime = Time.time;

		if (this.messageEndTime != 0f && this.messageEndTime - currTime <= 0f) {
			if (statusMessages.Count != 0f) {
				currentStatusMessage = statusMessages.Dequeue();
				messageEndTime = Time.time + 5.9f;
			} else {
				messageEndTime = 0f;
				currentStatusMessage = "";
			}
		}
	}

	/// <summary>
	/// Queues a status message to be displayed when possible
	/// </summary>
	/// <param name="message">Message.</param>
	public void QueueStatusMessage(string message) {
		statusMessages.Enqueue (message);
		if (statusMessages.Count != 0f && messageEndTime == 0f && currentStatusMessage == "") {
			currentStatusMessage = statusMessages.Dequeue();
			messageEndTime = Time.time + 5.9f;
		}
	}

	[RPC]
	public void Won() {
		wonGame = true;
	}

}
