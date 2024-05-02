//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using Redcode.Pools;
//using Random = UnityEngine.Random;

//public class Spanwer : MonoBehaviour
//{
//    private PoolManager poolManager;
//    [SerializeField] private Enemy aa;
//    private IEnumerator spawnCo;
//    private Pool<Enemy> pool;

//    private void Awake()
//    {
//        poolManager = GetComponent<PoolManager>();
//    }


//    void Start()
//    {
//        Pool.Create(aa, 5);

//        spawnCo = Spawn();


//        StartCoroutine(spawnCo);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//    }

//    private IEnumerator Spawn()
//    {
//        while (true)
//        {
//            Debug.Log("spawn");
////            var enemy = poolManager.GetFromPool<Enemy>();
//            yield return new WaitForSeconds(1f);
//        }
//    }

//    private float[] GetRandomPos()
//    {
//        float[] arr = new float[2];
//        arr[0] = Random.Range(0, 10);
//        arr[1] = Random.Range(0, 10);
//        return arr;
//    }
//}