using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlienProject;
using Redcode.Pools;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace AlienProject
{
	public enum EEnemyType
	{
		Walkable,
		Fix,
		Jumpable
	}

	public enum EEnemyAttackType
	{
		Melee,
		Range,
	}

	public enum EEnemyState
	{
		Idle,
		Move,
		Patrol
	}
	[Serializable]
	public struct AttackInfo
	{
		public float Power;
		public float Bleeding;
		public float Sleeping;
	}
	
	/// <summary>
	/// 현재 적의 추가 상태 정보
	/// 상태이상 수치, 속성 수치
	/// </summary>
	[System.Serializable]
	public struct EEnemyCurrentState
	{
		public float Damage;
		public float Hp;
		public float MoveSpeed;
		public float Bleeding;
		public float Sleeping;
	}

	[System.Serializable]
	public struct MaxState
	{
		public float MaxHp;
		public float MaxMoveSpeed;
		public float MaxBleeding;
		public float MaxSleeping;
	}
	public delegate void HitDelegate();

    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("필수 기본 구조")] public string _name;
        [SerializeField] public EEnemyCurrentState _currentState;
        [SerializeField] private MaxState _maxState;
        public EEnemyType _enemType;
        public EEnemyAttackType _AttackType;
        [SerializeField] private Canvas _enemeyCanvas;


        protected Vector3 moveDir;
        [SerializeField] public Transform _playerTR;
        [HideInInspector] public NavMeshAgent _agent;
        private Renderer _renderer;
        private Color _defaultColor;
        private Vector3 _nuckBackDir;
        protected Collider _collider;
        protected Rigidbody _rigidbody;

        private List<Image> bleedingImageList;

        /// <summary>
        /// 피격 당했을때 Event 
        /// </summary>
        public event HitDelegate HitEvent;

        protected virtual void Awake()
        {
            // _playerTR = CPlayerController._cPlayerController.transform;
            //_agent = GetComponent<NavMeshAgent>();
            _renderer = GetComponent<Renderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _defaultColor = _renderer.material.color;
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _currentState.MoveSpeed;

            var bleeding = _enemeyCanvas.transform.GetChild(0).GetComponentsInChildren<Image>();
            bleedingImageList = bleeding.ToList();
            InitializeEnemey();
        }

        //Tirrger는 공격측에서한다
        /*
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Enemy Trigger From - " + other.name);
            if (other.gameObject.tag.Equals("PlayerWeapon"))
            {
                //Hit(other.gameObject.GetComponent<pla>());
                Hit(1);
            }
        }
        */

		private void SetAgentPorperty()
		{
			throw new System.NotImplementedException();
		}

        public void Hit(float damage)
        {
            _currentState.Hp -= damage;
            if (_currentState.Hp < 0)
                Die();

			CUIManager.UIManager.GeneratePopUP(1, this.transform.position);
			StartCoroutine(SetColorAnimation());

            //기본 Kinematic이라 일단 보류
            /*_nuckBackDir = -_agent.velocity;
            NuckBack(1);*/
            HitEvent?.Invoke();

            Debug.Log("<color=orange>Enemey Hit  </color>" + _currentState.Hp);
        }


        public void Hit(float damage, AttackInfo info)
        {
            _currentState.Hp -= damage;
            bool isDie = UpdateState(info);
            if (_currentState.Hp < 0 || isDie)
                Die();

            CUIManager.UIManager.GeneratePopUP(damage, this.transform.position);
            StartCoroutine(SetColorAnimation());

            HitEvent?.Invoke();

            Debug.Log("<color=orange>Enemey Hit  </color>" + _currentState.Hp);
        }

        /// <summary>
        /// 적의 상태를 업데이트 합니다.
        /// <para>Hit 시 호출</para>
        /// </summary>
        /// <param name="info"></param>
        /// <returns>적의 죽음을 Bool로 반환합니다.</returns>
        private bool UpdateState(AttackInfo info)
        {
            _currentState.Hp -= info.Power;

            _currentState.Bleeding += info.Bleeding;
            _currentState.Sleeping += info.Sleeping;

            UpdateEnemeyUI();
            //Check Max
            if (_maxState.MaxBleeding <= _currentState.Bleeding)
            {
                Debug.Log("<color=red>BLEEDING</color>");
                return true;
            }

			if (HitEvent != null)
				HitEvent();
			else
			{
				Debug.LogError("Enemy : hit Event가 없습니다. 응 사실 없어도됨.");
			}
            if (_maxState.MaxSleeping <= _currentState.Sleeping)
            {
                Debug.Log("<color=red>Sleeping</color>");
                return true;
            }

            return false;
        }

        private void UpdateEnemeyUI()
        {
            for (int i = 0; i < _currentState.Bleeding; i++)
            {
                bleedingImageList[i].enabled = true;
            }
        }

        public virtual void Die()
        {
            Destroy(this.gameObject);
        }

        public abstract void InitializeEnemey();

        public void Move()
        {
            _agent.SetDestination(_playerTR.position);
        }

		public void KnockBack(int force)
		{
			_rigidbody.AddForce(_nuckBackDir * force, ForceMode.Impulse);
		}

        public IEnumerator SetColorAnimation()
        {
            _renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _renderer.material.color = _defaultColor;
        }

		public float[] GetRandomPosition()
		{
			float[] arr = new float[2];
			arr[0] = Random.Range(0, 10);
			arr[1] = Random.Range(0, 10);
			return arr;
		}

		#region Pooling

		public void OnCreatedInPool()
		{
			Debug.Log("OnCreatedInPool");
		}

		public void OnGettingFromPool()
		{
			Debug.Log("OnGettingFromPool");
			float[] arr = GetRandomPosition();
			this.transform.position = new Vector3(arr[0], 0, arr[1]);
		}

		#endregion // Pooling

		public virtual void OnDrawGizmos()
		{
		}

	} // class EnemyBase
} // namespace AlienProject