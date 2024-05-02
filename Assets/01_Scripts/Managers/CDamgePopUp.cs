using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AlienProject
{
	public class CDamgePopUp : MonoBehaviour
	{
		private TextMeshProUGUI _damageTMP;
		public AnimationCurve opacityCurve;
		public AnimationCurve sizeCurve;
		private float time = 0;

		private void Awake()
		{
			_damageTMP = GetComponent<TextMeshProUGUI>();
			Destroy(gameObject, 1.5f);
		}

		private void Update()
		{
			_damageTMP.color = new Color(255, 0, 0, opacityCurve.Evaluate(time));
			_damageTMP.fontSize = sizeCurve.Evaluate(time);
			time += Time.deltaTime;
		}
	} // class CDamgePopUp
} // namespace AlienProject