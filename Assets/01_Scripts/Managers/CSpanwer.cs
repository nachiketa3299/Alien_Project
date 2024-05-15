using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AlienProject
{
    [Serializable]
    public struct SpawnInfo
    {
        [SerializeField] public GameObject spawnEnemy;
        [SerializeField] public Transform spawnParent;
        [SerializeField] public Transform spawnCenter;
        [SerializeField] public float spawnRadius;
    }

    /// <summary>
    /// list
    /// </summary>
    public class CSpanwer : MonoBehaviour
    {
        //TODO Enemy로 변경필요
        [Header("Spawner")] [SerializeField]
        public SerializedDictionary<string, SpawnInfo> _spawnDic = new SerializedDictionary<string, SpawnInfo>();

        private IEnumerator spawnCo;

        private void Start()
        {
            //Test
            //1초에 10개씩 생성 -> 5번 반복
            Spawn("test2", 3, 5, 3);
        }

        /// <summary>
        /// <see cref="num"/> 만큼 한번 생성합니다.
        /// </summary>
        /// <param name="num"></param>
        public void Spawn(string key, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Debug.Log("spawn");
                var spawnObject = Instantiate(_spawnDic[key].spawnEnemy.gameObject,
                    GetRandomPosition(_spawnDic[key].spawnCenter.position, _spawnDic[key].spawnRadius),
                    Quaternion.identity, _spawnDic[key].spawnParent);
            }
        }

        /// <summary>
        /// <see cref="num"/>개씩 <see cref="wait"/>에 한번씩 <see cref="repeatTime"/>번 생성합니다.
        /// </summary>
        /// <param name="isCo"></param>
        /// <param name="num"></param>
        /// <param name="repeatTime"></param>
        /// <param name="wait"></param>
        public void Spawn(string key, int num, int repeatTime, float wait)
        {
            if (spawnCo == null)
                spawnCo = SpawnProcess(key, num, repeatTime, wait);
            StartCoroutine(spawnCo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnProcess(string key, int num, int repeatTime, float waitTime)
        {
            for (int j = 0; j < repeatTime; j++) //반복
            {
                for (int i = 0; i < num; i++) //n개 오브젝트 생성
                {
                    Debug.Log("spawn");
                    var spawnObject = Instantiate(_spawnDic[key].spawnEnemy.gameObject,
                        GetRandomPosition(_spawnDic[key].spawnCenter.position, _spawnDic[key].spawnRadius),
                        Quaternion.identity, _spawnDic[key].spawnParent);
                }

                yield return new WaitForSeconds(waitTime);
            }
        }


        // 특정 포인트를 중심으로 반지름 n에 거리에 있는 랜덤 위치를 반환하는 메소드
        public Vector3 GetRandomPosition(Vector3 center, float radius)
        {
            // 랜덤한 각도를 생성합니다.
            float angle = Random.Range(0f, Mathf.PI * 2f);
            // 반지름에 해당하는 랜덤한 거리를 생성합니다.
            float distance = Random.Range(0f, radius);

            // 랜덤한 위치를 계산합니다.
            float x = center.x + Mathf.Cos(angle) * distance;
            float z = center.z + Mathf.Sin(angle) * distance;
            float y = center.y; // 필요에 따라 Y 좌표를 수정할 수 있습니다.

            // 생성된 랜덤 위치를 반환합니다.
            return new Vector3(x, y, z);
        }
    }
}