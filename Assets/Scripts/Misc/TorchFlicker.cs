/* Module      : PlaceMaze
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : This class is responsible for flickering the fire light.
 *
 * Date        : 2014/04/27
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class TorchFlicker : MonoBehaviour {

	private float baseLight = .5f;
	private float cycle = 0.0f;
	private float flicker = .1f;

	// Update is called once per frame
	void Update () {

		cycle += 2f;
		if(cycle > 360.0f ) cycle = 0.0f;

		light.intensity = baseLight + ((Mathf.Sin(cycle * Mathf.Deg2Rad) * (flicker / 2.0f)) + (flicker / 2.0f));

		if (Random.Range (0f, 1f) > .95f)				
			light.intensity = light.intensity + Random.Range(0.0f, .01f);
	
	}
}
