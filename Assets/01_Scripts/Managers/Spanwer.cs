using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Redcode.Pools;
using Random = UnityEngine.Random;

namespace AlienProject
{
    public class Spanwer : MonoBehaviour
    {
        private PoolManager poolManager;
        [SerializeField] private Enemy enemy;
        private IEnumerator spawnCo;
        private Pool<Enemy> pool;

        private void Awake()
        {
            poolManager = GetComponent<PoolManager>();
        }


        void Start()
        {
            Pool.Create(enemy, 5);

            spawnCo = Spawn();
            StartCoroutine(spawnCo);
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                Debug.Log("spawn");
//            var enemy = poolManager.GetFromPool<Enemy>();
                yield return new WaitForSeconds(1f);
            }
        }

        private float[] GetRandomPos()
        {
            float[] arr = new float[2];
            arr[0] = Random.Range(0, 10);
            arr[1] = Random.Range(0, 10);
            return arr;
        }
    }
}