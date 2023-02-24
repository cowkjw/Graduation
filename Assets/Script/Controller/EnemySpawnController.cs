using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    int maximumEnemy = 9;

    public Stack<GameObject> spawnEnemy = new Stack<GameObject>();


    private void Start()
    {
        InitSpawnEnemise();
        StartCoroutine(SpawnEnemiesCoroutine(2.0f));
    }

    //private void Update()
    //{
    //    //SpawnEnemise();
    //}


    IEnumerator SpawnEnemiesCoroutine(float delay)
    {
        ///////////////수정하기

        while (true)
        {
            Debug.Log(Managers.Pool.monsterPool.Count);
            Debug.Log(spawnEnemy.Count);
            yield return new WaitForSeconds(delay);
            if (spawnEnemy.Count >= maximumEnemy)
            {
                if (!spawnEnemy.Peek().activeSelf)
                    PopEnemy();
                continue;
            }

            if (Managers.Pool.monsterPool.Count != 0)
            { 
                GameObject enemy = Managers.Pool.monsterPool.Peek();
                enemy.SetActive(true);
                spawnEnemy.Push(enemy);
                
            }
        }

        /////////////////////////////
    }



    public void PopEnemy()
    {
        if (spawnEnemy.Count == 0)
            return;
        spawnEnemy.Pop();

    }
    void InitSpawnEnemise()
    {
        if (spawnEnemy.Count >= maximumEnemy)
            return;
        for (int i = spawnEnemy.Count; i < maximumEnemy; i++)
        {
            GameObject enemy = Managers.Pool.monsterPool.Peek();
            enemy.SetActive(true);
            spawnEnemy.Push(enemy);
            Managers.Pool.monsterPool.Dequeue();
        }

    }
}
