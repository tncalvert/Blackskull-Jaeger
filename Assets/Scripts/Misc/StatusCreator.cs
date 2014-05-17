using UnityEngine;
using System.Collections;

/// <summary>
/// For testing allows us to send a status message on key press
/// </summary>
public class StatusCreator : MonoBehaviour {

	private int i;

	void Start() {
		i = 1;
	}

	void Update() {

		if(Input.GetKeyDown(KeyCode.M)) {

			gameObject.SendMessage ("QueueStatusMessage", ("Another message " + i++), SendMessageOptions.DontRequireReceiver);

		}

	}
}
