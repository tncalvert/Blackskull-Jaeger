﻿/* Module      : PlaceMaze
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : This class is responsible for placing a maze generated by MazeGeneration.cs
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for laying out the generated maze within the scene
/// </summary>
public class PlaceMaze : MonoBehaviour {

	/// <summary>
	/// Building block prefab
	/// </summary>
	public GameObject floorPrefab, wallPrefab, playerPrefab, mushroomPrefab,
				coinPrefab, hallPrefab, crossPrefab, turnPrefab,
				teePrefab, endPrefab, ghouliePrefab, edgePrefab,
				torchPrefab, statuePrefab, bossTrigger, endCellParticle,
				gouliePrefab, firePrefab, playerPrefabNetwork, bossRoomPrefab,
				bossLight, untraversedPrefab, ceilingPrefab;

	/// <summary>
	/// Size of a prefab object
	/// </summary>
	private int size;

	/// <summary>
	/// The pathfinder so we can place waypoints
	/// </summary>
	public Pathfinder pathfinder;

	/// <summary>
	/// We need this to set the game to playing
	/// </summary>
	public MenuScript menu;

	private Maze _maze;

	/// <summary>
	/// Will go through the maze structure and place object as needed
	/// </summary>
	/// <param name="maze">The maze built by MazeGeneration.cs</param>
	public void placeMaze(Maze maze, bool placeNetworkedObjects=true) {

		_maze = maze;

		size = 3;

		placeCells (maze, placeNetworkedObjects);

		if (placeNetworkedObjects) {
			// Place coins/mushrooms/etc.
			for (int i = 0; i < maze.width; ++i) {
				for (int j = 0; j < maze.height; ++j) {
					//Cell cell = maze.cells[i, j];

					// Coins
					if (maze.cells [i, j].hasCoin && maze.startingCell.pos.x != i && maze.startingCell.pos.y != j) {
						float offsetX = Random.Range (-1f, 1f);
						float offsetY = Random.Range (-1f, 1f);
						Network.Instantiate (coinPrefab, new Vector3 ((size * i) + offsetX, 0.07f, (size * j) + offsetY), Quaternion.identity, (int)NetworkGroups.Coins);
					}

					// Mushrooms
					if (maze.cells [i, j].hasMushroom != 0 && maze.startingCell.pos.x != i && maze.startingCell.pos.y != j) {

						Vector3 toPlacePoint = new Vector3 ((size / 2f) * 0.85f, 0.0f, 0.0f);
						int rotAmount = Random.Range (0, 360);
						toPlacePoint = Quaternion.AngleAxis (rotAmount, Vector3.up) * toPlacePoint;
						toPlacePoint = Vector3.ClampMagnitude (toPlacePoint, 0.8f * toPlacePoint.magnitude);
						GameObject m = (GameObject)Network.Instantiate (mushroomPrefab, new Vector3 (size * i, 0.007771775f, size * j) + toPlacePoint, Quaternion.identity, (int)NetworkGroups.Mushrooms);
						MushroomTypeHandler mt = m.GetComponent<MushroomTypeHandler> ();
						if (maze.cells [i, j].hasMushroom == 1)
							mt.SetMushroomType (MushroomType.Good);
						else
							mt.SetMushroomType (MushroomType.Bad);
					}

					GhoulieMovement gm;
					if (maze.cells [i, j].hasGhoulie && maze.startingCell.pos.x != i && maze.startingCell.pos.y != j) {
						if (Random.Range (0f, 1f) > .5f)
							gm = (Network.Instantiate (ghouliePrefab, new Vector3 (size * i, -1f, size * j), Quaternion.identity, (int)NetworkGroups.Ghoulies) as GameObject).GetComponent<GhoulieMovement> ();
						else 
							gm = (Network.Instantiate (gouliePrefab, new Vector3 (size * i, 1f, size * j), Quaternion.identity, (int)NetworkGroups.Ghoulies) as GameObject).GetComponent<GhoulieMovement> ();
						gm.mazeX = maze.width;
						gm.mazeY = maze.height;
					}

				}
			}
		}
			
		int openWall = 0;
		if (maze.startingCell.openWalls [1])
			openWall = 1;
		if (maze.startingCell.openWalls [2])
			openWall = 2;
		if (maze.startingCell.openWalls [3])
			openWall = 3;

		// Place player
		if (placeNetworkedObjects) {
			// Server
			Network.Instantiate (playerPrefab, new Vector3 (size * maze.startingCell.pos.x, 2f, size * maze.startingCell.pos.y), Quaternion.AngleAxis (openWall * 90f, Vector3.up), (int)NetworkGroups.Player);
		}
		// Client loading happens below in OnConnectedToServer

		//Place end
		Instantiate(bossRoomPrefab, new Vector3((size * maze.endingCell.pos.x)+13.4f, -.12f, (size * maze.endingCell.pos.y)+1.5f), Quaternion.AngleAxis (180f, Vector3.up));
		Instantiate(bossLight, new Vector3((size * maze.endingCell.pos.x)+20f, 10f, (size * maze.endingCell.pos.y)+1.5f), Quaternion.identity);
		Instantiate (bossTrigger, new Vector3 ((size * maze.endingCell.pos.x)+10f, 1.25f, (size * maze.endingCell.pos.y)+1.5f), Quaternion.identity);
		Instantiate (endCellParticle, new Vector3 ((size * maze.endingCell.pos.x+10f), 0f, (size * maze.endingCell.pos.y)+1.5f), Quaternion.AngleAxis(270f, transform.right));
		Instantiate (hallPrefab, new Vector3(size * maze.endingCell.pos.x + 3f, 2f, (size * maze.endingCell.pos.y)), Quaternion.identity);
		Instantiate (hallPrefab, new Vector3(size * maze.endingCell.pos.x + 3f, 2f, (size * maze.endingCell.pos.y)+3f), Quaternion.identity);


		GameObject menuObject = GameObject.Find ("Menu");
		if (menuObject != null) {
			menu = menuObject.GetComponent<MenuScript> ();
			menu.gameStart = true;
		}

	}

	/// <summary>
	/// Loops through all the cells in the maze, placing the proper cell prefab in the correct orientation.
	/// </summary>
	public void placeCells(Maze maze, bool placeNetworkedObjects) {

		GameObject fix;

		float endTorchChance = .6f;
		float hallTorchChance = .3f;
		float turnTorchChance = .4f;
		float teeTorchChance = .3f;
		float statueChance = .4f;

		for(int i = 0; i < maze.width; ++i) {
			for(int j = 0; j < maze.height; ++j) {
				Cell cell = maze.cells[i, j];

				Instantiate(floorPrefab, new Vector3(size * i, -12f, size * j), Quaternion.identity);
				Instantiate(ceilingPrefab, new Vector3(size * i, -8.8f, size *j), Quaternion.identity);
				for(int x = 0; x < 4; ++x) {
					if(!cell.openWalls[x]) {
						switch(x) {
						case 0:
							Instantiate(wallPrefab, new Vector3(size *i, -10.3f, (float)(size * j) + 1.35f), Quaternion.identity);
							break;
						case 1:
							Instantiate(wallPrefab, new Vector3((float)(size * i) + 1.35f, -10.3f, size * j), Quaternion.AngleAxis(90f, Vector3.up));
							break;
						case 2:
							Instantiate(wallPrefab, new Vector3(size * i, -10.3f, (float)(size * j) - 1.35f), Quaternion.identity);
							break;
						case 3:
							Instantiate(wallPrefab, new Vector3((float)(size * i) - 1.35f, -10.3f, size * j), Quaternion.AngleAxis(90f, Vector3.up));
							break;
						}
					}
				}

				if (cell.openWalls[0]) {
					if (cell.openWalls[1]) {
						if (cell.openWalls[2]) {
							if (cell.openWalls[3]) {
								//North, East, South, West wall is open
								Instantiate(crossPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.identity);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
							}
							else {
								//North, East, South wall is open
								//West wall is closed
								fix = (GameObject)Instantiate(teePrefab, new Vector3(size *i, 1.98525f, (float)(size * j)), Quaternion.AngleAxis(90f, Vector3.up));
								fix.transform.position += (fix.transform.forward * 0.0945152f) + (-fix.transform.right * 0.010561f);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < teeTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) - 1f, 2.65f, (float)(size * j) -.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) -.85f), Quaternion.identity);
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) - 1f, 2.65f, (float)(size * j) +.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) +.85f), Quaternion.identity);
									}
								}
							}
						}
						else { //South wall is closed
							if (cell.openWalls[3]) {
								//North, East, West wall is open
								//South wall is closed
								fix = (GameObject)Instantiate(teePrefab, new Vector3(size *i, 1.98525f, (float)(size * j)), Quaternion.identity);
								fix.transform.position += (fix.transform.forward * 0.0945152f) + (-fix.transform.right * 0.010561f);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < teeTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.85f, 2.65f, (float)(size * j) - 1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .85f, 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.85f, 2.65f, (float)(size * j) - 1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .85f, 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
								}
							}
							else {
								//North, East wall is open
								//South, West wall is closed
								Instantiate(turnPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(90f, Vector3.up));
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < turnTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -1f, 2.65f, (float)(size * j) - .6f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) -.6f), Quaternion.identity);
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.6f, 2.65f, (float)(size * j) -1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .6f, 2f, (float)(size * j) -1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
								}
							}
						}
					}
					else { //East wall is closed
						if (cell.openWalls[2]) {
							if (cell.openWalls[3]) {
								//North, South, West wall is open
								//East wall is closed
								fix = (GameObject)Instantiate(teePrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(270f, Vector3.up));
								fix.transform.position += (fix.transform.forward * 0.0945152f) + (-fix.transform.right * 0.010561f);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < teeTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) -.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) -.85f), Quaternion.AngleAxis(180f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) +.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) +.85f), Quaternion.AngleAxis(180f, Vector3.up));
									}
								}
							}
							else {
								//North, South wall is open
								//East, West wall is closed
								Instantiate(hallPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(90f, Vector3.up));
								if (Random.Range(0f, 1f) < hallTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .25) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -1f, 2.65f, (float)(size * j) -.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) -.85f), Quaternion.identity);
									}
									else if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -1f, 2.65f, (float)(size * j) +.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) +.85f), Quaternion.identity);
									}
									else if (torchChance <= .75) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) +.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) +.85f), Quaternion.AngleAxis(180f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) -.85f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) -.85f), Quaternion.AngleAxis(180f, Vector3.up));
									}
								}
							}
						}
						else { //South wall is closed
							if (cell.openWalls[3]) {
								//North, West wall is open
								//East, South wall is closed
								Instantiate(turnPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.identity);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < turnTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) - .6f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) - .6f), Quaternion.AngleAxis(180f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.6f, 2.65f, (float)(size * j) -1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .6f, 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
								}
							}
							else {
								//North wall is open
								//East, South, West wall is closed
								Instantiate(endPrefab, new Vector3((float)(size *i), 2f, (float)(size * j)), Quaternion.AngleAxis(270f, Vector3.up));
								if (Random.Range(0f, 1f) < endTorchChance) {
									fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i), 2.65f, (float)(size * j) -1f), Quaternion.identity);
									fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
									Instantiate (torchPrefab, new Vector3((float)(size *i), 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
								}
								if (Random.Range(0f, 1f) < statueChance) 
									if (!(maze.startingCell.pos.x == i && maze.startingCell.pos.y == j))
										Instantiate (statuePrefab, new Vector3((float)(size *i), .54f, (float)(size * j) - .6f), Quaternion.identity);
							}
						}
					}
				}
				else { //North wall is closed
					if (cell.openWalls[1]) {
						if (cell.openWalls[2]) {
							if (cell.openWalls[3]) {
								//East, South, West wall is open
								//North wall is closed
								fix = (GameObject)Instantiate(teePrefab, new Vector3(size *i, 1.98525f, (float)(size * j)), Quaternion.AngleAxis(180f, Vector3.up));
								fix.transform.position += (fix.transform.forward * 0.0945152f) + (-fix.transform.right * 0.010561f);
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < teeTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.85f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .85f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.85f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .85f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
								}
							}
							else {
								//East, South wall is open
								//North, West wall is closed
								Instantiate(turnPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(180f, Vector3.up));
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < turnTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -1f, 2.65f, (float)(size * j) +.6f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j) + .6f), Quaternion.identity);
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.6f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .6f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
								}
							}
						}
						else { //South wall is closed
							if (cell.openWalls[3]) {
								//East, West wall is open
								//North, South wall is closed
								Instantiate(hallPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.identity);
								if (Random.Range(0f, 1f) < hallTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .25) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.85f, 2.65f, (float)(size * j) -1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .85f, 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
									else if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.85f, 2.65f, (float)(size * j) -1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .85f, 2f, (float)(size * j) - 1f), Quaternion.AngleAxis(270f, Vector3.up));
									}
									else if (torchChance <= .75) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -.85f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) - .85f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.85f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .85f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
								}
							}
							else {
								//East wall is open
								//North, South, West wall is closed
								Instantiate(endPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.identity);
								if (Random.Range(0f, 1f) < endTorchChance) {
									fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) -1f, 2.65f, (float)(size * j)), Quaternion.identity);
									fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
									Instantiate (torchPrefab, new Vector3((float)(size *i) - 1f, 2f, (float)(size * j)), Quaternion.identity);
								}
								if (Random.Range(0f, 1f) < statueChance) 
									if (!(maze.startingCell.pos.x == i && maze.startingCell.pos.y == j))
										Instantiate (statuePrefab, new Vector3((float)(size *i) - .6f, .54f, (float)(size * j)), Quaternion.AngleAxis(90f, Vector3.up));
							}
						}
					}
					else { //East wall is closed
						if (cell.openWalls[2]) {
							if (cell.openWalls[3]) {
								//South, West wall is open
								//North, East wall is closed
								Instantiate(turnPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(270f, Vector3.up));
								pathfinder.addWaypoint(new Vector2(size * i, size * j));
								if (Random.Range(0f, 1f) < turnTorchChance) {
									float torchChance = Random.Range (0f, 1f);
									if (torchChance <= .5) {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j) +.6f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j) + .6f), Quaternion.AngleAxis(180f, Vector3.up));
									}
									else {
										fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +.6f, 2.65f, (float)(size * j) +1f), Quaternion.identity);
										fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
										Instantiate (torchPrefab, new Vector3((float)(size *i) + .6f, 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
									}
								}
							}
							else {
								//South wall is open
								//North, East, West wall is closed
								Instantiate(endPrefab, new Vector3((float)(size *i), 2f, (float)(size * j)), Quaternion.AngleAxis(90f, Vector3.up));
								if (Random.Range(0f, 1f) < endTorchChance) {
									fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i), 2.65f, (float)(size * j) +1f), Quaternion.identity);
									fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
									Instantiate (torchPrefab, new Vector3((float)(size *i), 2f, (float)(size * j) + 1f), Quaternion.AngleAxis(90f, Vector3.up));
								}
								if (Random.Range(0f, 1f) < statueChance) 
									if (!(maze.startingCell.pos.x == i && maze.startingCell.pos.y == j))
										Instantiate (statuePrefab, new Vector3((float)(size *i), .54f, (float)(size * j) + .6f), Quaternion.AngleAxis(180f, Vector3.up));
							}
						}
						else { //South wall is closed
							if (cell.openWalls[3]) {
								//West wall is open
								//North, East, South wall is closed
								Instantiate(endPrefab, new Vector3(size *i, 2f, (float)(size * j)), Quaternion.AngleAxis(180f, Vector3.up));
								if (Random.Range(0f, 1f) < endTorchChance) {
									fix = (GameObject)Instantiate (firePrefab, new Vector3((float)(size *i) +1f, 2.65f, (float)(size * j)), Quaternion.identity);
									fix.transform.rotation = Quaternion.AngleAxis(270f, transform.right);
									Instantiate (torchPrefab, new Vector3((float)(size *i) + 1f, 2f, (float)(size * j)), Quaternion.AngleAxis(180f, Vector3.up));
								}
								if (Random.Range(0f, 1f) < statueChance) 
									if (!(maze.startingCell.pos.x == i && maze.startingCell.pos.y == j))
										Instantiate (statuePrefab, new Vector3((float)(size *i) + .6f, .54f, (float)(size * j)), Quaternion.AngleAxis(270f, Vector3.up));
							}
							else {
								//North, East, South, West wall is closed
								//This case should never happen!
								Debug.Log("Error! Impossible case of all walls not existing has occurred!"); 
							}
						}
					}
				}
			}//for j
		}//for i

	}

	/// <summary>
	/// If client, load player
	/// </summary>
	void OnConnectedToServer() {
		int openWall = 0;
		if (_maze.startingCell.openWalls [1])
			openWall = 1;
		if (_maze.startingCell.openWalls [2])
			openWall = 2;
		if (_maze.startingCell.openWalls [3])
			openWall = 3;

		Network.Instantiate (playerPrefabNetwork, new Vector3 (size * _maze.startingCell.pos.x, 2f, size * _maze.startingCell.pos.y), Quaternion.AngleAxis (openWall * 90f, Vector3.up), (int)NetworkGroups.Player);
	}

}