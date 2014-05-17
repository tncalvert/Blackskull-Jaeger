/* Module      : Pathfinder
 * Author      : Tim Calvert
 * Email       : tncalvert@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : This class is responsible handling pathfinding through the maze
 *
 * Date        : 2014/04/08
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Responsible for handling pathfinding through the maze
/// </summary>
public class Pathfinder : MonoBehaviour {

	/// <summary>
	/// The waypoints in the maze
	/// </summary>
	public List<Waypoint> waypoints = new List<Waypoint>();

	/// <summary>
	/// Used to manage an identifier for waypoints
	/// </summary>
	private int id = 0;

	/// <summary>
	/// Finds a path from start to end.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="start">Start position.</param>
	/// <param name="end">End position.</param>
	/// <param name="objYVal">The y value of the object that is being pathed</param> 
	public Queue<Vector3> getPath(Vector2 start, Vector2 end, float objYVal) {

		Waypoint first, last;
		first = getClosestWaypoint (start);
		last = getClosestWaypoint (end);

		if (first == null || last == null) {
			// No vaild path
			return new Queue<Vector3>();
		}

		// Reset everything
		foreach (Waypoint w in waypoints) {
			w.parent = null;
			w.fCost = 0;
			w.gCost = 0;
		}

		List<Waypoint> path = runAStar (first, last);

		return new Queue<Vector3>(path.Select (w => toVec3 (w.position, objYVal)));

	}

	/// <summary>
	/// Converts a Vector2 to a Vector3
	/// </summary>
	/// <returns>The new vector 3</returns>
	/// <param name="v">The Vector2 to convert</param>
	/// <param name="yVal">The y value to put into the new vector</param> 
	private Vector3 toVec3(Vector2 v, float yVal) {
		return new Vector3 (v.x, yVal, v.y);
	}

	/// <summary>
	/// Runs A*.
	/// </summary>
	/// <returns>The path as a list of waypoints</returns>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	private List<Waypoint> runAStar(Waypoint start, Waypoint end) {

		List<Waypoint> closedSet = new List<Waypoint> ();
		// Open set must be sorted by cost
		List<Waypoint> openSet = new List<Waypoint> ();
		openSet.Add (start);

		start.gCost = 0;
		start.fCost = start.gCost + start.heuristicTo (end);

		while (openSet.Count > 0) {

			Waypoint curr = openSet[0];
			openSet.Remove (curr);

			if(curr.Equals (end)) {
				break;
			}

			foreach(Waypoint succ in curr.getChildren()) {
				float tempCost = curr.gCost + curr.costTo(succ);

				if((openSet.Contains(succ) && succ.gCost <= tempCost) ||
				   (closedSet.Contains(succ) && succ.gCost <= tempCost)) {
					continue;
				}

				openSet.Remove (succ);
				closedSet.Remove(succ);

				succ.parent = curr;
				succ.gCost = tempCost;
				succ.fCost = succ.gCost + succ.heuristicTo(end);

				openSet.Add (succ);
				openSet.Sort((x, y) => x.fCost.CompareTo(y.fCost));
			}

			closedSet.Add (curr);
		}

		List<Waypoint> path = new List<Waypoint> ();
		Waypoint c = end;
		while (!c.Equals (start)) {
			path.Insert (0, c);
			c = c.parent;
		}

		return path;

	}

	/// <summary>
	/// Adds a waypoint.
	/// </summary>
	/// <param name="pos">The 2D position of the waypoint.</param>
	public void addWaypoint(Vector2 pos) {
		Waypoint wp = new Waypoint (id++, pos);

		foreach (Waypoint w in waypoints) {

			Vector3 dir = new Vector3(w.position.x - pos.x, 0f, w.position.y - pos.y);
			float distance = dir.magnitude;
			dir.Normalize();

			Ray r = new Ray(new Vector3(pos.x, 1f, pos.y), dir);
			bool canSee = !Physics.SphereCast(r, 0.75f, distance, 1 << LayerMask.NameToLayer("Walls"));

			if(canSee) {
				wp.addChild(w, distance);
				w.addChild(wp, distance);
			}
		}

  		waypoints.Add (wp);
	}

	/// <summary>
	/// Gets the closest visible waypoint.
	/// </summary>
	/// <returns>The closest waypoint.</returns>
	/// <param name="pos">Position.</param>
	public Waypoint getClosestWaypoint(Vector2 pos) {
		float shortestDistance = float.MaxValue;
		Waypoint closest = null;

		foreach (Waypoint w in waypoints) {

			Vector3 dir = new Vector3(w.position.x - pos.x, 0f, w.position.y - pos.y);
			float distance = dir.magnitude;
			dir.Normalize();
			
			Ray r = new Ray(new Vector3(pos.x, 1f, pos.y), dir);
			bool canSee = !Physics.SphereCast(r, 0.75f, distance, 1 << LayerMask.NameToLayer("Walls"));

			if(canSee && distance < shortestDistance) {
				shortestDistance = distance;
				closest = w;
			}
		
		}

		return closest;
	}
}

/// <summary>
/// A waypoint in the maze
/// </summary>
public class Waypoint {

	/// <summary>
	/// The identifier.
	/// </summary>
	public int id;

	/// <summary>
	/// Two dimensional position. Our object won't be moving along the y-axis
	/// so there's no need to include it.
	/// </summary>
	public Vector2 position;

	/// <summary>
	/// The parent of this waypoint in the path being generated.
	/// Will be set and then cleared during path creation.
	/// </summary>
	public Waypoint parent;

	/// <summary>
	/// The estimated cost of this path
	/// </summary>
	public float fCost;

	/// <summary>
	/// Cost from starting node to this node
	/// </summary>
	public float gCost;

	/// <summary>
	/// The children.
	/// </summary>
	private List<Waypoint> children;

	/// <summary>
	/// The cost table to children
	/// </summary>
	private Dictionary<Waypoint, float> costTable;

	/// <summary>
	/// Gets the manhatten distance between two waypoints
	/// </summary>
	/// <returns>The heuristic value</returns>
	/// <param name="other">Another waypoint</param>
	public float heuristicTo(Waypoint other) {
		return (this.position.x - other.position.x) + (this.position.y - other.position.y);
	}

	/// <summary>
	/// Gets the cost to another point
	/// </summary>
	/// <returns>The cost.</returns>
	/// <param name="other">Another waypoint</param>
	public float costTo(Waypoint other) {
		if (costTable.ContainsKey (other)) {
			return costTable[other];
		} else {
			return float.PositiveInfinity;
		}
	}

	/// <summary>
	/// Adds the child with the given cost. If the waypoint already exists
	/// but the new cost is lower, the value is updated.
	/// </summary>
	/// <param name="wp">The child</param>
	/// <param name="cost">The cost to the child</param>
	public void addChild(Waypoint wp, float cost) {
		if (costTable.ContainsKey (wp) && cost < costTable [wp]) {
			costTable [wp] = cost;
		} else if (!costTable.ContainsKey (wp)) {
			children.Add(wp);
			costTable.Add (wp, cost);
		}
	}

	/// <summary>
	/// Gets the children.
	/// </summary>
	/// <returns>The children.</returns>
	public List<Waypoint> getChildren() {
		return children;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Waypoint"/> class.
	/// </summary>
	/// <param name="id">An id</param>
	/// <param name="pos">Position.</param>
	public Waypoint(int id, Vector2 pos) {
		this.id = id;
		position = pos;
		costTable = new Dictionary<Waypoint, float> ();
		children = new List<Waypoint> ();
	}

	public override int GetHashCode ()
	{
		return this.id * 17 + position.GetHashCode();
	}

	public override bool Equals (object obj)
	{
		Waypoint other = (obj as Waypoint);
		if (other == null)
			return false;

		return position.x == other.position.x && position.y == other.position.y;
	}

}
