using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace AlienProject
{
    using UnityEngine;

    public class CEnemyUI : MonoBehaviour
    {
        private Canvas _canvas;

        private List<Image> sleepingImageList;
        private List<Image> bleedingImageList;

        [SerializeField] private Image _hpImage;
        [SerializeField] private GameObject sleepingGroup;
        [SerializeField] private GameObject bleedingGroup;

        private Image sleepingImage;
        private Image bleedingImage;

        private void Awake()
        {
            _canvas = transform.GetComponentInChildren<Canvas>();
        }

        public void Init(int maxBleed, int maxSleep)
        {
            for (int i = 0; i < maxBleed - 1; i++)
            {
                Instantiate(bleedingGroup.transform.GetChild(0), bleedingGroup.transform);
            }

            for (int i = 0; i < maxSleep - 1; i++)
            {
                Instantiate(sleepingGroup.transform.GetChild(0), sleepingGroup.transform);
            }

            sleepingImageList = sleepingGroup.GetComponentsInChildren<Image>().ToList();
            bleedingImageList = bleedingGroup.GetComponentsInChildren<Image>().ToList();
        }

        /// <summary>
        /// 적의 축적된 상태이상을 UI로 표현합니다.
        /// 하나씩 축적된다는 가정하에
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="effectInfo"></param>
        public void UpdateUI(float hp, EffectInfo effectInfo)
        {
            bleedingImageList[effectInfo.bleeding - 1].enabled = true;
            sleepingImageList[effectInfo.bleeding - 1].enabled = true;
            _hpImage.fillAmount = hp * 0.01f;
            
        }

        public void ResetUI(Effect effect)
        {
            switch (effect)
            {
                case Effect.Bleeding:
                    foreach (Image VARIABLE in bleedingImageList)
                    {
                        VARIABLE.enabled = false;
                    }

                    break;

                case Effect.Sleeping:
                    foreach (Image VARIABLE in sleepingImageList)
                    {
                        VARIABLE.enabled = false;
                    }

                    break;
                default:
                    Debug.LogError("정의되지않은 상태이상");
                    break;
            }
        }

        public void UpdateUI(EffectInfo effectInfo)
        {
        }

        public void UpdateUI(float hp)
        {
        }
    }
}