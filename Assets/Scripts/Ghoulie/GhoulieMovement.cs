/* Module      : GhoulieMovement
 * Author      : Josh Morse, Tim Calvert
 * Email       : jbmorse@wpi.edu, tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Handles moving the ghoulies throughout the world
 *
 * Date        : 2014/04/01
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles moving the ghoulies around the maze
/// </summary>
public class GhoulieMovement : MonoBehaviour {

	/// <summary>
	/// The speed.
	/// </summary>
	float speed = 2.0f;

	/// <summary>
	/// The pathfinder.
	/// </summary>
	public Pathfinder pathfinder;

	/// <summary>
	/// The path.
	/// </summary>
	Queue<Vector3> path;

	/// <summary>
	/// The target.
	/// </summary>
	Vector3 target;

	/// <summary>
	/// The player's GameObject
	/// </summary>
	public GameObject player;

	/// <summary>
	/// The ghoulies handler
	/// </summary>
	public GhoulieHandler ghoulies;

	/// <summary>
	/// The maze x distance
	/// </summary>
	public int mazeX;

	/// <summary>
	/// The maze y distance
	/// </summary>
	public int mazeY;

	private float height;

	private Animator animator;

	private bool playingAnim = false;

	/// <summary>
	/// Capture the height
	/// </summary>
	void Awake() {
		if (networkView.isMine) {
			animator = GetComponent<Animator>();
			height = transform.position.y;
		}
	}

	/// <summary>
	/// Initialize the starting direction
	/// </summary>
	void Start () {
		if(networkView.isMine) {
			ghoulies = GameObject.Find ("GhoulieHandler").GetComponent<GhoulieHandler> ();
			ghoulies.addGhoulie (this);
	
			player = GameObject.Find ("Hero(Clone)");
	
			pathfinder = GameObject.Find ("Pathfinder").GetComponent<Pathfinder>();
			path = pathfinder.getPath (new Vector2 (transform.position.x, transform.position.z),
			                           new Vector2 (Random.Range (0, mazeX) * 3, Random.Range (0, mazeY) * 3), height);
			if(path.Count != 0)
				target = path.Dequeue ();
		}
	}

	/// <summary>
	/// Removes object from tracking list
	/// </summary>
	void OnDestroy() {
		if(networkView.isMine)
			ghoulies.removeGhoulie (this);
	}
	
	/// <summary>
	/// Move the ghoulie in the direction each frame
	/// </summary>
	void Update () {
		if(networkView.isMine) {

			if (Random.Range(0, 10000) < 2) {
				audio.Play();
				networkView.RPC ("playAudio", RPCMode.Others);
			}

			if (path.Count == 0) {
				animator.SetInteger("State", 0);
				path = pathfinder.getPath (new Vector2(transform.position.x, transform.position.z),
				                           new Vector2(Random.Range (0, mazeX) * 3, Random.Range (0, mazeY) * 3), height);
				if(path.Count != 0)
					target = path.Dequeue ();
			}

			if(!playingAnim)
				animator.SetInteger("State", 1);

			Vector3 dirNorm = (target - transform.position).normalized;
			transform.position += dirNorm * speed * Time.deltaTime;
			if(dirNorm != Vector3.zero) {
				Vector3 rayPos = transform.position;

				if(name == "Ghoulie2(Clone)") {
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation (-dirNorm), Time.deltaTime * 5f);
				} else {
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation (dirNorm), Time.deltaTime * 5f);
					rayPos += Vector3.up * 2f;
				}

				// Check if player is in front of us and attack if that is the case
				if(Physics.Raycast(rayPos, dirNorm, 3.0f, (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("PlayerNetwork")))) {
					StartCoroutine("PlayAnimOnce", new AnimPlay { newState = 2, endState = 1, waitTime = 1.5f });
				}
			}
	
			if ((target - transform.position).sqrMagnitude < 0.25f) {
				if(path.Count != 0)
					target = path.Dequeue ();
			}
		}
	}

	/// <summary>
	/// Handles collisions with walls and other ghoulies, as well as stealing coins from players
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Character") {
			other.gameObject.SendMessage("LoseCoins");
		}
	}

	IEnumerator PlayAnimOnce(AnimPlay info) {
		playingAnim = true;
		animator.SetInteger ("State", info.newState);
		yield return new WaitForSeconds(info.waitTime);
		playingAnim = false;
		animator.SetInteger ("State", info.endState);
	}

	/// <summary>
	/// Called after the player picks up a mushroom and handles either
	/// getting a path to the player or getting a path away from the player
	/// </summary>
	public void PickedUp(MushroomType type, Vector3 target) {

		if (type == MushroomType.Bad) {
			path = pathfinder.getPath (new Vector2 (transform.position.x, transform.position.z),
			                           new Vector2 (target.x, target.z), height);
			path.Enqueue (new Vector3 (target.x, 1f, target.z));

		} else if (type == MushroomType.Good) {

			// Pick a value away from the player and path there
			int xCoord, yCoord;
			if(target.x <= transform.position.x) {
				xCoord = Random.Range (0, Mathf.Max (0, (int)target.x - 1));
			} else {
				xCoord = Random.Range(Mathf.Min(mazeX, (int)target.x + 1), mazeX);
			}
			if(target.y <= transform.position.y) {
				yCoord = Random.Range (0, Mathf.Max (0, (int)target.y - 1));
			} else {
				yCoord = Random.Range(Mathf.Min(mazeY, (int)target.y + 1), mazeY);
			}

			xCoord *= 3;
			yCoord *= 3;

			path = pathfinder.getPath (new Vector2 (transform.position.x, transform.position.z),
                   	new Vector2 (xCoord, yCoord), height);
		}

		if (path.Count != 0)
			this.target = path.Dequeue ();

	}

	[RPC]
	public void playAudio() {
		audio.Play ();
	}


}

public struct AnimPlay {
	public int newState;
	public int endState;
	public float waitTime;
}
