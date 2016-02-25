using VRage.Game;

namespace SEModAPI.API.Definitions
{
	using System;
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	public class ConfigurationDefinition
	{
		private MyObjectBuilder_Configuration _baseDefinition;

		public bool Changed
		{
			get;
			private set;
		}

		/// <exception cref="ArgumentOutOfRangeException">
		/// <see cref="LargeCubeSize"/> cannot be less than 0f.
		/// </exception>
		public float LargeCubeSize
		{
			get { return _baseDefinition.CubeSizes.Large; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				if (Math.Abs( _baseDefinition.CubeSizes.Large - value ) < Single.Epsilon)
					return;
				_baseDefinition.CubeSizes.Large = value;
				Changed = true;
			}
		}

		/*
		public float MediumCubeSize
		{
			get { return m_baseDefinition.CubeSizes.Medium; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (m_baseDefinition.CubeSizes.Medium == value) return;
				m_baseDefinition.CubeSizes.Medium = value;
				Changed = true;
			}
		}
		*/

		/// <exception cref="ArgumentOutOfRangeException">
		/// <see cref="SmallCubeSize"/> cannot be less than 0f.
		/// </exception>
		public float SmallCubeSize
		{
			get { return _baseDefinition.CubeSizes.Small; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				if (Math.Abs( _baseDefinition.CubeSizes.Small - value ) < Single.Epsilon)
					return;
				_baseDefinition.CubeSizes.Small = value;
				Changed = true;
			}
		}

		public string SmallDynamic
		{
			get { return _baseDefinition.BaseBlockPrefabs.SmallDynamic; }
			set
			{
				if (_baseDefinition.BaseBlockPrefabs.SmallDynamic == value)
					return;
				_baseDefinition.BaseBlockPrefabs.SmallDynamic = value;
				Changed = true;
			}
		}

		public string SmallStatic
		{
			get { return _baseDefinition.BaseBlockPrefabs.SmallStatic; }
			set
			{
				if (_baseDefinition.BaseBlockPrefabs.SmallStatic == value)
					return;
				_baseDefinition.BaseBlockPrefabs.SmallStatic = value;
				Changed = true;
			}
		}

		/*
		public string MediumDynamic
		{
			get { return m_baseDefinition.BaseBlockPrefabs.MediumDynamic; }
			set
			{
				if (m_baseDefinition.BaseBlockPrefabs.MediumDynamic == value) return;
				m_baseDefinition.BaseBlockPrefabs.MediumDynamic = value;
				Changed = true;
			}
		}
		*/

		/*
		public string MediumStatic
		{
			get { return m_baseDefinition.BaseBlockPrefabs.MediumStatic; }
			set
			{
				if (m_baseDefinition.BaseBlockPrefabs.MediumStatic == value) return;
				m_baseDefinition.BaseBlockPrefabs.MediumStatic = value;
				Changed = true;
			}
		}
		*/

		public string LargeDynamic
		{
			get { return _baseDefinition.BaseBlockPrefabs.LargeDynamic; }
			set
			{
				if (_baseDefinition.BaseBlockPrefabs.LargeDynamic == value)
					return;
				_baseDefinition.BaseBlockPrefabs.LargeDynamic = value;
				Changed = true;
			}
		}

		public string LargeStatic
		{
			get { return _baseDefinition.BaseBlockPrefabs.LargeStatic; }
			set
			{
				if (_baseDefinition.BaseBlockPrefabs.LargeStatic == value)
					return;
				_baseDefinition.BaseBlockPrefabs.LargeStatic = value;
				Changed = true;
			}
		}
	}
}
