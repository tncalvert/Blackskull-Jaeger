/* Module      : MazeGeneration
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : This class is responsible for generating a maze.
 *
 * Date        : 2014/03/28
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class responsible for building the structure of the maze
/// </summary>
public class MazeGeneration : MonoBehaviour {

	/// <summary>
	/// Script responsible for placing the maze itself, once we are done generating it
	/// </summary>
	public PlaceMaze placeMaze;

	/// <summary>
	/// Script responsible for determining the size of the maze.
	/// </summary>
	public MenuScript menu;
	
	void Start() {
		GameObject menuObject = GameObject.Find ("Menu");
		if (menuObject == null) {
			GenerateMaze (15, 15, 0, true);
		}
	}

	/// <summary>
	/// The seed value for the maze. Starts as -1 so we can identify if it
	/// hasn't been set and generate one
	/// </summary>
	private int mazeSeed = -1;

	/// <summary>
	/// A stack holding visited cells that is used to manage the walk through
	/// cells when generating the maze
	/// </summary>
	private Stack<Cell> cellStack;

	/// <summary>
	/// The number of cells total in the maze. Width * Height
	/// </summary>
	private int numberOfCells;

	/// <summary>
	/// The number of vells we have visited so far. When it reaches
	/// <see cref="numberOfCells"/> the algorithm will end. 
	/// </summary>
	private int visitedCells;

	/// <summary>
	/// An int representing the upper bound of how many extra walls will
	/// be removed from the perfect maze (will produce loops). 
	/// A value of 0 will produce a perfect maze.
	/// </summary>
	public int wallRemover = 20;

	/// <summary>
	/// The width of the maze
	/// </summary>
	private int x = 1;

	/// <summary>
	/// The height of the maze
	/// </summary>
	private int y = 1;

	/// <summary>
	/// The object representing the maze itself
	/// </summary>
	public Maze maze;

	/// <summary>
	/// Gets or sets the width of the maze.
	/// </summary>
	/// <value>The width.</value>
	public int X {
		get {
			return x;
		}
		
		set {
			x = value;
		}
	}

	/// <summary>
	/// Gets or sets the height of the maze.
	/// </summary>
	/// <value>The height.</value>
	public int Y {
		get {
			return y;
		}

		set {
			y = value;
		}
	}

	private bool placeNetworkedObjects = true;

	/// <summary>
	/// Generates the maze. Will create a random seed value and use the current
	/// values for width and height (by default 1 for both, but these can be set individually).
	/// </summary>
	public void GenerateMaze() {
		if(mazeSeed == -1)
			mazeSeed = Random.Range (1000, 10000);
		
		Random.seed = mazeSeed;
		cellStack = new Stack<Cell> ();
		numberOfCells = x * y;
		visitedCells = 1;
		
		_GenerateMaze ();
		
		placeMaze.placeMaze (maze, placeNetworkedObjects);
	}

	/// <summary>
	/// Generates the maze. Will use the seed passed in and use the current
	/// values for width and height (by default 1 for both, but these can be set individually).
	/// </summary>
	/// <param name="seed">Seed.</param>
	public void GenerateMaze(int seed) {
		mazeSeed = seed;
		GenerateMaze ();
	}

	/// <summary>
	/// Generates the maze. Will use the width and height specified and generate
	/// a random seed.
	/// </summary>
	/// <param name="x">The width.</param>
	/// <param name="y">The height.</param>
	public void GenerateMaze(int x, int y, int wallRemover, bool placeNetworkedObjects) {
		this.x = x;
		this.y = y;
		this.wallRemover = wallRemover;
		this.placeNetworkedObjects = placeNetworkedObjects;
		GenerateMaze ();
	}

	/// <summary>
	/// Generates the maze. Uses the seed passed in and the width and height
	/// provided.
	/// </summary>
	/// <param name="seed">The seed value.</param>
	/// <param name="x">The width.</param>
	/// <param name="y">The height.</param>
	public void GenerateMaze(int seed, int x, int y, int wallRemover, bool placeNetworkedObjects) {
		mazeSeed = seed;
		this.x = x;
		this.y = y;
		this.wallRemover = wallRemover;
		this.placeNetworkedObjects = placeNetworkedObjects;
		GenerateMaze ();
	}

	/// <summary>
	/// Responsible for actually generating the maze.
	/// </summary>
	/// <returns>The generate maze.</returns>
	private Maze _GenerateMaze() {

		maze = new Maze (x, y);
		Cell currentCell = maze.cells[0, Random.Range (0, y)];
		Cell finalCell = maze.cells [x-1, Random.Range (0, y-1)];
		maze.startingCell = currentCell;
		maze.endingCell = finalCell;

		//While loop that generates the perfect maze
		while (visitedCells < numberOfCells) {

			Position[] neighborIndex = getNeighborIndecies(currentCell);

			if(neighborIndex.Length > 0) {

				int nextCell = Random.Range (0, neighborIndex.Length);

				int dir = neighborIndex[nextCell].getDirection(currentCell.pos);
				currentCell.openWalls[dir] = true;
				dir = (dir + 2) % 4;

				cellStack.Push (currentCell);
				currentCell = maze.cells[neighborIndex[nextCell].x, neighborIndex[nextCell].y];

				currentCell.openWalls[dir] = true;

				++visitedCells;
			} else {
				currentCell = cellStack.Pop ();
			}

		}

		//For loop that removes walls from the perfect maze, up to value wallRemover
		//Could possibly always remove walls that are already removed. :D
		int randX, randY, randDir;
		for (int removed = 0; removed < wallRemover; removed++) {

			randX = Random.Range (1, x-1);
			randY = Random.Range (1, y-1);
			randDir = Random.Range (0, 3);
			currentCell = maze.cells[randX, randY];

			currentCell.openWalls[randDir] = true;

			switch (randDir) {
			case 0:
				randY += 1;
				break;
			case 1: 
				randX += 1;
				break;
			case 2:
				randY -= 1;
				break;
			default:
				randX -= 1;
				break;
			}

			randDir = (randDir + 2) % 4;
			currentCell = maze.cells[randX, randY];
			
			currentCell.openWalls[randDir] = true;

		}

		//Remove the walls at the end of the maze that leads to the boss room.
		finalCell.openWalls [1] = true;
		currentCell = maze.cells[finalCell.pos.x, finalCell.pos.y+1];
		currentCell.openWalls [1] = true;

		// Place coins
		int min = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.25f), 100);
		int max = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.6f), 101);
		int numCoins = Random.Range (min, max);

		min = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.05f), 15);
		max = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.15f), 16);
		int numGoodMushrooms = Random.Range (min, max);

		min = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.05f), 15);
		max = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.15f), 16);
		int numBadMushrooms = Random.Range (min, max);

		min = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.08f), 10);
		max = System.Math.Min (Mathf.CeilToInt(numberOfCells * 0.16f), 30);
		int numGhoulies = Random.Range (min, max);

		// Limit to nearest number divisible by 5
		int m = numCoins / 5;
		numCoins = (m == 0 ? 1 : m) * 5;
		m = numGoodMushrooms / 5;
		numGoodMushrooms = (m == 0 ? 1 : m) * 5;
		m = numBadMushrooms / 5;
		numBadMushrooms = (m == 0 ? 1 : m) * 5;
		m = numGhoulies / 5;
		numGhoulies = (m == 0 ? 1 : m) * 5;

		int placedCoins = 0;
		int placedGoodMushrooms = 0;
		int placedBadMushrooms = 0;
		int placedGhoulies = 0;

		while (placedCoins < numCoins) {
			int i, j;
			i = Random.Range (0, x);
			j = Random.Range (0, y);
			if(!maze.cells[i, j].hasCoin) {
				maze.cells[i, j].hasCoin = true;
				++placedCoins;
			}
		}

		while (placedGoodMushrooms < numGoodMushrooms) {
			int i, j;
			i = Random.Range (0, x);
			j = Random.Range (0, y);
			if(maze.cells[i, j].hasMushroom == 0) {
				maze.cells[i, j].hasMushroom = 1;
				++placedGoodMushrooms;
			}
		}

		while (placedBadMushrooms < numBadMushrooms) {
			int i, j;
			i = Random.Range (0, x);
			j = Random.Range (0, y);
			if(maze.cells[i, j].hasMushroom == 0) {
				maze.cells[i, j].hasMushroom = 2;
				++placedBadMushrooms;
			}
		}

		while (placedGhoulies < numGhoulies) {
			int i, j;
			i = Random.Range (0, x);
			j = Random.Range (0, y);
			if(!maze.cells[i, j].hasGhoulie) {
				maze.cells[i, j].hasGhoulie = true;
				++placedGhoulies;
			}
		}

		maze.numCoins = numCoins;
		maze.numMushrooms = numGoodMushrooms + numBadMushrooms;

		return maze;
	}

	/// <summary>
	/// Gets the positions of neighboring cells that are valid.
	/// Cells are valid if the are on the grid (values aren't outside the grid)
	/// and all of theirs walls are still up.
	/// </summary>
	/// <returns>An array of <see cref="Position"/> will all valid positions</returns>
	/// <param name="cell">The cell to use as the center</param>
	private Position[] getNeighborIndecies(Cell cell) {
		Position[] neighborIndex = {new Position(), new Position(), new Position(), new Position()};
		neighborIndex[0].x = cell.pos.x;
		neighborIndex[0].y = cell.pos.y - 1;
		neighborIndex[1].x = cell.pos.x + 1;
		neighborIndex[1].y = cell.pos.y;
		neighborIndex[2].x = cell.pos.x;
		neighborIndex[2].y = cell.pos.y + 1;
		neighborIndex[3].x = cell.pos.x - 1;
		neighborIndex[3].y = cell.pos.y;

		neighborIndex = neighborIndex.Where(n => n.x >= 0 && n.x < x && n.y >= 0 && n.y < y).ToArray();
		neighborIndex = neighborIndex.Where(n => maze.cells[n.x, n.y].AllWallsIntact).ToArray();

		return neighborIndex;
	}

}

/// <summary>
/// Struct to hold information about a cell in the maze
/// </summary>
public class Cell {

	public Cell() {
		pos = new Position ();
		traversed = false;
	}

	/// <summary>
	/// Shows whether a wall is open (true) or closed (false)
	/// 0 - North
	/// 1 - East
	/// 2 - South
	/// 3 - West
	/// </summary>
	public bool[] openWalls = { false, false, false, false};

	/// <summary>
	/// Determines if all walls are intact.
	/// </summary>
	/// <value><c>true</c> if all walls intact; otherwise, <c>false</c>.</value>
	public bool AllWallsIntact { get { return !(openWalls[0] || openWalls[1] || openWalls[2] || openWalls[3]); }}

	/// <summary>
	/// The position of the cell in the grid.
	/// </summary>
	public Position pos;

	/// <summary>
	/// Bool flag to indicate if there will be a coin on this cell
	/// </summary>
	public bool hasCoin;

	/// <summary>
	/// Indicates that there is a mushroom on the cell.
	/// 0: No mushroom
	/// 1: Good mushroom
	/// 2: Bad mushroom
	/// </summary>
	public uint hasMushroom;

	/// <summary>
	/// Bool flag to indicate if there will be a ghoulie on this cell
	/// </summary>
	public bool hasGhoulie;

	/// <summary>
	/// Tells whether the cell has been traversed or not.
	/// </summary>
	public bool traversed;

}

/// <summary>
/// X,Y coordinate pair
/// </summary>
public class Position {
	public int x;
	public int y;

	/// <summary>
	/// Returns the direction from otherPos to this position.
	/// 0: this position is North of otherPos
	/// 1: this position is East of otherPos
	/// 2: this position is South of otherPos
	/// 3: this position is West of otherPos
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="otherPos">The other position to use as center</param>
	public int getDirection(Position otherPos) {
		int xDiff = otherPos.x - this.x;
		int yDiff = otherPos.y - this.y;

		if (xDiff == -1) {
			return 1;
		} else if (xDiff == 1) {
			return 3;
		} else if (yDiff == -1) {
			return 0;
		} else if (yDiff == 1) {
			return 2;
		} else {
			return -1;
		}
	}

	public override string ToString ()
	{
		return "(" + x + ", " + y + ")";
	}
}

/// <summary>
/// Holds information about the maze
/// </summary>
public class Maze {

	/// <summary>
	/// Initializes a new instance of the <see cref="Maze"/> class.
	/// </summary>
	/// <param name="x">The number of horizontal cells</param>
	/// <param name="y">The number of vertical cells</param>
	public Maze(int x, int y) {
		width = x;
		height = y;
		cells = new Cell[x, y];
		for (int i = 0; i < x; ++i) {
			for(int j = 0; j < y; ++j) {
				cells[i, j] = new Cell();
				cells[i, j].pos.x = i;
				cells[i, j].pos.y = j;
			}
		}
	}

	/// <summary>
	/// Array of cells making up the maze
	/// </summary>
	public Cell[,] cells;

	/// <summary>
	/// The starting cell.
	/// </summary>
	public Cell startingCell;

	/// <summary>
	/// The ending cell.
	/// </summary>
	public Cell endingCell;

	/// <summary>
	/// The width.
	/// </summary>
	public int width;

	/// <summary>
	/// The height.
	/// </summary>
	public int height;

	/// <summary>
	/// The number of coins.
	/// </summary>
	public int numCoins;

	/// <summary>
	/// The number of mushrooms.
	/// </summary>
	public int numMushrooms;
}
