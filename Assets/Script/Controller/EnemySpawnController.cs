using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    int maximumEnemy = 9;

    float spawnDelay = 8.0f;


    Stack<GameObject> enemyStack = new Stack<GameObject>();


    public void Start()
    {
        InitSpawnEnemies();
        StartCoroutine(SpawnEnemiesCoroutine(spawnDelay));
    }


    IEnumerator SpawnEnemiesCoroutine(float delay)
    {

        while (true)
        {
            while (Managers.Pool.monsterPool.Count != 0) // ��ȯ �� �������� �������� ť�� ��������
            {
                GameObject enemy = Managers.Pool.monsterPool.Peek();
                enemyStack.Push(enemy); // ������� ���ÿ� �־��
                Managers.Pool.monsterPool.Dequeue();
            }

            yield return new WaitForSeconds(delay);

            while(enemyStack.Count!=0) // ������� �־��� ������ ���� ������ �� 
            {
                enemyStack.Pop().SetActive(true);
            }
        }

    }

    void InitSpawnEnemies() // ������ ����ŭ ���� ��ȯ
    {
        for (int i = 0; i < maximumEnemy; i++)
        {
            GameObject enemy = Managers.Pool.monsterPool.Peek();
            enemy.SetActive(true);
            Managers.Pool.monsterPool.Dequeue();
        }

    }
}
