namespace SEModAPI.API.TypeConverters
{
	using Sandbox.Common.ObjectBuilders.VRageData;
	using VRageMath;

	public struct Vector3Wrapper
	{
		private Vector3 _vector;

		public Vector3Wrapper(float x, float y, float z)
		{
			_vector.X = x;
			_vector.Y = y;
			_vector.Z = z;
		}
		public Vector3Wrapper(SerializableVector3 v)
		{
			_vector = v;
		}
		public Vector3Wrapper(Vector3 v)
		{
			_vector = v;
		}

		public float X
		{
			get { return _vector.X; }
			set { _vector.X = value; }
		}
		public float Y
		{
			get { return _vector.Y; }
			set { _vector.Y = value; }
		}
		public float Z
		{
			get { return _vector.Z; }
			set { _vector.Z = value; }
		}

		public static implicit operator Vector3Wrapper(SerializableVector3 v)
		{
			return new Vector3Wrapper(v);
		}

		public static implicit operator Vector3Wrapper(Vector3 v)
		{
			return new Vector3Wrapper(v);
		}

		public static implicit operator SerializableVector3(Vector3Wrapper v)
		{
			return new SerializableVector3(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3(Vector3Wrapper v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public override string ToString()
		{
			return _vector.ToString();
		}
	}
}
