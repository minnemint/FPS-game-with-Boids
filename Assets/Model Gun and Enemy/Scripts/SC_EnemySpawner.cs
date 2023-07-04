using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_EnemySpawner : MonoBehaviour
{
    public GameObject droneEnemyPrefab;
    public GameObject pathfindEnemyPrefab;
    public SC_DamageReceiver player;
    public Texture crosshairTexture;
    public float spawnInterval = 2; //Spawn new enemy each n seconds
    public int enemiesPerWave = 3; //How many enemies per wave
    public float droneRatio = 0.8f; //How many drones ratio
    public Transform[] droneSpawnPoints;
    public Transform[] pathfindSpawnPoints;
    public float waveIntermissionTime = 3f;
    private bool gameEnded = false;

    float nextSpawnTime = 0;
    public int totalNumberOfWaves = 3;
    int waveNumber = 1;
    bool waitingForWave = true;
    float newWaveTimer = 0;
    int enemiesToEliminate;
    //How many enemies we already eliminated in the current wave
    int enemiesEliminated = 0;
    int totalEnemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Wait waveIntermissionTime seconds for new wave to start
        newWaveTimer = waveIntermissionTime;
        waitingForWave = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForWave)
        {
            if (totalNumberOfWaves >= waveNumber)
            {
                if (newWaveTimer >= 0)
                {
                    newWaveTimer -= Time.deltaTime;
                }
                else
                {
                    //Initialize new wave
                    enemiesToEliminate = waveNumber * enemiesPerWave;
                    enemiesEliminated = 0;
                    totalEnemiesSpawned = 0;
                    waitingForWave = false;
                }
            }
            else
            {
                //end game
                gameEnded = true;
                if (Input.GetKey(KeyCode.Space))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
        else
        {
            if(Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + spawnInterval;

                //Spawn enemy 
                if(totalEnemiesSpawned < enemiesToEliminate)
                {
                    SpawnRandomEnemy();
                }
            }
        }

        if (player.playerHP <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
    }

    private void SpawnRandomEnemy()
    {
        if (Random.Range(0f, 1f) < droneRatio)
        {
            //spawn drone
            Transform randomPoint = droneSpawnPoints[Random.Range(0, droneSpawnPoints.Length - 1)];
            GameObject droneEnemy = Instantiate(droneEnemyPrefab, randomPoint.position, Quaternion.identity);
            SC_NPCEnemy npc = droneEnemy.GetComponent<SC_NPCEnemy>();
            npc.es = this;
            FlockManager.FM.allFish.Add(droneEnemy);
        }
        else
        {
            //spawn pathfind enemy
            Transform randomPoint = pathfindSpawnPoints[Random.Range(0, pathfindSpawnPoints.Length - 1)];
            GameObject pathfindEnemy = Instantiate(pathfindEnemyPrefab, randomPoint.position, Quaternion.identity);
            PathfindEnemy npc = pathfindEnemy.GetComponent<PathfindEnemy>();
            npc.playerTransform = player.transform;
            npc.es = this;
        }

        totalEnemiesSpawned++;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, Screen.height - 35, 100, 25), ((int)player.playerHP).ToString() + " HP");
        GUI.Box(new Rect(Screen.width / 2 - 35, Screen.height - 35, 70, 25), player.weaponManager.selectedWeapon.bulletsPerMagazine.ToString());

        if(player.playerHP <= 0 || gameEnded)
        {
            GUI.Box(new Rect(Screen.width / 2 - 85, Screen.height / 2 - 20, 170, 40), "Game Over\n(Press 'Space' to Restart)");
        }
        else
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - 4, Screen.height / 2 - 4, 8, 8), crosshairTexture);
        }

        GUI.Box(new Rect(Screen.width / 2 - 50, 10, 100, 25), (enemiesToEliminate - enemiesEliminated).ToString());

        if (waitingForWave && !gameEnded)
        {
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 4 - 12, 250, 25), "Waiting for Wave " + waveNumber.ToString() + " (" + ((int)newWaveTimer).ToString() + " seconds left...)");
        }
    }

    //drone
    public void EnemyEliminated(SC_NPCEnemy enemy)
    {
        enemiesEliminated++;
        FlockManager.FM.allFish.Remove(enemy.gameObject);
        if(enemiesToEliminate - enemiesEliminated <= 0)
        {
            //Start next wave
            newWaveTimer = waveIntermissionTime;
            waitingForWave = true;
            waveNumber++;
        }
    }

    //pathfind
    public void EnemyEliminated(PathfindEnemy enemy)
    {
        enemiesEliminated++;
        if (enemiesToEliminate - enemiesEliminated <= 0)
        {
            //Start next wave
            newWaveTimer = waveIntermissionTime;
            waitingForWave = true;
            waveNumber++;
        }
    }
}