using UnityEngine;
using System.Collections;
using System.Collections.Generic;           // for using list

public class GameController : MonoBehaviour
{
    public TileType[] tileTypes;
    public GameObject[] pickUps;
    public GameObject rock;
    public List<GameObject> players = new List<GameObject>(2);

    struct emptySpace
    {
        public emptySpace(int x, int z) { xLocation = x; zLocation = z; }
        int xLocation;
        int zLocation;
        public int getXLocation() { return xLocation; }
        public int getZLocation() { return zLocation; }
        public bool equalTo(emptySpace other)
        {
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
    private int wave;
    private float timeBetweenLasers;                // duration between the lasers
    private float warningTime;
    private float lazertimer = 0.0f;
    private float gameTimer;
    private int numOfLasers;
    private int mapSizeX = 13;
    private int mapSizeZ = 13;
    private GameObject[] wall;                                  // array of walls in the game
    private List<GameObject> rocks = new List<GameObject>();    // array of rocks in the game
    private List<emptySpace> emptyBlocks = new List<emptySpace>();
    private Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();
    private bool gameStart = false;

    private UIController uiController;
    private Grid gridScript;
    public enum GameState {Transition, Playing, Gameover }
    GameState currentState;

    int[,] wallTiles;       // 0:NorthWall 1: SouthWall 2:EastWall 3:WestWall 
                            // 4: coner Rock == rock 5: Nothing 6: Rocks 7:item 8:player 9: (used for rockchecking)

    // Use this for initialization
    void Start()
    {
        gridScript = GetComponent<Grid>();
        uiController = GetComponent<UIController>();
        wave = 0;
        uiController.setStartText("");
        generateAllTiles();
        generateWallTileVisual();
        generatePlayer();                           // creates player (always do this after generating rocks)
        setCurrentState(GameState.Transition);
    }

    void Update()
    {
        switch(currentState)
        {
            case GameState.Transition:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    setupWave();
                    uiController.startText.text = "";
                    setGameStart(true);
                    //showList();                                                    // debugging line
                    setCurrentState(GameState.Playing);
                }
                break;
            case GameState.Playing:
                gameTimer -= Time.deltaTime;                                          // count down the game time
                lazertimer += Time.deltaTime;
                if (gameTimer <= 0f)                                                  // round over
                {
                    destoryAllRocks();                                                   // reseting for next round
                    lazertimer = 0;
                    gameTimer = getTimeLimit();
                    uiController.setStartText("Press Spacebar to start the next wave!");
                    setCurrentState(GameState.Transition);
                }
                uiController.displayTime(gameTimer);                                                      // displays time
                if (lazertimer > timeBetweenLasers && gameStart)                                   // 4 seconds between the lasers
                {
                    lazertimer = 0;
                    generateLasers(numOfLasers);
                }
                break;
            case GameState.Gameover:

                break;
        }
    }

    void generateAllTiles()                                // Generating the tile pieces
    {
        wallTiles = new int[(mapSizeX + 2), (mapSizeZ + 2)];

        //Nothingness
        for (int x = 1; x <= mapSizeX; x++){
            for (int z = 1; z <= mapSizeZ; z++){
                emptyBlocks.Add(new emptySpace(x, z));
                wallTiles[x, z] = 5;
            }
        }
        // N & S walls
        for (int x = 1; x <= mapSizeX; x++)
        {
            //South Wall
            wallTiles[x, 0] = 1;
            //North Wall
            wallTiles[x, mapSizeX + 1] = 0;
        }

        // E & W walls
        for (int z = 1; z <= mapSizeZ; z++)
        {
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
        for (int x = 0; x < mapSizeX + 2; x++){
            for (int z = 0; z < mapSizeZ + 2; z++){
                if (wallTiles[x, z] >= 0 && wallTiles[x, z] < 5){
                    TileType tt = tileTypes[wallTiles[x, z]];
                    GameObject currentWall = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    if (wallTiles[x, z] < 4){
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
            if (!wall[rand].GetComponent<WallScript>().getIsCharging())
            {
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
            
            if (checkRockPlayerOverlapLgality(thisSpace.getXLocation(), thisSpace.getZLocation())) // checking to see if it overlaps with players AND if this is a empty spot                                                    // avoiding overlaps
            {
                wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] = 6;    // try putting the rock in this position
                if (checksForLegalRockSpawn())
                {        // check to see if it traps any space
                    GameObject currentRock = (GameObject)Instantiate(rock, new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity);
                    rocks.Add(currentRock);
                    emptyBlocks.RemoveAt(randomLocation);
                    i++;
                    int passingNum = maxRockNum - i;
                    if (maxRockNum - i > groupingMax)
                        passingNum = groupingMax;
                    i = i + generateGroupOfRocks(passingNum, thisSpace.getXLocation(), thisSpace.getZLocation());
                }
                else
                {
                    wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] = 5;        // if it does traps a space don't put the rock there
                }
            }
        }
    }

    bool checkRockPlayerOverlapLgality(int x, int z)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (dist(x, players[i].transform.position.x, z, players[i].transform.position.z) <= .9f)
                return false;
        }
        return true;
    }
    
    // ways to check item overlap
    void setupWave()
    {
        wave++;
        if (wave < 3)
        {
            maxRockNum = 40;
            itemGoal = 15;
            setTimeLimit(30.0f);
            groupingMax = 5;
            timeBetweenLasers = 4.0f;                   // duration between the lasers
            warningTime = 3.0f;
            numOfLasers = 5;                            // number of lasers to be fired at a time
        }
        else if (wave < 5)
        {
            maxRockNum = 35;
            itemGoal = 10;
            groupingMax = 3;
            setTimeLimit(60.0f);
            timeBetweenLasers = 4.0f;                   // duration between the lasers
            warningTime = 3.0f;
            numOfLasers = 8;                            // number of lasers to be fired at a time
        }
        else if (wave < 7)
        {
            maxRockNum = 70;
            itemGoal = 10;
            groupingMax = 3;
            setTimeLimit(60.0f);
            timeBetweenLasers = 3.0f;                   // duration between the lasers
            warningTime = 2.0f;
            numOfLasers = 12;                            // number of lasers to be fired at a time
        }
        else if (wave < 8)
        {
            maxRockNum = 15;
            itemGoal = 15;
            setTimeLimit(30.0f);
            groupingMax = 2;
            timeBetweenLasers = 3.0f;                   // duration between the lasers
            warningTime = 2.0f;
            numOfLasers = 12;                            // number of lasers to be fired at a time
        }
        else if (wave < 10)
        {

        }
        else if (wave >= 12)
        {

        }
        gameTimer = getTimeLimit();
        for (int i = 0; i < players.Count; i++) // resetting all item counter of all players
            players[i].GetComponent<PlayersBase>().resetItemNum();

        uiController.displayItemCount();
        uiController.displayWave();
        generateRocks();
        gridScript.CreateGrid();
        for (int i = 0; i < players.Count; i++) { // instantiating all items
            itemSpawn("player" + (1 + i));
            if (i > 0) {
                players[i].GetComponent<Unit>().makePathRequest();
            }
        }
    }
    
    void destoryAllRocks()
    {
        for(int i = 0; i < rocks.Count; i++)           // destroy all rocks
            Destroy(rocks[i]);
        rocks.Clear();
        emptyBlocks.Clear();
        for (int x = 1; x <= mapSizeX; x++){              // resetting the wallTiles
            for(int z = 1; z <= mapSizeZ; z++){
                emptyBlocks.Add(new emptySpace(x, z));
                if (wallTiles[x, z] == 6){
                    wallTiles[x, z] = 5;
                }
            }
        }
        removeItem("player1pickUp");                       // removes player 1 pickup
        removeItem("player2pickUp");                       // removes player 2 pickup
    }

    void generatePlayer()                                   // always call this after generateRocks
    {
        bool playerPlaced = false;
        for(int i = 0; i < players.Count; i++)
        {
            while (!playerPlaced)                                                                    // placing a player in the game world
            {
                int randomLocation = Mathf.FloorToInt(Random.Range(1, emptyBlocks.Count - 0.0001f));
                emptySpace thisSpace = emptyBlocks[randomLocation];
                if (wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] == 5)              // making sure that player doesn't overlap with rock
                {
                    //wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] = 8;
                    players[i] = (GameObject)Instantiate(players[i], new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity);
                    playerPlaced = true;
                }
            }
            playerPlaced = false;
        }
    }
    
    //for debugging
    void showList()
    {
        Debug.Log("List size : " + emptyBlocks.Count);
        string thisLine= "";
        
        for (int z = mapSizeZ; z >= 1; z--)
        {
            for (int x = 1; x <= mapSizeX; x++)
            {
                thisLine += "[" + wallTiles[x,z] + "]";

            }
        Debug.Log(thisLine);
        thisLine = "";
        }
        for(int i = 0; i < emptyBlocks.Count; i++)
        {
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
    
    public void itemSpawn(string playerInfo)                                 // spawn an item on to the game world !!!!!!!!!!!!!!!! USE CHECKPLAYEROVERLAP FUNCTION
    {
        int randomLocation = Mathf.FloorToInt(Random.Range(1, emptyBlocks.Count - 0.0001f));
        emptySpace thisSpace = emptyBlocks[randomLocation];
        bool isGoodLocation = false;
        while (!isGoodLocation)                     // make sure not to over lap item with a rock
        {
            isGoodLocation = true;                  // assume that the location is good so far
            randomLocation = Mathf.FloorToInt(Random.Range(1, emptyBlocks.Count - 0.0001f));
            thisSpace = emptyBlocks[randomLocation];
            if (wallTiles[thisSpace.getXLocation(), thisSpace.getZLocation()] == 5)    // making sure that player doesn't overlap with rock
            {
                for (int q = 0; q < players.Count; q++) { 
                    if(players[q].tag == playerInfo) {
                        if (dist(players[q].transform.position.x, thisSpace.getXLocation(), players[q].transform.position.z, thisSpace.getZLocation()) < 5.2f) {
                            isGoodLocation = false; // too close to a player
                            break;
                        }
                    }
                }

            }
         }

            if (playerInfo == "player1") {
                GameObject temp = (GameObject)Instantiate(pickUps[0], new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity);
                items.Add(playerInfo + "pickUp", temp);                 // instantiates an item
            }
            else if (playerInfo == "player2") {
                GameObject temp = (GameObject)Instantiate(pickUps[1], new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity);
                players[1].GetComponent<Unit>().target = temp.transform;
                items.Add(playerInfo + "pickUp", temp);                 // instantiates an item
                //players[1].GetComponent<Unit>().makePathRequest();
        }
            else if (playerInfo == "player3") {
                items.Add(playerInfo + "pickUp", (GameObject)Instantiate(pickUps[2], new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity));                 // instantiates an item
            }
            else if (playerInfo == "player4") {
                items.Add(playerInfo + "pickUp", (GameObject)Instantiate(pickUps[3], new Vector3(thisSpace.getXLocation(), 0.5f, thisSpace.getZLocation()), Quaternion.identity));                 // instantiates an item
            }
        }

    public void removeItem(string itemTag)
    {
        GameObject temp = null;
        if(items.TryGetValue(itemTag, out temp))
        {
            Destroy(temp);
            items.Remove(itemTag);
        }
        else
            Debug.Log("ERROR - COULD NOT FIND " + itemTag);
    }

    float dist(float ax, float bx, float az, float bz)      // calculates the distance between two points
    {
        return Mathf.Sqrt(((ax - bx) * (ax - bx)) + ((az - bz) * (az - bz)));
    }
    
    int generateGroupOfRocks(int amount, int x, int z) // return how many rocks actually generated !!!!!NOT FINISHED!!!!!!!!
    {
        bool isGood = true;
        int currentX = x;
        int currentZ = z;
        int generatedRocks = 0;
        int direction = Mathf.FloorToInt(Random.Range(0, 4 - 0.00001f)); // 0=N 1=E 2=S 3=W
        while (isGood && generatedRocks < amount){
            switch(direction)
            {
                case 0:             // North
                    if (checkRockPlayerOverlapLgality(currentX, currentZ + 1) && wallTiles[currentX, currentZ + 1] == 5)     // if empty spot exists
                    {
                        wallTiles[currentX, currentZ+1] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn())
                        {
                            currentZ++;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else
                        {
                            wallTiles[currentX, currentZ + 1] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 1:             // East
                    if (checkRockPlayerOverlapLgality(currentX + 1, currentZ) && wallTiles[currentX+1, currentZ] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX+1, currentZ] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn())
                        {
                            currentX++;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else
                        {
                            wallTiles[currentX+1, currentZ] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 2:             // South
                    if (checkRockPlayerOverlapLgality(currentX, currentZ - 1) && wallTiles[currentX, currentZ-1] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX, currentZ - 1] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn())
                        {
                            currentZ--;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else
                        {
                            wallTiles[currentX, currentZ - 1] = 5;
                            isGood = false;
                        }
                    }
                    else
                        isGood = false;
                    break;
                case 3:             // West
                    if (checkRockPlayerOverlapLgality(currentX - 1, currentZ) && wallTiles[currentX-1, currentZ] == 5)       // if empty spot exists
                    {
                        wallTiles[currentX - 1, currentZ] = 6;    // try putting the rock in this position
                        if (checksForLegalRockSpawn())
                        {
                            currentX--;
                            insertRock(currentX, currentZ);
                            generatedRocks++;
                        }
                        else
                        {
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

            if(isGood)  // if the chain of rocks are still good to grow
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
        for (int i = 0; i < emptyBlocks.Count; i++)
        {
            if (emptyBlocks[i].getXLocation() == x && emptyBlocks[i].getZLocation() == z)
            {
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
        for (int a = 1; a <= mapSizeX; a++){            // looking for the first occurance of "nothing"
            for (int b = 1; b <= mapSizeZ; b++){
                if(wallTiles[a,b] == 5)
                {
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
                else if (wallTiles[a, b] == 9){
                    connectedNothings++;
                    totalNumberOfNothing++;
                    wallTiles[a, b] = 5;                // turns 9's back to nothing
                }
            }
        }
        return (connectedNothings == totalNumberOfNothing);
    }

    void floodCheck(int x, int z)
    {
        if (wallTiles[x, z] != 5) // 5: nothing thus, stop if it is not a nothing
            return;
        wallTiles[x, z] = 9;      // if it is nothing, make it NOT a nothing (9)
        floodCheck(x + 1, z);   // right
        floodCheck(x - 1, z);   // left
        floodCheck(x, z + 1);   // up
        floodCheck(x, z - 1);   // down
    }
   
    public void itemCount(string itemTag)
    {
        if (itemTag == "player1pickUp")
            players[0].GetComponent<PlayersBase>().inclementItemNum();
        else if (itemTag == "player2pickUp")
            players[1].GetComponent<PlayersBase>().inclementItemNum();
        else if (itemTag == "player3pickUp")
            players[2].GetComponent<PlayersBase>().inclementItemNum();
        else if (itemTag == "player4pickUp")
            players[3].GetComponent<PlayersBase>().inclementItemNum();
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

    public void setwave(int x)                             // sets the wave of the game
    {
        wave = x;
    }
    
    public void setWarningTime(float x)
    {
        warningTime = x;
    }

    public void setCurrentState(GameState state)
    {
        currentState = state;
    }

    void setTimeLimit(float t)
    {
        timeLimit = t;
    }

    void setGameStart(bool x)                               // sets flag that will start the game
    {
        gameStart = x;
    }

    //getters
    public int getItemGoal()
    {
        return itemGoal;
    }

    public int getWave()                                   // returns the wave of the game
    {
        return wave;
    }

    public float getWarningTime()
    {
        return warningTime;
    }

    public GameState getGameState()
    {
        return currentState;
    }

    float getTimeLimit()
    {
        return timeLimit;
    }

    bool getGameStart()
    {
        return gameStart;
    }
}