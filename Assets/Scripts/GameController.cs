using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;
using System.Collections.Generic;           // for using list

public class GameController : MonoBehaviour {

    public Text objectiveText;
    public Canvas pauseScreen;
    public Canvas gameOverScreen;
    public Canvas succesScreen;
    public Canvas endScreen;
    public TileType[] tileTypes;
    public GameObject[] playerPrefabs;
    public GameObject pickUp;   // use this instead
    public GameObject blueItem;
    public GameObject rock;
    public List<GameObject> players = new List<GameObject>(3);
    public bool playerMovement;

    struct emptySpace {
        public emptySpace(int x, int z) { xLocation = x; zLocation = z; }
        int xLocation;
        int zLocation;
        public int getXLocation() { return xLocation; }
        public int getZLocation() { return zLocation; }
        public bool equalTo(emptySpace other) {
            if (this.xLocation == other.getXLocation() && this.zLocation == other.getZLocation())
                return true;
            return false;
        }
    }
    // varibles changing by wave
    private int maxRockNum;                         // 16 is perfect for the first wave 
    private int itemGoal;
    private int groupingMax;
    private float timeLimit;
    private float timeBetweenLasers;                // duration between the lasers
    private float specialItemTime;
    private bool specialItemOnce;
    private float warningTime;
    private float lazertimer = 0.0f;
    private float gameTimer;
    private int numOfLasers;
    private int mapSizeX = 13;
    private int mapSizeZ = 13;
    private int[] scores = new int[4];
    private bool successful;
    private GameObject[] wall;                                  // array of walls in the game
    private List<GameObject> rocks = new List<GameObject>();    // array of rocks in the game
    private List<emptySpace> emptyBlocks = new List<emptySpace>();
    private pathRequestManager manager;
    private GameObject item;
    private GameObject specialItem;
    private bool isSepcialAvailable;
    private bool gameStart = false;
    private bool levelSetup;
    private UIController uiController;
    private Grid gridScript;
    private memoryScript lvlMemory;
    public enum GameState { Transition, Playing, Success, Gameover, EndLevel }
    private GameState currentState;

    int[,] wallTiles;       // 0:NorthWall 1: SouthWall 2:EastWall 3:WestWall 
                            // 4: coner Rock == rock 5: Nothing 6: Rocks 7:item 8:player 9: (used for rockchecking)

    // Use this for initialization
    void Start() {
        pauseScreen = pauseScreen.GetComponent<Canvas>();
        gameOverScreen = gameOverScreen.GetComponent<Canvas>();
        succesScreen = succesScreen.GetComponent<Canvas>();
        endScreen = endScreen.GetComponent<Canvas>();
        pauseScreen.enabled = true;
        gameOverScreen.enabled = false;
        succesScreen.enabled = false;
        endScreen.enabled = false;
        playerMovement = false;
        levelSetup = true;
        isSepcialAvailable = false;
        objectiveText.text = "";

        lvlMemory = GameObject.FindGameObjectWithTag("Memory").GetComponent<memoryScript>();
        manager = GetComponent<pathRequestManager>();
        gridScript = GetComponent<Grid>();
        uiController = GetComponent<UIController>();
        generateAllTiles();
        generateWallTileVisual();
        generatePlayer();                           // creates player (always do this after generating rocks)
        setCurrentState(GameState.Transition);
        uiController.displayEpisode();
    }

    void Update() {
        switch (currentState) {
            case GameState.Transition:
                if (levelSetup) {
                    setupLevel();
                    levelSetup = false;
                    generatePlayer();
                    itemSpawn();
                    uiController.displayEpisode();
                }
                playerMovement = false;
                if (Input.GetKeyDown(KeyCode.Space)) {
                    manager.clearQueue();
                    players[0].GetComponent<PlayersBase>().speed = 6.0f;
                    setCurrentState(GameState.Playing); // change this to countdown state when I have time
                    pauseScreen.enabled = false;
                    setGameStart(true);
                    AIPathRequests();
                    successful = false;
                }
                break;
            case GameState.Playing:
                if (!playerMovement) {
                    playerMovement = true;                                            // allowing players to move
                }
                gameTimer -= Time.deltaTime;                                          // count down the game time
                lazertimer += Time.deltaTime;
                if (gameTimer <= 0f)                                                  // round over
                {                                                                    // reseting for next round
                    lazertimer = 0;
                    gameTimer = getTimeLimit();
                    gameOverScreen.enabled = true;
                    AIPathStop();
                    setCurrentState(GameState.Gameover);
                }
                else if(gameTimer < specialItemTime && specialItemOnce) {
                    itemSpawn(true);
                    specialItemOnce = false;
                }
                else if (successful) {
                    lazertimer = 0;
                    gameTimer = getTimeLimit();
                    succesScreen.enabled = true;
                    playerMovement = false;
                    AIPathStop();
                    lvlMemory.setLevelProgression();
                    if (lvlMemory.getCurrentLvl() != 6)
                        setCurrentState(GameState.Success);
                    else
                        setCurrentState(GameState.EndLevel);
                }
                uiController.displayTime(gameTimer);                                                      // displays time
                if (lazertimer > timeBetweenLasers && gameStart)                                   // 4 seconds between the lasers
                {
                    lazertimer = 0;
                    generateLasers(numOfLasers);
                }
                break;
            case GameState.Success:
                playerMovement = false;
                if (Input.GetKeyDown(KeyCode.Space)) {
                    destoryAllRocks();
                    destroyAllPlayers();
                    succesScreen.enabled = false;
                    pauseScreen.enabled = true;
                    levelSetup = true;
                    lvlMemory.setCurrentLvl(lvlMemory.getCurrentLvl() + 1);
                    setCurrentState(GameState.Transition);
                }
                break;
            case GameState.Gameover:
                gameOverScreen.enabled = true;
                playerMovement = false;
                if (Input.GetKeyDown(KeyCode.Space)) {
                    destoryAllRocks();
                    resetAllScores();
                    destroyAllPlayers();
                    gameOverScreen.enabled = false;
                    pauseScreen.enabled = true;
                    levelSetup = true;
                    setCurrentState(GameState.Transition);
                }
                break;
            case GameState.EndLevel:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    endScreen.enabled = false;
                    lvlMemory.setCurrentLvl(1);
                    switch (lvlMemory.getCurrentEpisode()) {
                        case "episode1":
                            lvlMemory.setCurrentEpisode("episode2");
                            SceneManager.LoadScene("episode2");
                            break;
                        case "episode2":
                            lvlMemory.setCurrentEpisode("episode3");
                            SceneManager.LoadScene("episode3");
                            break;
                        case "episode3":
                            SceneManager.LoadScene("levelSelection");
                            break;
                        default:
                            SceneManager.LoadScene("levelSelection");
                            break;
                    }
                }
                break;
        }
    }

    void generateAllTiles()                                // Generating the tile pieces
    {
        wallTiles = new int[(mapSizeX + 2), (mapSizeZ + 2)];

        //Nothingness
        for (int x = 1; x <= mapSizeX; x++) {
            for (int z = 1; z <= mapSizeZ; z++) {
                emptyBlocks.Add(new emptySpace(x, z));
                wallTiles[x, z] = 5;
            }
        }
        // N & S walls
        for (int x = 1; x <= mapSizeX; x++) {
            //South Wall
            wallTiles[x, 0] = 1;
            //North Wall
            wallTiles[x, mapSizeX + 1] = 0;
        }

        // E & W walls
        for (int z = 1; z <= mapSizeZ; z++) {
            //West Wall
            wallTiles[0, z] = 3;
            //East Wall
            wallTiles[mapSizeX + 1, z] = 2;
        }

        //coners
        wallTiles[0, 0] = 4;
        wallTiles[0, mapSizeZ + 1] = 4;
        wallTiles[mapSizeX + 1, 0] = 4;
        wallTiles[mapSizeX + 1, mapSizeZ + 1] = 4;
    }

    void generateWallTileVisual()                           // Generating all four walls plus the four coners
    {
        wall = new GameObject[(mapSizeX * 4)];
        int i = 0;
        for (int x = 0; x < mapSizeX + 2; x++) {
            for (int z = 0; z < mapSizeZ + 2; z++) {
                if (wallTiles[x, z] >= 0 && wallTiles[x, z] < 5) {
                    TileType tt = tileTypes[wallTiles[x, z]];
                    GameObject currentWall = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    if (wallTiles[x, z] < 4) {
                        wall[i] = currentWall;
                        i++;
                    }
                }
            }
        }
    }

    void generateLasers(int n)                              // generates n-number of Lasers at a time
    {
        while (n > 0)                           // making sure that Lasers don't overlap at a spawning points
        {
            int rand = Mathf.FloorToInt(Random.Range(0, wall.Length - 0.01f));
            if (!wall[rand].GetComponent<WallScript>().getIsCharging()) {
                wall[rand].GetComponent<WallScript>().setIsCharging(true);
                n--;
            }
        }
    }

    void generateRocks()                           // generates rocks depending on different level
    {
        int i = 0;
        while (i < maxRockNum)                                                                    // spawn 10 rocks
        {
            int randomLocation = Mathf.FloorToInt(Random.Range(1, emptyBlocks.Count - 0.0001f));
            emptySpace thisSpace = emptyBlocks[randomLocation];

            if (checkPositionWithPlayers(thisSpace.getXLocation(), thisSpace.getZLocation(), .9f)) // checking to see if it overlaps with players AND if this is a empty spot                                                    // avoiding overlaps
            {
                wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] = 6;    // try putting the rock in this position
                if (checksForLegalRockSpawn()) {        // check to see if it traps any space
                    GameObject currentRock = (GameObject)Instantiate(rock, new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity);
                    rocks.Add(currentRock);
                    emptyBlocks.RemoveAt(randomLocation);
                    i++;
                    int passingNum = maxRockNum - i;
                    if (maxRockNum - i > groupingMax)
                        passingNum = groupingMax;
                    i = i + generateGroupOfRocks(passingNum, thisSpace.getXLocation(), thisSpace.getZLocation());
                }
                else {
                    wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] = 5;        // if it does traps a space don't put the rock there
                }
            }
        }
    }

    // ways to check item overlap
    void setupLevel() {
        bool specialItem = false;
        if (lvlMemory.getCurrentEpisode() == "episode1") {
            if (lvlMemory.getCurrentLvl() == 1) {
                maxRockNum = 40;
                itemGoal = 3;
                setTimeLimit(30.0f); // 30 f
                groupingMax = 5;
                timeBetweenLasers = 4.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 5;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 3 items in 30 seconds\n- Difficulty: Easy\n*Lazers are pretty slow";
                Debug.Log("Loading Level 1");
            }
            else if (lvlMemory.getCurrentLvl() == 2) {
                maxRockNum = 35;
                itemGoal = 4;
                groupingMax = 5;
                setTimeLimit(30.0f);
                timeBetweenLasers = 4.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 5;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 4 items in 30 seconds\n- Difficulty: Easy\n*Lazers are pretty slow";
                Debug.Log("Loading Level 2");
            }
            else if (lvlMemory.getCurrentLvl() == 3) {
                maxRockNum = 35;
                itemGoal = 7;
                groupingMax = 4;
                setTimeLimit(35.0f);
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 6;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 7 items in 35 seconds\n- Difficulty: Easy\n*Normal Lazer Speed\n*Look for the Speed Boost item!";
                Debug.Log("Loading Level 3");
            }
            else if (lvlMemory.getCurrentLvl() == 4) {
                maxRockNum = 30;
                itemGoal = 8;
                setTimeLimit(35.0f);
                groupingMax = 4;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 9 items in 35 seconds\n- Difficulty: Normal\n*Normal Lazer Speed\n*Look for the Speed Boost item!";
                Debug.Log("Loading Level 4");
            }
            else if (lvlMemory.getCurrentLvl() == 5) {
                maxRockNum = 25;
                itemGoal = 9;
                setTimeLimit(35.0f);
                groupingMax = 3;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 7 items in 35 seconds\n- Difficulty: Normal\n*Normal Lazer Speed";
                Debug.Log("Loading Level 5");
            }
            else if (lvlMemory.getCurrentLvl() == 6) {
                maxRockNum = 25;
                itemGoal = 10;
                setTimeLimit(40.0f);
                groupingMax = 3;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 7 items in 35 seconds\n- Difficulty: Normal\n*Normal Lazer Speed";
                Debug.Log("Loading Level 6");
            }
        }
        else if (lvlMemory.getCurrentEpisode() == "episode2") {
            if (lvlMemory.getCurrentLvl() == 1) {
                maxRockNum = 30;
                itemGoal = 7;
                setTimeLimit(35.0f);
                groupingMax = 4;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 7 items in 35 seconds\n- Difficulty: Normal\n*Speed Boost Available\n*Normal Lazer Speed";
                Debug.Log("Loading Level 1");
            }
            else if (lvlMemory.getCurrentLvl() == 2) {
                maxRockNum = 25;
                itemGoal = 7;
                groupingMax = 3;
                setTimeLimit(30.0f);
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 7 items in 30 seconds\n- Difficulty: Hard\n*Speed Boost Available\n*Normal Lazer Speed";
                Debug.Log("Loading Level 2");
            }
            else if (lvlMemory.getCurrentLvl() == 3) {
                maxRockNum = 20;
                itemGoal = 5;
                groupingMax = 3;
                setTimeLimit(20.0f);
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 5 items in 20 seconds\n- Difficulty: Hard\n*Normal Lazer Speed";
                Debug.Log("Loading Level 3");
            }
            else if (lvlMemory.getCurrentLvl() == 4) {
                maxRockNum = 20;
                itemGoal = 9;
                setTimeLimit(35.0f);
                groupingMax = 2;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 3.0f;
                numOfLasers = 9;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 9 items in 35 seconds\n- Difficulty: Hard\n*Normal Lazer Speed";
                Debug.Log("Loading Level 4");
            }
            else if (lvlMemory.getCurrentLvl() == 5) {
                maxRockNum = 25;
                itemGoal = 6;
                setTimeLimit(30.0f);
                groupingMax = 2;
                timeBetweenLasers = 3.0f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 6 items in 30 seconds\n- Difficulty: Very Hard\n*Speed Boost Available\n*Lazers seems to be faster....";
                Debug.Log("Loading Level 5");
            }
            else if (lvlMemory.getCurrentLvl() == 6) {
                maxRockNum = 20;
                itemGoal = 11;
                setTimeLimit(50.0f);
                groupingMax = 3;
                timeBetweenLasers = 2.5f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 11 items in 50 seconds\n- Difficulty: Very Hard\n*Speed Boost Available\n*Lazers are pretty fast... Watch out!";
                Debug.Log("Loading Level 5");
            }
        }
        else if (lvlMemory.getCurrentEpisode() == "episode3") {
            if (lvlMemory.getCurrentLvl() == 1) {
                maxRockNum = 20;
                itemGoal = 8;
                setTimeLimit(40.0f);
                groupingMax = 2;
                timeBetweenLasers = 2.5f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                objectiveText.text =
                "- Collect 8 items in 40 seconds\n- Difficulty: Very Hard\n*Fast Lazer Speed!";
                Debug.Log("Loading Level 1");
            }
            else if (lvlMemory.getCurrentLvl() == 2) {
                maxRockNum = 15;
                itemGoal = 7;
                groupingMax = 2;
                setTimeLimit(30.0f);
                timeBetweenLasers = 2.5f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 7 items in 30 seconds\n- Difficulty: Very Hard\n*Speed Boost available\n*Fast Lazer Speed!";
                Debug.Log("Loading Level 2");
            }
            else if (lvlMemory.getCurrentLvl() == 3) {
                maxRockNum = 15;
                itemGoal = 5;
                groupingMax = 2;
                setTimeLimit(30.0f);
                timeBetweenLasers = 2.0f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 5 items in 30 seconds\n- Difficulty: Very Hard\n*Speed Boost available\n*Lazers are even faster!";
                Debug.Log("Loading Level 3");
            }
            else if (lvlMemory.getCurrentLvl() == 4) {
                maxRockNum = 15;
                itemGoal = 6;
                setTimeLimit(35.0f);
                groupingMax = 1;
                timeBetweenLasers = 2.0f;                   // duration between the lasers
                warningTime = 2.0f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 6 items in 35 seconds\n- Difficulty: Very Hard\n*Speed Boost available\n*Very fast Lazers";
                Debug.Log("Loading Level 4");
            }
            else if (lvlMemory.getCurrentLvl() == 5) {
                maxRockNum = 15;
                itemGoal = 6;
                setTimeLimit(35.0f);
                groupingMax = 1;
                timeBetweenLasers = 2.0f;                   // duration between the lasers
                warningTime = 1.5f;
                numOfLasers = 7;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 6 items in 35 seconds\n- Difficulty: INTENSE\n*Speed Boost available\n*Good luck........";
                Debug.Log("Loading Level 5");
            }
            else if (lvlMemory.getCurrentLvl() == 6) {
                maxRockNum = 10;
                itemGoal = 8;
                setTimeLimit(40.0f);
                groupingMax = 1;
                timeBetweenLasers = 1.5f;                   // duration between the lasers
                warningTime = 1.5f;
                numOfLasers = 8;                            // number of lasers to be fired at a time
                specialItem = true;
                objectiveText.text =
                "- Collect 10 items in 35 seconds\n- Difficulty: INTENSE\n*Speed Boost available\n*So close! but soooooo far away";
                Debug.Log("Loading Level 5");
            }
        }

        specialItemTimeSet(specialItem);
        specialItemOnce = true;
        gameTimer = getTimeLimit();
        resetAllScores();                            // resetting all item counter of all players
        uiController.displayItemCount();
        uiController.displayWave();
        generateRocks();
        gridScript.CreateGrid();
    }

    void destoryAllRocks() {
        for (int i = 0; i < rocks.Count; i++)           // destroy all rocks
            Destroy(rocks[i]);
        rocks.Clear();
        emptyBlocks.Clear();
        for (int x = 1; x <= mapSizeX; x++) {              // resetting the wallTiles
            for (int z = 1; z <= mapSizeZ; z++) {
                emptyBlocks.Add(new emptySpace(x, z));
                if (wallTiles[x, z] == 6) {
                    wallTiles[x, z] = 5;
                }
            }
        }
        if(item != null)
            removeItem();                       // removes point item
        if(specialItem != null)
            removeItem(true);                   // removes speed item
    }

    void generatePlayer()                                   // always call this after generateRocks
    {
        for (int i = 0; i < players.Count; i++) {
            while (players[i] == null)                                                                    // placing a player in the game world
            {
                Debug.Log("player" + i + " generated");
                int margin = 5;
                int randX = Random.Range(margin, gridScript.gridSizeX - margin);
                int randY = Random.Range(margin, gridScript.gridSizeY - margin);
                Vector3 randLocation;
                while (!gridScript.grid[randX, randY].walkable) {
                    randX = Random.Range(margin, gridScript.gridSizeX - margin);
                    randY = Random.Range(margin, gridScript.gridSizeY - margin);
                }
                randLocation = gridScript.grid[randX, randY].worldPosition;
                
                players[i] = (GameObject)Instantiate(playerPrefabs[i], new Vector3(randLocation.x, 0.5f, randLocation.z), Quaternion.identity);
            }
        }
    }

    //for debugging
    void showList() {
        Debug.Log("List size : " + emptyBlocks.Count);
        string thisLine = "";

        for (int z = mapSizeZ; z >= 1; z--) {
            for (int x = 1; x <= mapSizeX; x++) {
                thisLine += "[" + wallTiles[x, z] + "]";

            }
            Debug.Log(thisLine);
            thisLine = "";
        }
        for (int i = 0; i < emptyBlocks.Count; i++) {
            Debug.Log("x =" + emptyBlocks[i].getXLocation() + "  z =" + emptyBlocks[i].getZLocation());
        }
    }

    void resetBoard()                                       // resets the board
    {
        for (int x = 1; x <= mapSizeX; x++)
            for (int z = 1; z <= mapSizeZ; z++)
                wallTiles[x, z] = 5;                                            // resets all of the wallTiles
        for (int i = 0; i < rocks.Count; i++)
            rocks[i].GetComponent<RockScript>().destorySelf();                  // destorys all of the rocks
    }

    public void AIPathRequests() {
        for (int i = 1; i < players.Count; i++) {
            if (players[i] != null) {
                if (currentState == GameState.Playing) {
                    Unit tempScript = players[i].GetComponent<Unit>();
                    Debug.Log("making " + i + "'s request");
                    if (tempScript.goingForPoint) { // currently going after points
                        Debug.Log("pursueing Point!");
                        // if not the closest && feel lazy
                        if (!isClosest(players[i]) && !tempScript.goForPoint(Random.Range(0.0f, 10.0f))) {
                            tempScript.goingForPoint = false;
                            tempScript.wondering = true;
                            wonderAround(players[i]);
                        }
                        // otherwise keep pursuing for points
                        else if (tempScript.target != item.transform.position) {
                            tempScript.target = item.transform.position;
                            tempScript.makePathRequest();
                        }
                    }
                    else if (tempScript.goingForSpeed) { // currently going after speed
                        Debug.Log("pursueing speed!");
                        if (!isSepcialAvailable) {  // special item not exiting anymore, go for regular item
                            tempScript.goingForSpeed = false;
                            // if I don't wanna be lazy
                            // OR not the closest but still  want to go for it
                            if (isClosest(players[i]) || tempScript.goForPoint(Random.Range(0.0f, 10.0f))) {
                                tempScript.goingForPoint = true;
                                tempScript.target = item.transform.position;
                                tempScript.makePathRequest();
                            }
                            // not closest and want to wonder
                            else { //wonder
                                tempScript.wondering = true;
                                wonderAround(players[i]);
                            }
                        }
                        // else if special item exist, keep going for it
                        else if (!tempScript.moving) {
                            tempScript.target = specialItem.transform.position;
                            tempScript.makePathRequest();
                        }
                    }
                    else {
                        Debug.Log("pursueing neither!");
                        if (tempScript.wondering) { // currently wondering
                            Debug.Log("Currently wondering");
                            // if speed item is available and want to pursue it
                            if (isSepcialAvailable && players[i].GetComponent<Unit>().goForSpeed(Random.Range(0.0f, 10.0f))) {
                                Debug.Log("gonna pursue special!");
                                tempScript.wondering = false;
                                tempScript.goingForSpeed = true;
                                tempScript.target = specialItem.transform.position;
                                tempScript.makePathRequest();
                            }
                            // if closest to item or want to pursue an item
                            else if (isClosest(players[i]) || tempScript.goForPoint(Random.Range(0.0f, 10.0f))) {
                                Debug.Log("gonna pursue point!");
                                tempScript.wondering = false;
                                tempScript.goingForPoint = true;
                                tempScript.target = item.transform.position;
                                tempScript.makePathRequest();
                            }
                            // otherwise don't do anything. aka keep moving to wondering destination
                        }
                        else if (!tempScript.moving) { // if currently not doing anything, wonder around!
                            Debug.Log("gonna wonder!");
                            tempScript.wondering = true;
                            wonderAround(players[i]);
                        }
                    }
                }
                else            // game is paused
                    players[i].GetComponent<Unit>().StopCoroutine("FollowPath");
            }
        }
        
    }

    public void wonderAround(GameObject charactor) {
        Vector3 randLocation = randomLocation();
        charactor.GetComponent<Unit>().wonderingTarget = new Vector3(randLocation.x, 0.5f, randLocation.z);
        charactor.GetComponent<Unit>().makeWonderingPathRequest();
    }

    public void AIPathStop() {
        for (int i = 1; i < players.Count; i++) {
            if (players[i] != null) {
                Debug.Log("player" + i + " stopped");
                players[i].GetComponent<Unit>().StopCoroutine("FollowPath");
            }
        }
    }

    public Vector3 randomLocation() {
        int randX = Random.Range(0, gridScript.gridSizeX);
        int randY = Random.Range(0, gridScript.gridSizeY);
        Vector3 randLocation = Vector3.zero;

        bool isGoodLocation = false;
        while (!isGoodLocation) {                     // make sure not to over lap item with a rock
            isGoodLocation = true;                  // assume that the location is good so far

            // making sure that player doesn't overlap with rock
            while (!gridScript.grid[randX, randY].walkable) {
                randX = Random.Range(0, gridScript.gridSizeX);
                randY = Random.Range(0, gridScript.gridSizeY);
            }
            randLocation = gridScript.grid[randX, randY].worldPosition;

            if (checkPositionWithPlayers(randLocation.x, randLocation.z, 5.2f)) {
                isGoodLocation = false; // too close to a player
                                        // find new position
                randX = Random.Range(0, gridScript.gridSizeX);
                randY = Random.Range(0, gridScript.gridSizeY);
                break;
            }
        }
        return randLocation;
    }

    public void itemSpawn(bool isSpecial = false)                // spawn an item on to the game world !!!!! USE CHECKPLAYEROVERLAP FUNCTION
    {
        Vector3 randLocation = randomLocation();
        if (!isSpecial) {
            GameObject temp = (GameObject)Instantiate(pickUp, new Vector3(randLocation.x, 0.5f, randLocation.z), Quaternion.identity);
            item = temp;
        }
        else {
            GameObject temp = (GameObject)Instantiate(blueItem, new Vector3(randLocation.x, 0.5f, randLocation.z), Quaternion.identity);
            specialItem = temp;
            isSepcialAvailable = true;
        }
    }

    public void removeItem(bool isSpecial = false) {    // not using in PlayerBase anymore
        if (!isSpecial)
            Destroy(item);
        else {
            isSepcialAvailable = false;
            Destroy(specialItem);
        }
    }

    public void specialItemTimeSet(bool levelWithItem = false) {
        if (levelWithItem)
            specialItemTime = Random.Range(timeLimit - 3.0f, timeLimit / 2);
        else
            specialItemTime = -999.999f;
    }

    public void moveItem() {  // deletes the old item and make a new one
        Destroy(item);
        itemSpawn();
        //AIPathRequests();
    }

    public bool isClosest(GameObject me) {
        Transform dest = item.transform;
        // getting current object's distance to target
        float myDist = dist(dest.position.x, me.transform.position.x, dest.position.z, me.transform.position.z);

        for(int i = 1; i < players.Count; i++) {
            if(players[i] != null && players[i] != me) {
                float tempDist = dist(dest.position.x, players[i].transform.position.x, dest.position.z, players[i].transform.position.z);
                if (tempDist < myDist)
                    return false;
            }
        }
        return true;
    }

    float dist(float ax, float bx, float az, float bz)      // calculates the distance between two points
    {
        return Mathf.Sqrt(((ax - bx) * (ax - bx)) + ((az - bz) * (az - bz)));
    }

    public int countPlayers() {
        int num = 0;
        for (int i = 0; i < players.Count; i++) {
            if (players[i] != null) {
                num++;
            }
        }
        return num;
    }

    bool checkPositionWithPlayers(float x, float z, float d) {
        for (int i = 0; i < players.Count; i++) {
            if(players[i] != null) {
                if (dist(players[i].transform.position.x, x, players[i].transform.position.z, z) < d)
                    return false;
            }
        }
        return true;
    }

    int generateGroupOfRocks(int amount, int x, int z) // return how many rocks actually generated !!!!!NOT FINISHED!!!!!!!!
    {
        bool isGood = true;
        int currentX = x;
        int currentZ = z;
        int generatedRocks = 0;
        int direction = Mathf.FloorToInt(Random.Range(0, 4 - 0.00001f)); // 0=N 1=E 2=S 3=W
        while (isGood && generatedRocks < amount) {
            switch (direction) {
                case 0:             // North
                    if (checkPositionWithPlayers(currentX, currentZ + 1, .9f) && wallTiles[currentX, currentZ + 1] == 5)     // if empty spot exists
                    {
                        wallTiles[currentX, currentZ + 1] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn()) {
                            currentZ++;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else {
                            wallTiles[currentX, currentZ + 1] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 1:             // East
                    if (checkPositionWithPlayers(currentX + 1, currentZ, .9f) && wallTiles[currentX + 1, currentZ] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX + 1, currentZ] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn()) {
                            currentX++;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else {
                            wallTiles[currentX + 1, currentZ] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 2:             // South
                    if (checkPositionWithPlayers(currentX, currentZ - 1, .9f) && wallTiles[currentX, currentZ - 1] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX, currentZ - 1] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn()) {
                            currentZ--;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else {
                            wallTiles[currentX, currentZ - 1] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 3:             // West
                    if (checkPositionWithPlayers(currentX - 1, currentZ, .9f) && wallTiles[currentX - 1, currentZ] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX - 1, currentZ] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn()) {
                            currentX--;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else {
                            wallTiles[currentX - 1, currentZ] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                default:
                    Debug.Log("incorrect direction");
                    isGood = false;
                    break;
            }

            if (isGood)  // if the chain of rocks are still good to grow
            {
                float probability = Random.Range(0, 10);
                if (probability < 1.5)      //turn left     15%
                    direction--;
                else if (probability > 8.5) // turn right   15%
                    direction++;

                if (direction < 0)          // fixing directions to keep it between 0-3
                    direction = direction + 4;
                else if (direction > 3)
                    direction = direction % 4;
            }
        }

        return generatedRocks;
    }

    void insertRock(int x, int z)   // edits wallTiles and emptyBlocks at the same time
    {
        wallTiles[x, z] = 6;
        GameObject currentRock = (GameObject)Instantiate(rock, new Vector3(x, 0.5f, z), Quaternion.identity);
        rocks.Add(currentRock);
        for (int i = 0; i < emptyBlocks.Count; i++) {
            if (emptyBlocks[i].getXLocation() == x && emptyBlocks[i].getZLocation() == z) {
                emptyBlocks.RemoveAt(i);
                break;
            }
        }
    }

    bool checksForLegalRockSpawn()  // use recursion to check for rock spawn legality  <----- I think I just finished?????? try using it
    {
        int x = 0;
        int z = 0;
        bool found = false;
        for (int a = 1; a <= mapSizeX; a++) {            // looking for the first occurance of "nothing"
            for (int b = 1; b <= mapSizeZ; b++) {
                if (wallTiles[a, b] == 5) {
                    x = a;
                    z = b;
                    found = true;
                    break;
                }
            }
            if (found)
                break;
        }

        floodCheck(x, z);                               // flood fills all connected
        int totalNumberOfNothing = 0;
        int connectedNothings = 0;
        for (int a = 1; a <= mapSizeX; a++) {            // counts the number of inverted "Nothing"
            for (int b = 1; b <= mapSizeZ; b++) {
                if (wallTiles[a, b] == 5)
                    totalNumberOfNothing++;
                else if (wallTiles[a, b] == 9) {
                    connectedNothings++;
                    totalNumberOfNothing++;
                    wallTiles[a, b] = 5;                // turns 9's back to nothing
                }
            }
        }
        return (connectedNothings == totalNumberOfNothing);
    }

    void floodCheck(int x, int z) {
        if (wallTiles[x, z] != 5) // 5: nothing thus, stop if it is not a nothing
            return;
        wallTiles[x, z] = 9;      // if it is nothing, make it NOT a nothing (9)
        floodCheck(x + 1, z);   // right
        floodCheck(x - 1, z);   // left
        floodCheck(x, z + 1);   // up
        floodCheck(x, z - 1);   // down
    }

    public void handleScores(string ptag, string what) {
        if(what == "reset") {
            if (ptag == "player1")
                scores[0] = 0;
            else if (ptag == "player2")
                scores[1] = 0;
            else if (ptag == "player3")
                scores[2] = 0;
            else if (ptag == "player4")
                scores[3] = 0;
        }
        else if(what == "count") {
            if (ptag == "player1")
                scores[0] += 1;
            else if (ptag == "player2")
                scores[1] += 1;
            else if (ptag == "player3")
                scores[2] += 1;
            else if (ptag == "player4")
                scores[3] += 1;
        }
    }

    public void resetAllScores() {
        scores[0] = 0;
        scores[1] = 0;
        scores[2] = 0;
        scores[3] = 0;
    }

    public void destroyAllPlayers() {
        for (int i = 1; i < players.Count; i++) {
            if (players[i] != null) {
                Destroy(players[i]);
            }
        }
    }

    //setters
    public void setNumOfLasers(int x)                       // sets the number of lasers to be fired at a time
    {
        numOfLasers = x;
    }

    public void setTimeBetweenLasers(float x)               // sets the duration time between laser blasts
    {
        timeBetweenLasers = x;
    }

    public void setWarningTime(float x) {
        warningTime = x;
    }

    public void setCurrentState(GameState state) {
        currentState = state;
    }

    void setTimeLimit(float t) {
        timeLimit = t;
    }

    void setGameStart(bool x) {                              // sets flag that will start the game
        gameStart = x;
    }

    public void setSucessful(bool x) {
        successful = x;
    }

    public void setIsSepcialAvailable(bool x) {
        isSepcialAvailable = false;
    }

    //getters
    public GameState getCurrentState() {
        return currentState;
    }

    public int getItemGoal() {
        return itemGoal;
    }

    public float getWarningTime() {
        return warningTime;
    }

    public GameState getGameState() {
        return currentState;
    }

    float getTimeLimit() {
        return timeLimit;
    }

    bool getGameStart() {
        return gameStart;
    }

    public int getScores(string tag) {
        if (tag == "player1")
            return scores[0];
        else if (tag == "player2")
            return scores[1];
        else if (tag == "player3")
            return scores[2];
        else if (tag == "player4")
            return scores[3];
        return 0;
    }

    // buttons
    public void HomePress() {
        SceneManager.LoadScene("levelSelection");
    }
}