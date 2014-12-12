using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using VRageMath;

using SEModAPI.Support;
using SEModAPI.API.Definitions;
using SEModAPI.API.Definitions.CubeBlocks;

namespace SEModAPI.API
{
	public struct Vector3Wrapper
	{
		private Vector3 vector;

		public Vector3Wrapper(float x, float y, float z)
		{
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
		}
		public Vector3Wrapper(SerializableVector3 v)
		{
			vector = v;
		}
		public Vector3Wrapper(Vector3 v)
		{
			vector = v;
		}

		public float X
		{
			get { return vector.X; }
			set { vector.X = value; }
		}
		public float Y
		{
			get { return vector.Y; }
			set { vector.Y = value; }
		}
		public float Z
		{
			get { return vector.Z; }
			set { vector.Z = value; }
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
			return vector.ToString();
		}
	}

	public struct Vector3DWrapper
	{
		private Vector3D vector;

		public Vector3DWrapper(double x, double y, double z)
		{
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
		}
		public Vector3DWrapper(SerializableVector3D v)
		{
			vector = v;
		}
		public Vector3DWrapper(Vector3D v)
		{
			vector = v;
		}

		public double X
		{
			get { return vector.X; }
			set { vector.X = value; }
		}
		public double Y
		{
			get { return vector.Y; }
			set { vector.Y = value; }
		}
		public double Z
		{
			get { return vector.Z; }
			set { vector.Z = value; }
		}

		public static implicit operator Vector3DWrapper(SerializableVector3D v)
		{
			return new Vector3DWrapper(v);
		}

		public static implicit operator Vector3DWrapper(Vector3D v)
		{
			return new Vector3DWrapper(v);
		}

		public static implicit operator SerializableVector3D(Vector3DWrapper v)
		{
			return new SerializableVector3D(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3D(Vector3DWrapper v)
		{
			return new Vector3D(v.X, v.Y, v.Z);
		}

		public override string ToString()
		{
			return vector.ToString();
		}
	}

	public class Vector3DTypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3DWrapper((double)propertyValues["X"], (double)propertyValues["Y"], (double)propertyValues["Z"]);
		}
	}


	public class Vector3TypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3Wrapper((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
		}
	}

	public class Vector3ITypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3I((int)propertyValues["X"], (int)propertyValues["Y"], (int)propertyValues["Z"]);
		}
	}
}
