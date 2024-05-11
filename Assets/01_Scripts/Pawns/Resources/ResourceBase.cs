using UnityEngine;

namespace AlienProject
{
	/// <summary>
	/// HP, SP, MP 등의 게임 로직에서 사용되는 "자원"을 표현하는 컴포넌트입니다.
	/// </summary>
	public abstract partial class ResourceBase : MonoBehaviour, IInitializable
	{
		private float _currentValue;
		private float _maxValue;

		public float Ratio => _currentValue / _maxValue;
		public float Percentage => Ratio * 100f;

		#region IInitializable Impelementation

		public void Initialize(PawnDataBase data)
		{
			_currentValue = data.healthData.current;
			_maxValue = data.healthData.max;
		}

		#endregion

		#region Unity Callbacks

		#endregion
	} // class ResourceBase

} // namespace AlienProject