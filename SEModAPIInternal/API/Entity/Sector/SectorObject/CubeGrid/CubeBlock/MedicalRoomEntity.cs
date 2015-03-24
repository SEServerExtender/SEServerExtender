namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class MedicalRoomEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string MedicalRoomNamespace = "";
		public static string MedicalRoomClass = "Sandbox.Game.Entities.Cube.MyMedicalRoom";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public MedicalRoomEntity( CubeGridEntity parent, MyObjectBuilder_MedicalRoom definition )
			: base( parent, definition )
		{
		}

		public MedicalRoomEntity( CubeGridEntity parent, MyObjectBuilder_MedicalRoom definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Medical Room" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_MedicalRoom ObjectBuilder
		{
			get
			{
				MyObjectBuilder_MedicalRoom block = (MyObjectBuilder_MedicalRoom)base.ObjectBuilder;

				return block;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Obsolete]
		[Category( "Medical Room" )]
		public ulong SteamUserId
		{
			get { return ObjectBuilder.SteamUserId; }
		}

		#endregion "Properties"
	}
}