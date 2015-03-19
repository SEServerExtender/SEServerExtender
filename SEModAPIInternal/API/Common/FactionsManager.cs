using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class Faction
	{
		#region "Attributes"

		private MyObjectBuilder_Faction m_faction;
		private Dictionary<long, FactionMember> m_members;
		private Dictionary<long, FactionMember> m_joinRequests;
		private long m_memberToModify;

		public static string FactionNamespace = "";
		public static string FactionClass = "=9DUcsoJb37UyrFZNPPoFzkiQCma=";

		public static string FactionGetMembersMethod = "get_Members";
		public static string FactionGetJoinRequestsMethod = "get_JoinRequests";
		public static string FactionAddApplicantMethod = "AddJoinRequest";
		public static string FactionRemoveApplicantMethod = "CancelJoinRequest";
		public static string FactionAcceptApplicantMethod = "AcceptJoin";
		public static string FactionRemoveMemberMethod = "KickMember";

		public static string FactionMembersDictionaryField = "=hrd2r4yATC2NCJnhqOpJBW6EOU=";
		public static string FactionJoinRequestsDictionaryField = "=1xaBHU23sqENJAlqfZYI7iPsvP=";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public Faction( MyObjectBuilder_Faction faction )
		{
			m_faction = faction;

			m_members = new Dictionary<long, FactionMember>( );
			foreach ( MyObjectBuilder_FactionMember member in m_faction.Members )
			{
				m_members.Add( member.PlayerId, new FactionMember( this, member ) );
			}

			m_joinRequests = new Dictionary<long, FactionMember>( );
			foreach ( MyObjectBuilder_FactionMember member in m_faction.JoinRequests )
			{
				m_joinRequests.Add( member.PlayerId, new FactionMember( this, member ) );
			}
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Object BackingObject
		{
			get
			{
				return FactionsManager.Instance.InternalGetFactionById( m_faction.FactionId );
			}
		}

		public MyObjectBuilder_Faction BaseEntity
		{
			get { return m_faction; }
		}

		[Browsable( false )]
		public List<FactionMember> Members
		{
			get
			{
				RefreshFactionMembers( );

				List<FactionMember> memberList = new List<FactionMember>( m_members.Values );
				return memberList;
			}
		}

		[Browsable( false )]
		public List<FactionMember> JoinRequests
		{
			get
			{
				RefreshFactionJoinRequests( );

				List<FactionMember> memberList = new List<FactionMember>( m_joinRequests.Values );
				return memberList;
			}
		}

		public long Id
		{
			get { return m_faction.FactionId; }
		}

		public string Name
		{
			get { return m_faction.Name; }
		}

		public string Description
		{
			get { return m_faction.Description; }
		}

		public string PrivateInfo
		{
			get { return m_faction.PrivateInfo; }
		}

		public string Tag
		{
			get { return m_faction.Tag; }
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( FactionNamespace, FactionClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for Faction" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, FactionGetMembersMethod );
				result &= BaseObject.HasMethod( type1, FactionGetJoinRequestsMethod );
				result &= BaseObject.HasMethod( type1, FactionAddApplicantMethod );
				result &= BaseObject.HasMethod( type1, FactionRemoveApplicantMethod );
				result &= BaseObject.HasMethod( type1, FactionAcceptApplicantMethod );
				result &= BaseObject.HasMethod( type1, FactionRemoveMemberMethod );
				result &= BaseObject.HasField( type1, FactionMembersDictionaryField );
				result &= BaseObject.HasField( type1, FactionJoinRequestsDictionaryField );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void RemoveMember( long memberId )
		{
			try
			{
				if ( !m_members.ContainsKey( memberId ) )
					return;

				m_memberToModify = memberId;

				MyObjectBuilder_FactionMember memberToRemove = new MyObjectBuilder_FactionMember( );
				foreach ( MyObjectBuilder_FactionMember member in m_faction.Members )
				{
					if ( member.PlayerId == m_memberToModify )
					{
						memberToRemove = member;
						break;
					}
				}
				m_faction.Members.Remove( memberToRemove );

				Action action = InternalRemoveMember;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected void RefreshFactionMembers( )
		{
			List<MyObjectBuilder_FactionMember> memberList = BaseEntity.Members;

			//Cleanup missing members
			List<FactionMember> membersToRemove = new List<FactionMember>( );
			foreach ( FactionMember member in m_members.Values )
			{
				if ( memberList.Contains( member.BaseEntity ) )
					continue;

				membersToRemove.Add( member );
			}
			foreach ( FactionMember member in membersToRemove )
			{
				m_members.Remove( member.PlayerId );
			}

			//Add new members
			foreach ( MyObjectBuilder_FactionMember member in memberList )
			{
				if ( m_members.ContainsKey( member.PlayerId ) )
					continue;

				FactionMember newMember = new FactionMember( this, member );
				m_members.Add( newMember.PlayerId, newMember );
			}
		}

		protected void RefreshFactionJoinRequests( )
		{
			List<MyObjectBuilder_FactionMember> memberList = BaseEntity.JoinRequests;

			//Cleanup missing members
			List<FactionMember> membersToRemove = new List<FactionMember>( );
			foreach ( FactionMember member in m_joinRequests.Values )
			{
				if ( memberList.Contains( member.BaseEntity ) )
					continue;

				membersToRemove.Add( member );
			}
			foreach ( FactionMember member in membersToRemove )
			{
				m_joinRequests.Remove( member.PlayerId );
			}

			//Add new members
			foreach ( MyObjectBuilder_FactionMember member in memberList )
			{
				if ( m_joinRequests.ContainsKey( member.PlayerId ) )
					continue;

				FactionMember newMember = new FactionMember( this, member );
				m_joinRequests.Add( newMember.PlayerId, newMember );
			}
		}

		protected void InternalRemoveMember( )
		{
			if ( m_memberToModify == 0 )
				return;

			FactionsManager.Instance.RemoveMember( Id, m_memberToModify );

			BaseObject.InvokeEntityMethod( BackingObject, FactionRemoveMemberMethod, new object[ ] { m_memberToModify } );

			m_memberToModify = 0;
		}

		#endregion "Methods"
	}

	public class FactionMember
	{
		#region "Attributes"

		private MyObjectBuilder_FactionMember m_member;
		private Faction m_parent;

		public static string FactionMemberNamespace = "";
		//public static string FactionMemberClass = "32F8947D11EDAF4D079FD54C2397A951";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public FactionMember( Faction parent, MyObjectBuilder_FactionMember definition )
		{
			m_parent = parent;
			m_member = definition;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Faction Parent
		{
			get { return m_parent; }
		}

		public MyObjectBuilder_FactionMember BaseEntity
		{
			get { return m_member; }
		}

		public string Name
		{
			get
			{
				return PlayerMap.Instance.GetPlayerItemFromPlayerId( PlayerId ).Name;
			}
		}

		public ulong SteamId
		{
			get
			{
				return PlayerMap.Instance.GetPlayerItemFromPlayerId( PlayerId ).SteamId;
			}
		}

		public long PlayerId
		{
			get { return m_member.PlayerId; }
		}

		public bool IsFounder
		{
			get { return m_member.IsFounder; }
		}

		public bool IsLeader
		{
			get { return m_member.IsLeader; }
		}

		public bool IsDead
		{
			get
			{
				return PlayerMap.Instance.GetPlayerItemFromPlayerId( PlayerId ).IsDead;
			}
		}

		#endregion "Properties"
	}

	public class FactionsManager
	{
		#region "Attributes"

		private static FactionsManager m_instance;
		private MyObjectBuilder_FactionCollection m_factionCollection;
		private Dictionary<long, Faction> m_factions;

		protected long m_factionToModify;
		protected long m_memberToModify;

		public static string FactionManagerNamespace = "";
		public static string FactionManagerClass = "=zCz9UXvRmT148ybvcyvkfMadCL=";

		public static string FactionManagerGetFactionCollectionMethod = "GetObjectBuilder";
		public static string FactionManagerGetFactionByIdMethod = "TryGetFactionById";
		public static string FactionManagerRemoveFactionByIdMethod = "KickPlayerFromFaction";

		//////////////////////////////////////////////////////

		public static string FactionNetManagerNamespace = "";
		public static string FactionNetManagerClass = "";

		public static string FactionNetManagerRemoveFactionMethod = "RemoveFaction";
		public static string FactionNetManagerRemoveMemberMethod = "MemberLeaves";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected FactionsManager( )
		{
			m_instance = this;
			m_factions = new Dictionary<long, Faction>( );

			Console.WriteLine( "Finished loading FactionsManager" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static FactionsManager Instance
		{
			get
			{
				if ( m_instance == null )
					m_instance = new FactionsManager( );

				return m_instance;
			}
		}

		public Object BackingObject
		{
			get
			{
				return WorldManager.Instance.InternalGetFactionManager( );
			}
		}

		public List<Faction> Factions
		{
			get
			{
				RefreshFactions( );

				List<Faction> factionList = new List<Faction>( m_factions.Values );
				return factionList;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( FactionManagerNamespace, FactionManagerClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for FactionsManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, FactionManagerGetFactionCollectionMethod );
				result &= BaseObject.HasMethod( type1, FactionManagerGetFactionByIdMethod );
				result &= BaseObject.HasMethod( type1, FactionManagerRemoveFactionByIdMethod );

				result &= BaseObject.HasMethod( type1, FactionNetManagerRemoveFactionMethod );
				result &= BaseObject.HasMethod( type1, FactionNetManagerRemoveMemberMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public MyObjectBuilder_FactionCollection GetSubTypeEntity( )
		{
			m_factionCollection = (MyObjectBuilder_FactionCollection)BaseObject.InvokeEntityMethod( BackingObject, FactionManagerGetFactionCollectionMethod );

			return m_factionCollection;
		}

		protected void RefreshFactions( )
		{
			List<MyObjectBuilder_Faction> factionList = GetSubTypeEntity( ).Factions;

			//Cleanup missing factions
			List<Faction> factionsToRemove = new List<Faction>( );
			foreach ( Faction faction in m_factions.Values )
			{
				bool foundMatch = false;
				foreach ( MyObjectBuilder_Faction entry in factionList )
				{
					if ( entry.FactionId == faction.Id )
					{
						foundMatch = true;
						break;
					}
				}
				if ( foundMatch )
					continue;

				factionsToRemove.Add( faction );
			}
			foreach ( Faction faction in factionsToRemove )
			{
				m_factions.Remove( faction.Id );
			}

			//Add new factions
			foreach ( MyObjectBuilder_Faction faction in factionList )
			{
				if ( m_factions.ContainsKey( faction.FactionId ) )
					continue;

				Faction newFaction = new Faction( faction );
				m_factions.Add( newFaction.Id, newFaction );
			}
		}

		public void RemoveFaction( long id )
		{
			m_factionToModify = id;
			m_factions.Remove( id );

			Action action = InternalRemoveFaction;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		internal void RemoveMember( long factionId, long memberId )
		{
			m_factionToModify = factionId;
			m_memberToModify = memberId;

			Action action = InternalRemoveMember;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#region "Internal"

		internal Object InternalGetFactionById( long id )
		{
			Object internalFaction = BaseObject.InvokeEntityMethod( BackingObject, FactionManagerGetFactionByIdMethod, new object[ ] { id } );

			return internalFaction;
		}

		protected void InternalRemoveFaction( )
		{
			if ( m_factionToModify == 0 )
				return;

			BaseObject.InvokeEntityMethod( BackingObject, FactionNetManagerRemoveFactionMethod, new object[ ] { m_factionToModify } );

			BaseObject.InvokeEntityMethod( BackingObject, FactionManagerRemoveFactionByIdMethod, new object[ ] { m_factionToModify } );

			m_factionToModify = 0;
		}

		protected void InternalRemoveMember( )
		{
			if ( m_factionToModify == 0 )
				return;
			if ( m_memberToModify == 0 )
				return;

			BaseObject.InvokeEntityMethod( BackingObject, FactionNetManagerRemoveMemberMethod, new object[ ] { m_factionToModify, m_memberToModify } );

			m_factionToModify = 0;
			m_memberToModify = 0;
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}