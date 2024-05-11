using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienProject
{
    [AddComponentMenu("Alien Project/Enemy/Default Enemy Component")]
    public class CEnemyDefault : EnemyBase
    {
        private CEnemyFOV _fieldOfView;
        private Transform _target = null;
        private EEnemyState _state = EEnemyState.Idle;

        public override void Awake()
        {
            base.Awake();

            _fieldOfView = GetComponentInChildren<CEnemyFOV>();
            _fieldOfView.FindTargetEvent += StartFollow;
            _fieldOfView.TargetEnmptyEvent += StopFollow;
        }

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
            }
        }

        private void StartFollow(Transform target)
        {
            Debug.Log("StartFollow");

            if (this._target != target)
            {
                _agent.isStopped = false;
                _state = EEnemyState.Move;
                this._target = target;
            }
        }

        private void StopFollow(Transform tr)
        {
            Debug.Log("StopFollow");

            _agent.isStopped = true;
            _state = EEnemyState.Idle;
            this._target = null;
        }

        public override void InitializeEnemey()
        {
        }
    } // class E_Default
} // namespace AlienProject