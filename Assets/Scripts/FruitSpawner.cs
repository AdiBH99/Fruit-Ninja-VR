using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FruitSpawner : MonoBehaviour
{
    public float startTime = 1f;
    public float intervalTime = 3f;

    public int numberOfFruitsFromEachType = 20;

    public int maxNumberOfSpawnFruitsAtATime = 4;
    // List of Fruits to be spawn 
    public List<GameObject> FruitPrefabs;

    // fruits that are now being spawn
    public List<GameObject> UsedFruit = new List<GameObject>();
    // fruits that are not being spawn
    public List<GameObject> UnusedFruit = new List<GameObject>();

    public List<GameObject> UsedBombs = new List<GameObject>();
    public List<GameObject> UnusedBombs = new List<GameObject>();

    public float bomb_prob = 0.2f;
    
    // List of Bombs to be spawn
    public List<GameObject> BombPrefabs;

    // List of spawning locations
    //public Transform SpawnPoints;
    List<Vector3> SpawnPointList = new List<Vector3>();

    public GameObject swordAttacher;


    public GameObject mainCamera; // Reference to the main camera

    public GameObject cubePrefab;


    // spawning location paramas
    public float randomXStart = -0.8f;
    public float randomXEnd = 0.8f;
    public float randomYStart = 0.8f;
    public float randomYEnd = 6f;
    public float randomZStart = -0.8f;
    public float randomZEnd = 0.8f;

    public float upForceStart = 0f;
    public float upForceEnd = 0f;
    public float forwardForceStart = 10f;
    public float forwardForceEnd = 20f;
    public float rightForceStart = -5f;
    public float rightForceEnd = 50f;


    // Start is called before the first frame update
    void Start()
    {
        // StartSpawning();
        // StartGameInPool();
    }

    public void StartSpawning()
    {
        // initialize spawn points  
        // foreach (Transform spawn_point in SpawnPoints)
        // {
            
        // }
        // Vector3 point1 = new Vector3(3f,5f,3f);
        // Vector3 point2 = new Vector3(-7f,5f,-7f);
        // // Vector3 point3 = new Vector3(-10f,5f, 10f);
        // SpawnPointList.Add(point1);
        // SpawnPointList.Add(point2);
        // SpawnPointList.Add(point3);
        // Vector3 positionOffset = new Vector3(0f, 3f, 0);
        // // todo add for loop that generates random position offsets
        // SpawnPointList.Add(mainCamera.transform.TransformVector(positionOffset));
        for (int i = 0; i < 10; i++)  // Replace 'numberOfOffsets' with how many offsets you want to generate
        {
            float randomX = Random.Range(randomXStart, randomXEnd);
            float randomY = Random.Range(randomYStart, randomYEnd);
            float randomZ = Random.Range(randomZStart, randomZEnd);

            Vector3 positionOffset = new Vector3(randomX, randomY, randomZ);
            SpawnPointList.Add(mainCamera.transform.TransformVector(positionOffset));
            // Instantiate(cubePrefab, positionOffset, Quaternion.identity);
        }

        // for regular fruits
        foreach (GameObject fruit in FruitPrefabs)
        {
            for (int i = 0; i < numberOfFruitsFromEachType; i++)
            {
                GameObject fruitGameObject = Instantiate(fruit, Vector3.zero, Quaternion.identity, transform);
                fruitGameObject.SetActive(false);
                UnusedFruit.Add(fruitGameObject);
            }
        }

        //for bombs
        for (int i = 0; i < numberOfFruitsFromEachType; i++)
        {
            GameObject bombGameObject = Instantiate(BombPrefabs[0], Vector3.zero, Quaternion.identity, transform);
            bombGameObject.SetActive(false);
            UnusedBombs.Add(bombGameObject);
        }


    }

    //spawn regular fruit
    public void SpawnFruit()
    {
        for (int i = 0; i < Random.Range(1, maxNumberOfSpawnFruitsAtATime); i++)
        {
            if (Random.value < bomb_prob && UnusedBombs.Count > 0)
            {
                Vector3 randomPos = SpawnPointList[Random.Range(0, SpawnPointList.Count)];
                GameObject randomBomb = UnusedBombs[Random.Range(0, UnusedBombs.Count)];
                randomBomb.transform.localPosition = randomPos;
                AddSpecialForce(randomBomb);
                UsedBombs.Add(randomBomb);
                UnusedBombs.Remove(randomBomb);

            }
            else if (UnusedFruit.Count > 0 || UsedFruit.Count < 15)
            {
                Vector3 randomPos = SpawnPointList[Random.Range(0, SpawnPointList.Count)];

                GameObject randomFruit = UnusedFruit[Random.Range(0, UnusedFruit.Count)];
                randomFruit.transform.localPosition = randomPos;
                AddSpecialForce(randomFruit);
                UsedFruit.Add(randomFruit);
                UnusedFruit.Remove(randomFruit);
            }
        }
    }
    //add random force in a fruit
    void AddSpecialForce(GameObject obj)
    {
        obj.SetActive(true);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
            // Use main camera's direction for the force
        Vector3 cameraUp = Camera.main.transform.up;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        rig.AddForce(obj.transform.up * Random.Range(upForceStart, upForceEnd) * (1f / Time.timeScale));
        rig.AddForce(obj.transform.forward * Random.Range(forwardForceStart, forwardForceEnd) * (1f / Time.timeScale));
        rig.AddForce(obj.transform.right * Random.Range(rightForceStart, rightForceEnd) * (1f / Time.timeScale));
    }

    public void StartGameInPool()
    {
        InvokeRepeating("SpawnFruit", startTime, intervalTime);
        InvokeRepeating("RecycleUsedFruits", 0f, 5f);
    }
    // reset all objects and cancel spawn functions
    // public void ResetPool()
    // {
    //     CancelInvoke("SpawnFruit");
    //     foreach (GameObject fruitGameObj in UsedFruit.ToArray())
    //     {
    //         Fruit fruit = fruitGameObj.GetComponent<Fruit>();
    //         //ReycleFruit(fruit);
    //     }
    // }
    // reset all objects and cancel spawn functions
    // reset all objects and cancel spawn functions
    // public void ResetPool()
    // {
    //     CancelInvoke("SpawnFruit");
    //     CancelInvoke("SpawnSpecialFruits");
    //     foreach (GameObject fruitGameObj in UsedFruit.ToArray())
    //     {
    //         Fruit fruit = fruitGameObj.GetComponent<Fruit>();
    //         RecycleFruit(fruit);
    //     }
    // }
    // public void RecycleFruit(Fruit fruit)
    // {
    //     fruit.ResetVelocity();
    //     UsedFruit.Remove(fruit.gameObject);
    //     UnusedFruit.Add(fruit.gameObject);
    // }

    public void RecycleUsedFruits()
    {
        foreach (GameObject fruitGameObj in UsedFruit.ToArray())
        {
            Fruit fruit = fruitGameObj.GetComponent<Fruit>();
            if (fruit.IsDeleted())
            {
                UsedFruit.Remove(fruit.gameObject);
                UnusedFruit.Add(fruit.gameObject);
                fruit.ResetDeleteMode();
            }
        }
        foreach (GameObject bombGameObject in UsedBombs.ToArray())
        {
            Fruit fruit = bombGameObject.GetComponent<Fruit>();
            if (fruit.IsDeleted())
            {
                UsedBombs.Remove(fruit.gameObject);
                UnusedBombs.Add(fruit.gameObject);
                fruit.ResetDeleteMode();
            }
        }
    }
    
    public void StopSpawning()
    {
        CancelInvoke("SpawnFruit");
        CancelInvoke("RecycleUsedFruits");
        UsedFruit.Clear();
        UnusedFruit.Clear();
        UsedBombs.Clear();
        UnusedBombs.Clear();
    }
}
