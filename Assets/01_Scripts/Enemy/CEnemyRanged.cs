using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Serialization;


namespace AlienProject
{
    [AddComponentMenu("Alien Project/Enemy/Ranged Enemy Component")]
    public class CEnemyRanged : EnemyBase
    {
        private CEnemyFOV _fieldOfView;
        private Transform _followTr = null;
        [SerializeField] private GameObject _projectile;
        [SerializeField] private float _shootPower = 1f;
        [SerializeField] private Transform _shootPosition;
        [SerializeField] private float _reloadDelay = 1f;

        [Header("For Debug")] [SerializeField] private EEnemyState _state = EEnemyState.Idle;

        private Transform _target;
        private Coroutine _shootCoroutine;

        public override void Awake()
        {
            base.Awake();
            _fieldOfView = GetComponentInChildren<CEnemyFOV>();
            _fieldOfView.FindTargetEvent += StartShoot;
            _fieldOfView.TargetEnmptyEvent += StopFollow;
        }

        private void FixedUpdate()
        {
            if (_followTr is not null)
                _agent.SetDestination(_followTr.position);
        }

        private Vector3 testShootDir;

        private void Update()
        {
            switch (_state)
            {
                case EEnemyState.Idle:
                {
                    _agent.isStopped = true;
                    break;
                }
                case EEnemyState.Move:
                {
                    _agent.SetDestination(_target.position);
                    break;
                }
                case EEnemyState.Patrol:
                {
                    break;
                }
                /*case EEnemyState.Shoot:
                {
                    Shoot(_reloadDelay, target);
                    break;
                }*/
            }
        }

        private void StartShoot(Transform target)
        {
            Debug.Log("ShotToTarget");
            if (this._target != target)
            {
                _agent.isStopped = false;
                _state = EEnemyState.Move;
                this._target = target;
                _shootCoroutine = StartCoroutine(Shoot(_reloadDelay, target));
            }
        }

        private IEnumerator Shoot(float reloadDelay, Transform target)
        {
            while (true)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                dir.y = 3;
                testShootDir = dir;

                var projectile = Instantiate(_projectile, _shootPosition.position, quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                rb.AddForce(dir * _shootPower, ForceMode.Impulse);
                yield return new WaitForSeconds(_reloadDelay);
            }
        }

        private void StopFollow(Transform tr)
        {
            Debug.Log("StopFollow");
            this._target = null;
            _state = EEnemyState.Idle;
            if (_shootCoroutine != null)
                StopCoroutine(_shootCoroutine);
        }

        public override void InitializeEnemey()
        {
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, testShootDir);

            Vector3 test = new Vector3(0, 30, 1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, test);
        }
    }
}