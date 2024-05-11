// 2024-04-15 Checked by RZN
// HP, SP, MP 등의 게임 로직에서 사용되는 "자원"을 표현하는 컴포넌트

using UnityEngine;
using UnityEngine.Events;

namespace AlienProject
{
	[AddComponentMenu("Alien Project/Resources/Resource Component")]
	public class CResource : MonoBehaviour
	{
		public enum EResourceType
		{
			None
		}

		[Header("리소스 이벤트")]
		[SerializeField]
		private UnityEvent _onValueChanged;

		[SerializeField]
		private UnityEvent _onValueZero;

		[SerializeField]
		private UnityEvent _onValueMax;

		[SerializeField]
		private EResourceType _resourceType;

		private float _currentValue;
		private float _maxValue;

		public EResourceType Type => _resourceType;
		public float Ratio => _currentValue / _maxValue;
		public float Percentage => Ratio * 100f;

		private void InitializeWithValue(float initialValue) { _currentValue = initialValue; }
		private void InitializeWithMaxValue() { _currentValue = _maxValue; }

		protected virtual void Update()
		{
			if (_currentValue <= 0f)
			{
				_onValueZero.Invoke();
			}
			else if (_currentValue >= _maxValue)
			{
				_onValueMax.Invoke();
			}
			else
			{
				_onValueChanged.Invoke();
			}
		}
	}

} // namespace AlienProject