/* Module      : PlaceMaze
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : This class is responsible for placing the waypoints laid down for the boss
 *
 * Date        : 2014/05/03
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

public class Boss_Waypoint_Generator : MonoBehaviour {

	/// <summary>
	/// The pathfinder so we can place waypoints
	/// </summary>
	public Pathfinder pathfinder;

	/// <summary>
	/// Make all the waypoints before anything happens in the scene
	/// </summary>
	void Awake () {

		pathfinder = GameObject.Find ("Pathfinder").GetComponent<Pathfinder> ();

		//middle of room
	    pathfinder.addWaypoint(new Vector2(-13f, -5f));
		pathfinder.addWaypoint(new Vector2(-10f, -5f));
		pathfinder.addWaypoint(new Vector2(-7f, -5f));
		pathfinder.addWaypoint(new Vector2(-4f, -5f));
		pathfinder.addWaypoint(new Vector2(-1f, -5f));
		pathfinder.addWaypoint(new Vector2(-13f, -8f));
		pathfinder.addWaypoint(new Vector2(-10f, -8f));
		pathfinder.addWaypoint(new Vector2(-7f, -8f));
		pathfinder.addWaypoint(new Vector2(-4f, -8f));
		pathfinder.addWaypoint(new Vector2(-1f, -8f));
		pathfinder.addWaypoint(new Vector2(-13f, -2f));
		pathfinder.addWaypoint(new Vector2(-10f, -2f));
		pathfinder.addWaypoint(new Vector2(-7f, -2f));
		pathfinder.addWaypoint(new Vector2(-4f, -2f));
		pathfinder.addWaypoint(new Vector2(-1f, -2f));

		//left side third row
		pathfinder.addWaypoint(new Vector2(-14f, -15f));
		pathfinder.addWaypoint(new Vector2(-10f, -15f));
		pathfinder.addWaypoint(new Vector2(-5f, -15f));
		pathfinder.addWaypoint(new Vector2(-.5f, -15f));
		pathfinder.addWaypoint(new Vector2(4.5f, -15f));
		pathfinder.addWaypoint(new Vector2(8.8f, -15f));
		pathfinder.addWaypoint(new Vector2(13.5f, -15f));
		pathfinder.addWaypoint(new Vector2(18.2f, -15f));

		//left side second row
		pathfinder.addWaypoint(new Vector2(-14f, -18.7f));
		pathfinder.addWaypoint(new Vector2(-10f, -18.7f));
		pathfinder.addWaypoint(new Vector2(-5f, -18.7f));
		pathfinder.addWaypoint(new Vector2(-.5f, -18.7f));
		pathfinder.addWaypoint(new Vector2(4.5f, -18.7f));
		pathfinder.addWaypoint(new Vector2(8.8f, -18.7f));
		pathfinder.addWaypoint(new Vector2(13.5f, -18.7f));
		pathfinder.addWaypoint(new Vector2(18.2f, -18.7f));

		//right side first row
		pathfinder.addWaypoint(new Vector2(-14f, 4f));
		pathfinder.addWaypoint(new Vector2(-10f, 4f));
		pathfinder.addWaypoint(new Vector2(-5f, 4f));
		pathfinder.addWaypoint(new Vector2(-.5f, 4f));
		pathfinder.addWaypoint(new Vector2(4.5f, 4f));
		pathfinder.addWaypoint(new Vector2(8.8f, 4f));
		pathfinder.addWaypoint(new Vector2(13.5f, 4f));
		pathfinder.addWaypoint(new Vector2(18.2f, 4f));

		//right side second row
		pathfinder.addWaypoint(new Vector2(-14f, 7.6f));
		pathfinder.addWaypoint(new Vector2(-10f, 7.6f));
		pathfinder.addWaypoint(new Vector2(-5f, 7.6f));
		pathfinder.addWaypoint(new Vector2(-.5f, 7.6f));
		pathfinder.addWaypoint(new Vector2(4.5f, 7.6f));
		pathfinder.addWaypoint(new Vector2(8.8f, 7.6f));
		pathfinder.addWaypoint(new Vector2(13.5f, 7.6f));
		pathfinder.addWaypoint(new Vector2(18.2f, 7.6f));

		//Back of room
		pathfinder.addWaypoint(new Vector2(18.2f, -11f));
		pathfinder.addWaypoint(new Vector2(18.2f, -8f));
		pathfinder.addWaypoint(new Vector2(18.2f, -5f));
		pathfinder.addWaypoint(new Vector2(18.2f, -2f));
		pathfinder.addWaypoint(new Vector2(18.2f, 1f));


	}

}
