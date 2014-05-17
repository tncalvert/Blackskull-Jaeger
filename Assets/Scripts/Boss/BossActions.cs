/* Module      : BossActions
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Controls the actions of the boss
 *
 * Date        : 2014/04/15
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Boss actions.
/// </summary>
public class BossActions : MonoBehaviour {

	/// <summary>
	/// The Audio clips for the boss.
	/// </summary>
	public AudioClip line1, line2, line3, hit1, 
					 hit2, laugh, death;

	/// <summary>
	/// The player targets.
	/// </summary>
	public Transform playerTarget, player2Target;

	/// <summary>
	/// The current target.
	/// </summary>
	public Transform currentTarget;

	/// <summary>
	/// The animator for the boss.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// The coins that the boss explodes into.
	/// </summary>
	public GameObject coinPhysicsPrefab;

	/// <summary>
	/// The state of the boss.
	/// </summary>
	private State state;

	/// <summary>
	/// The speed at which the boss moves.
	/// </summary>
	public float speed;

	/// <summary>
	/// The times for the boss.
	/// </summary>
	public float idleTime, attackTime, deathTime, targetSwitchTime;

	/// <summary>
	/// The healths for the boss.
	/// </summary>
	public int prevHealth, currHealth;

	/// <summary>
	/// The orb timeout for renewal.
	/// </summary>
	private float orbTimeout;

	/// <summary>
	/// The height of the boss.
	/// </summary>
	private float height;

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
	/// The orbs the boss uses for health (the vials).
	/// </summary>
	List<Orb> orbs = new List<Orb>();

	/// <summary>
	/// Capture the height
	/// </summary>
	void Awake() {
		if(networkView.isMine)
			height = transform.position.y;
		pathfinder = GameObject.Find ("Pathfinder").GetComponent<Pathfinder> ();
		targetSwitchTime = 10.0f;
	}

	/// <summary>
	/// Initialize all the boss state variables.
	/// </summary>
	void Start () {
		if (networkView.isMine) {
			try {
				playerTarget = GameObject.Find ("Hero(Clone)").transform;
			} catch {}
			try {
				player2Target = GameObject.Find ("HeroNetwork(Clone)").transform;
			} catch {}

			if(player2Target != null) {
				// Assume we always have a player1
				//currentTarget = Random.Range (0, 2) == 0 ? playerTarget : player2Target;
				currentTarget = player2Target;
			} else {
				currentTarget = playerTarget;
			}

			idleTime = 5.0f;
			attackTime = 3f;
			deathTime = 5f;
			currHealth = 14;
			speed = 2f;
			prevHealth = currHealth;
			orbTimeout = 0.0f;
			state = State.Idle;
		}
	}
	
	/// <summary>
	/// Each frame, run the state of the boss based on the current state.
	/// </summary>
	void Update () {
		if (currentTarget == null && player2Target == null) {
			// Player 2 left
			currentTarget = playerTarget;
		}

		targetSwitchTime -= Time.deltaTime;

		if (networkView.isMine){
			switch (state) {

			//The boss is idle. This is the starting state, as well as used as inbetween states where the boss is waiting.
			case State.Idle:
				idleTime -= Time.deltaTime;

				if (Random.Range(0, 100) < 1 && !audio.isPlaying) {
					int randSound = Random.Range (0,3);
					if (randSound == 0) {
						audio.clip = line1;
						networkView.RPC ("playAudio", RPCMode.Others, 1);
					}
					else if (randSound == 1) {
						audio.clip = line2;
						networkView.RPC ("playAudio", RPCMode.Others, 2);
					}
					else {
						audio.clip = line3;
						networkView.RPC ("playAudio", RPCMode.Others, 3);
					}
					audio.Play();
				}

				if(idleTime < 0.0f) {
					state = State.Move;
					animator.SetInteger("State", 1);
					path = pathfinder.getPath (new Vector2 (transform.position.x, transform.position.z),
					                           new Vector2 (currentTarget.position.x, currentTarget.position.z), height);
					if(path.Count != 0)
						target = path.Dequeue ();
				}

				if(prevHealth != currHealth) {
					state = State.Hit;
				}

				break;
			
			//The boss is moving. The boss could be moving directly at the player, or A-staring to his location.
			case State.Move:
				
				if((currentTarget.position - transform.position).sqrMagnitude <= 3.0f) {
					animator.SetInteger("State", 2);
					attackTime = 3f;
					state = State.Attack;
				}
				else if((currentTarget.position - transform.position).sqrMagnitude <= 50.0f) {
					Vector3 dirNorm = (currentTarget.position - transform.position).normalized;
					transform.position += dirNorm * speed * Time.deltaTime;
					if(dirNorm != Vector3.zero)
						transform.rotation = Quaternion.LookRotation (dirNorm);
				}
				else if(prevHealth != currHealth) {
					state = State.Hit;
				}

				else {

					if ((target - transform.position).sqrMagnitude > .5f) {
					Vector3 dirNorm = (target - transform.position).normalized;
					transform.position += dirNorm * speed * Time.deltaTime;
					if(dirNorm != Vector3.zero)
						transform.rotation = Quaternion.LookRotation (dirNorm);
					}

					if (path.Count == 0) {
						path = pathfinder.getPath (new Vector2 (transform.position.x, transform.position.z),
						                           new Vector2 (currentTarget.position.x, currentTarget.position.z), height);
						if(path.Count != 0)
							target = path.Dequeue ();
					}
					
					else if ((target - transform.position).sqrMagnitude < 1f) {
						if(path.Count != 0)
							target = path.Dequeue ();
					}

				}

				break;

			//The boss is hit. Reduce his health.
			case State.Hit:

				if (Random.Range(0, 4) < 1 && !audio.isPlaying) {
					int randSound = Random.Range (0,2);
					if (randSound == 0) {
						audio.clip = hit1;
						networkView.RPC ("playAudio", RPCMode.Others, 4);
					}
					else {
						audio.clip = hit2;
						networkView.RPC ("playAudio", RPCMode.Others, 5);
					}
					audio.Play();
				}

				prevHealth = currHealth;

				if(currHealth <= 0) {
					state = State.Die;
				} else {
					state = State.Idle;
					animator.SetInteger("State", 0);
				}

				break;
			
			//The boss died. Kill him and make him explode coins.
			case State.Die:

				if (deathTime == 5f) {
					audio.Pause();
					audio.PlayOneShot(death);
					networkView.RPC ("playAudio", RPCMode.Others, 7);
					particleSystem.Play ();
				}

				deathTime -= Time.deltaTime;

				if (deathTime <= 0f) {
					GameObject coin;
					for (int i = 0; i < 60; i++) {
						coin = (GameObject)Network.Instantiate (coinPhysicsPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, (int)NetworkGroups.Coins);
						coin.rigidbody.AddForce(new Vector3(Random.value, Random.value, Random.value) * 500);
					}
					Network.Destroy(gameObject);

					try {
						GameObject.Find ("Hero(Clone)").networkView.RPC ("Won", RPCMode.All);
						GameObject.Find ("HeroNetwork(Clone)").networkView.RPC ("Won", RPCMode.All);
					} catch {}
				}

				break;

			//The boss is attacking. He will attack for a set amount of time, then go back to idle.
			case State.Attack:
				attackTime -= Time.deltaTime;
				orbTimeout -= Time.deltaTime;

				Vector3 dirNorm1 = (currentTarget.transform.position - transform.position).normalized;
				if(dirNorm1 != Vector3.zero)
					transform.rotation = Quaternion.LookRotation (dirNorm1);
				
				if((currentTarget.position - transform.position).sqrMagnitude > 12.0f) {
					state = State.Idle;
						idleTime = .5f;
					animator.SetInteger("State", 0);
				}
				if(prevHealth != currHealth) {
					state = State.Hit;
				}
				if(attackTime <= 0) {
					state = State.Idle;
					idleTime = 2f;
					animator.SetInteger ("State", 0);
				}

				break;

			}
		}
	}

	/// <summary>
	/// Adds the orb.
	/// </summary>
	/// <param name="o">O.</param>
	public void addOrb(Orb o) {
		orbs.Add(o);
	}

	/// <summary>
	/// Function that resets an orb
	/// </summary>
	private void resetAnOrb() {
		List<Orb> hitOrbs = orbs.FindAll (o => !o.gameObject.activeSelf);
		if (hitOrbs.Count == 0)
			return;
		GameObject orb = hitOrbs [Random.Range (0, hitOrbs.Count)].gameObject;
		orb.SetActive (true);
		orb.networkView.RPC ("Enable", RPCMode.Others);
		currHealth += 1;
		prevHealth = currHealth;
	}

	/// <summary>
	/// This triggers when the boss swings his axe at the player and hits.
	/// The boss will reset an orb and knock the player back.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other) {
		if (networkView.isMine) {
			if (other.gameObject.tag == "Character") {
				if (orbTimeout <= 0.0f) {
					orbTimeout = 2.5f;
					resetAnOrb ();
				}

				if (Random.Range(0, 4) < 1 && !audio.isPlaying) {
					audio.clip = laugh;
					audio.Play();
					networkView.RPC ("playAudio", RPCMode.Others, 6);
				}

				Vector3 dir = other.transform.position - transform.position;
				dir.y = 0; 
				AxeImpactReceiver victim = other.GetComponent<AxeImpactReceiver> ();
				victim.Impact ((dir.normalized * 40f));
				victim.networkView.RPC ("HitByAxe", RPCMode.Others, (dir.normalized * 40f));
			}
		}
	}

	public void SwitchTargetsOnOrbHit(int target) {
		if (targetSwitchTime > 0.0f)
			return;

		targetSwitchTime = 10.0f;

		if (target == 1)
			currentTarget = playerTarget;
		else if (target == 2)
			currentTarget = player2Target;
	}

	[RPC]
	public void SwitchTargets(int target) {
		currHealth -= 1;
		SwitchTargetsOnOrbHit (target);
	}

	[RPC]
	public void playAudio(int clip) {
		if (clip == 1)
			audio.PlayOneShot (line1);
		if (clip == 2)
			audio.PlayOneShot (line2);
		if (clip == 3)
			audio.PlayOneShot (line3);
		if (clip == 4)
			audio.PlayOneShot (hit1);
		if (clip == 5)
			audio.PlayOneShot (hit2);
		if (clip == 6)
			audio.PlayOneShot (laugh);
		if (clip == 7)
			audio.PlayOneShot (death);
	}

	private enum State {
		Idle,
		Hit,
		Die,
		Attack,
		Move
	}
}
