using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public int xWidth = 8;
	private string xText;
	public int yWidth = 8;
	private string yText;
	public int seed = 0;
	private string seedText;
	public int wallRemover = 0;
	private string wallText;
	private bool hiddenGame = false;
	private int temp = 0;
	public bool gameStart = false;
	private HostData hostDataSelected = null;
	private bool isClient = false;
	public bool inGameMenu = false;
	private bool quitting = false;

	/// <summary>
	/// The state of the menu
	/// 0: initial, show options to create server or join game
	/// 1: create server/new game, show normal info
	/// 2: join server, get host list and display
	/// </summary>
	private int state = 0;

	private string gameName = "Instance Name";

	public NetworkManager networkManager;

	void OnGUI () {

		if (!gameStart) {

			if(state == 0) {

				GUI.Label (new Rect ((Screen.width / 2) - 115, (Screen.height / 2) - 155, 75, 25), "Game name:");
				gameName = GUI.TextField(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 155, 150, 25), gameName);

				GUI.Label (new Rect((Screen.width / 2) - 115, (Screen.height / 2) - 125, 75, 25), "Local game:");
				hiddenGame = GUI.Toggle(new Rect((Screen.width / 2) - 25, Screen.height / 2 - 125, 15, 15), hiddenGame, "");

				if(GUI.Button (new Rect(Screen.width / 2 - 75, Screen.height / 2 - 90, 150, 50), "Create a server\nand play.")) {
					state = 1;
				}

				if(GUI.Button (new Rect(Screen.width / 2 - 75, Screen.height / 2 - 25, 150, 50), "Join a game.")) {
					networkManager.RefreshHostList();
					state = 2;
				}

				if(GUI.Button (new Rect(Screen.width / 2 - 75, Screen.height / 2 + 35, 150, 50), "Quit")) {
					Application.Quit();
				}

			} else if(state == 1) {

				GUI.Label (new Rect ((Screen.width / 2) - 80, (Screen.height / 2) - 160, 180, 20), "Input maze dimensions below");
				
				xText = GUI.TextField (new Rect ((Screen.width / 2) - 40, (Screen.height / 2) - 140, 30, 20), xWidth.ToString (), 3);
				if (int.TryParse (xText, out temp)) {
					xWidth = Mathf.Clamp (temp, 0, 20);
				} else if (xText == "")
					xWidth = 0;
				
				yText = GUI.TextField (new Rect ((Screen.width / 2) + 10, (Screen.height / 2) - 140, 30, 20), yWidth.ToString (), 3);
				if (int.TryParse (yText, out temp)) {
					yWidth = Mathf.Clamp (temp, 0, 20);
				} else if (yText == "")
					yWidth = 0;
				
				GUI.Label (new Rect ((Screen.width / 2) - 125, (Screen.height / 2) - 120, 275, 40), "Wall Remover! Higher numbers remove more walls. Enter 0 to generate a perfect maze");
				
				wallText = GUI.TextField (new Rect ((Screen.width / 2) - 25, (Screen.height / 2) - 80, 50, 20), wallRemover.ToString (), 5);
				if (int.TryParse (wallText, out temp)) {
					wallRemover = Mathf.Clamp (temp, 0, 9999);
				} else if (wallText == "")
					wallRemover = 0;
				
				GUI.Label (new Rect ((Screen.width / 2) - 60, (Screen.height / 2) - 60, 140, 20), "Maze seed (optional)");
				
				seedText = GUI.TextField (new Rect ((Screen.width / 2) - 25, (Screen.height / 2) - 40, 50, 20), seed.ToString (), 5);
				if (int.TryParse (seedText, out temp)) {
					seed = Mathf.Clamp (temp, 0, 65535);
				} else if (seedText == "")
					seed = 0;
				
				if (GUI.Button (new Rect ((Screen.width / 2) - 75, (Screen.height / 2), 150, 50), "Start Game")) {
					if (xWidth < 4)
						xWidth = 4;
					if (yWidth < 4)
						yWidth = 4;

					if(seed == 0)
						seed = Random.Range (1000, 10000);

					seedText = seed.ToString();

					string comment = "";
					comment += seedText + " " + xText + " " + yText + " " + wallText;
					networkManager.StartServer(gameName, hiddenGame, comment);

					OnLevelLoadMethod load = delegate {
						MazeGeneration m = GameObject.Find ("MazeGenerator").GetComponent<MazeGeneration>();
						m.GenerateMaze(seed, xWidth, yWidth, wallRemover, true);
					};
					
					GameObject.Find ("LevelChanger").GetComponent<LevelChanger>().LoadMaze(load);
					
				}

				if(GUI.Button (new Rect ((Screen.width / 2) - 75, (Screen.height / 2) + 75, 150, 50), "Back")) {
					state = 0;
				}

			} else if(state == 2 ) {

				if(GUI.Button (new Rect(Screen.width - 400, Screen.height - 75, 100, 50), "Find Games")) {
					networkManager.RefreshHostList();
				}

				if(networkManager.hostList != null) {

					for(int i = 0; i < networkManager.hostList.Length; ++i) {
						if(networkManager.hostList[i].connectedPlayers < networkManager.hostList[i].playerLimit) {
							if(GUI.Button (new Rect(Screen.width / 2 - 75, 100 + (i * 65), 150, 50),
							               networkManager.hostList[i].gameName + ": " + networkManager.hostList[i].connectedPlayers + "/" + networkManager.hostList[i].playerLimit)) {
								hostDataSelected = networkManager.hostList[i];
								isClient = true;

								int _seed, _x, _y, _wall;
								string[] vals = hostDataSelected.comment.Split(' ');
								int.TryParse(vals[0], out _seed);
								int.TryParse(vals[1], out _x);
								int.TryParse(vals[2], out _y);
								int.TryParse(vals[3], out _wall);
								OnLevelLoadMethod load = delegate {
									MazeGeneration m = GameObject.Find ("MazeGenerator").GetComponent<MazeGeneration>();
									m.GenerateMaze(_seed, _x, _y, _wall, false);
									networkManager.JoinServer(hostDataSelected);
								};
								GameObject.Find ("LevelChanger").GetComponent<LevelChanger>().LoadMaze(load);
							}
						} else {
							GUI.enabled = false;
							if(GUI.Button (new Rect(Screen.width / 2 - 75, 100 + (i * 65), 150, 50),
							               networkManager.hostList[i].gameName + ": " + networkManager.hostList[i].connectedPlayers + "/" + networkManager.hostList[i].playerLimit)) {
								hostDataSelected = networkManager.hostList[i];
								isClient = true;
								
								int _seed, _x, _y, _wall;
								string[] vals = hostDataSelected.comment.Split(' ');
								int.TryParse(vals[0], out _seed);
								int.TryParse(vals[1], out _x);
								int.TryParse(vals[2], out _y);
								int.TryParse(vals[3], out _wall);
								OnLevelLoadMethod load = delegate {
									MazeGeneration m = GameObject.Find ("MazeGenerator").GetComponent<MazeGeneration>();
									m.GenerateMaze(_seed, _x, _y, _wall, false);
									networkManager.JoinServer(hostDataSelected);
								};
								GameObject.Find ("LevelChanger").GetComponent<LevelChanger>().LoadMaze(load);
							}
							GUI.enabled = true;
						}

					}

				}

				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 75, 100, 50), "Back")) {
					state = 0;
				}

			}

		} else if(gameStart && inGameMenu) {

			if(GUI.Button (new Rect(Screen.width / 2 - 75, Screen.height / 2 - 75, 150, 50), "Main Menu")) {
				if(isClient) {
					Network.Disconnect();
				} else {
					networkManager.StopServer();
				}
			}

			if(GUI.Button (new Rect(Screen.width / 2 - 75, Screen.height / 2, 150, 50), "Quit")) {
				if(isClient)
					Network.Disconnect();
				else
					networkManager.StopServer();

				quitting = true;
			}
		}

	}

	void OnDisconnectedFromServer(NetworkDisconnection disc) {
		// We've disconnected, so we can't use Network.Destroy anymore
		GameObject.Destroy (GameObject.Find ("HeroNetwork(Clone)"));
		GameObject.Destroy (GameObject.Find ("Hero(Clone)"));

		GameObject lc = GameObject.Find ("LevelChanger");
		GameObject nm = GameObject.Find ("NetworkManager");
		GameObject.Find ("LevelChanger").GetComponent<LevelChanger> ().LoadMenu (delegate {
				GameObject.Destroy (this.gameObject);  // The menu already has a Menu object so this one can go
				GameObject.Destroy (lc);
				GameObject.Destroy (nm);
				if(quitting)
					Application.Quit ();
		});

	}

	/// <summary>
	/// Capture the ESC key for the menu
	/// </summary>
	void Update() {

		if (gameStart && Input.GetKeyDown (KeyCode.Escape)) {
			inGameMenu = !inGameMenu;
		}

		if (!gameStart || inGameMenu)
			Screen.lockCursor = false;
		else
			Screen.lockCursor = true;

	}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

}