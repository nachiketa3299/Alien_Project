using System.Collections;
using Redcode.Pools;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
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

	public delegate void HitDelegate();

	[DisallowMultipleComponent]
	[RequireComponent(typeof(NavMeshAgent))]
	public abstract class EnemyBase : PawnBase
	{
		[Header("필수 기본 구조")]

		public string _name;
		public float damage;
		public float hp;
		public float moveSpeed;
		public EEnemyType _enemType;
		public EEnemyAttackType _AttackType;

		protected Vector3 moveDir;
		public Transform _playerTR;

		[HideInInspector] public NavMeshAgent _agent;
		private Renderer _renderer;
		private Color _defaultColor;
		private Vector3 _nuckBackDir;
		protected Collider _collider;
		protected Rigidbody _rigidbody;

		/// <summary>
		/// 피격 당했을때 Event 
		/// </summary>
		public event HitDelegate HitEvent;

		protected override void Awake()
		{
			base.Awake();
			// _playerTR = CPlayerController._cPlayerController.transform;
			//_agent = GetComponent<NavMeshAgent>();

			_renderer = GetComponent<Renderer>();
			_rigidbody = GetComponent<Rigidbody>();
			_defaultColor = _renderer.material.color;
			_agent = GetComponent<NavMeshAgent>();
			_agent.speed = moveSpeed;

			InitializeEnemey();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag.Equals("PlayerWeapon"))
			{
				//Hit(other.gameObject.GetComponent<pla>());
				Hit(1);
			}
		}

		private void SetAgentPorperty()
		{
			throw new System.NotImplementedException();
		}

		public void Hit(float damage)
		{
			hp -= damage;
			if (hp < 0)
				Die();

			CUIManager.UIManager.GeneratePopUP(1, this.transform.position);
			StartCoroutine(SetColorAnimation());

			//기본 Kinematic이라 일단 보류
			/*_nuckBackDir = -_agent.velocity;
			NuckBack(1);*/

			if (HitEvent != null)
				HitEvent();
			else
			{
				Debug.LogError("Enemy : hit Event가 없습니다. 응 사실 없어도됨.");
			}

			Debug.Log("<color=orange>Enemey Hit  </color>" + hp);
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