using System;
using UnityEngine;

namespace AlienProject
{
    [RequireComponent(typeof(CStaminaPoint))]
    [AddComponentMenu("Alien Project/Character Component")]
    public class CCharacter : PawnBase
    {
        private CStaminaPoint _sp;

        // MARK: Inspector

        #region IDamageable Impelementation

        /// <summary>
        /// 플레이어에게 데미지를 입힙니다.<para></para>
        /// 적의 무기측에서 호출 해야합니다.
        /// </summary>
        /// <param name="damageAmount"></param>
        public override void TakeDamage(float damageAmount)
        {
            _hp.ApplyDamage(damageAmount);
            CUIManager.UIManager.SetPlayerHP(_hp.Ratio);
        }

        /// <summary>
        /// Test용 현재 적이 구현되어있지않아 테스트할수 없어서 만듬 - ssj
        /// </summary>
        [ContextMenu("TestPlayer_HIT")]
        public void Test()
        {
            TakeDamage(10);
            Debug.Log("TestPlayerHit" + _hp.Ratio);
        }

        #endregion // IDamageable Impelementation

        // MARK: Inspector

        [SerializeField] private OCharacterData _characterData;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            _sp = GetComponent<CStaminaPoint>();
        }

        protected override void Start()
        {
            base.Start();

            if (_shouldInitalizeWithPawnData)
            {
                foreach (var initializable in GetComponents<IInitializable>())
                {
                    initializable.Initialize(_characterData);
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            //other.tag
        }

        public void OnCollisionEnter(Collision other)
        {
        }

        #endregion // Unity Callbacks
    } // class CCharacter
} // namespace AlienProject