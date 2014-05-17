/* Module      : MushroomHandler
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Deals with the type of mushroom effect currently affecting the player
 *
 * Date        : 2014/04/01
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Deals with the type of mushroom effect currently affecting the player
/// </summary>
public class MushroomHandler : MonoBehaviour {

	/// <summary>
	/// The type of the mushroom effect.
	/// </summary>
	private MushroomType effectType;

	/// <summary>
	/// The time that the effect will end
	/// </summary>
	private float endTime;

	/// <summary>
	/// Init values
	/// </summary>
	void Start() {
		if(networkView.isMine) {
			endTime = 0f;
			effectType = MushroomType.None;
		}
	}

	/// <summary>
	/// Sets the effect type
	/// </summary>
	/// <param name="type">The effect type.</param>
	public void SetMushroomType(MushroomType type) {
		this.effectType = type;
		this.endTime = Time.time + 5.9f;
	}

	/// <summary>
	/// Decrements the time remaining
	/// </summary>
	void Update() {
		if(networkView.isMine) {
			float currTime = Time.time;
	
			if (this.endTime != 0f && this.endTime - currTime <= 0f) {
				this.endTime = 0f;
				this.effectType = MushroomType.None;
			}
		}
	}

	/// <summary>
	/// Gets the type of the effect.
	/// </summary>
	/// <returns>The effect type.</returns>
	public MushroomType GetEffectType() {
		return this.effectType;
	}

	/// <summary>
	/// Gets the time remaining.
	/// </summary>
	/// <returns>The time remaining.</returns>
	public int GetTimeRemaining() {
		float time = this.endTime - Time.time;
		if (time < 0f)
			time = 0f;
		return Mathf.FloorToInt(time);
	}
}
