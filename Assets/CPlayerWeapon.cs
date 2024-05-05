using System;
using System.Collections;
using System.Collections.Generic;
using AlienProject;
using NaughtyAttributes;
using UnityEngine;

namespace AlienProject
{
    public enum StatusEffect
    {
        Bleeding,
        Sleeping
    }

//public enum 


    public class CPlayerWeapon : MonoBehaviour
    {
        [SerializeField] private float _attackPower = 1;
        [Required, SerializeField] private StatusEffect _statusEffect;
        [SerializeField] public AttackInfo _attackInfo;
        [HideInInspector] public Collider _collider;

        [HideInInspector] public bool isAttacking = false;

        private void Awake()
        {
            Init();
        }

        void Start()
        {
        }

        private void Init()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isAttacking)
                //return;
            if (other.tag.Equals("Enemy"))
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                Attack(enemy);
            }
        }

        private void Attack(EnemyBase enemy)
        {
            Debug.Log("Attack");
            enemy.Hit(_attackPower, _attackInfo);
        }
    }
}