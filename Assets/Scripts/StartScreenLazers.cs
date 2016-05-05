using UnityEngine;
using System.Collections;

public class StartScreenLazers : MonoBehaviour {

    public GameObject lazer;

    private float timer;
    private float bamTime;

	// Use this for initialization
	void Start () {
        timer = 0.0f;
        bamTime = 3.0f;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer > bamTime) {
            Random.seed = (int)Random.Range(0f, 9999f);
            createFromTop(Mathf.FloorToInt(Random.Range(0, 5 - 0.00001f)) );
            createFromBottom(Mathf.FloorToInt(Random.Range(0, 5 - 0.00001f)));
            createFromLeft(Mathf.FloorToInt(Random.Range(0, 5 - 0.00001f)));
            createFromRight(Mathf.FloorToInt(Random.Range(0, 5 - 0.00001f)));
            Debug.Log("BAM!");
            timer = 0.0f;
        }
	}

    void createFromTop(int x) {
        for(int i = 0; i < x; i++) {
            Random.seed = (int)System.DateTime.Now.Ticks;
            GameObject tempLazer = (GameObject)Instantiate(lazer, new Vector3(Random.Range(-7.0f, 7.0f), 9.0f, 2.0f), Quaternion.identity);
            tempLazer.GetComponent<LaserScript>().setTowards("DOWN");
        }
    }

    void createFromBottom(int x) {
        for (int i = 0; i < x; i++) {
            Random.seed = (int)System.DateTime.Now.Ticks;
            GameObject tempLazer = (GameObject)Instantiate(lazer, new Vector3(Random.Range(-7.0f, 7.0f), -9.0f, 2.0f), Quaternion.identity);
            tempLazer.GetComponent<LaserScript>().setTowards("UP");
        }
    }

    void createFromLeft(int x) {
        for (int i = 0; i < x; i++) {
            Random.seed = (int)System.DateTime.Now.Ticks;
            GameObject tempLazer = (GameObject)Instantiate(lazer, new Vector3(-10.0f, Random.Range(-6.0f, 6.0f), 2.0f), Quaternion.identity);
            tempLazer.GetComponent<LaserScript>().setTowards("WestWall");
        }
    }

    void createFromRight(int x) {
        for (int i = 0; i < x; i++) {
            Random.seed = (int)System.DateTime.Now.Ticks;
            GameObject tempLazer = (GameObject)Instantiate(lazer, new Vector3(10.0f, Random.Range(-6.0f, 6.0f), 2.0f), Quaternion.identity);
            tempLazer.GetComponent<LaserScript>().setTowards("EastWall");
        }
    }

}
