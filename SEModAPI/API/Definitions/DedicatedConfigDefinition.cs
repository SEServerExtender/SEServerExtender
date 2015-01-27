using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

using Sandbox.Common.ObjectBuilders;
using System.Linq;

namespace SEModAPI.API.Definitions
{
	[DataContract]
	public class DedicatedConfigDefinition
	{
		#region "Attributes"

		private MyConfigDedicatedData m_definition;

		#endregion

		#region "Constructors and Initializers"

		public DedicatedConfigDefinition( MyConfigDedicatedData definition )
		{
			m_definition = definition;
		}

		#endregion

		#region "Properties"

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the server's name" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the server's name
		/// </summary>
		public string ServerName
		{
			get { return m_definition.ServerName; }
			set
			{
				if ( m_definition.ServerName == value ) return;
				m_definition.ServerName = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the server's port" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the server's port
		/// </summary>
		public int ServerPort
		{
			get { return m_definition.ServerPort; }
			set
			{
				if ( m_definition.ServerPort == value ) return;
				m_definition.ServerPort = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the game mode" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the game mode
		/// </summary>
		public MyGameModeEnum GameMode
		{
			get { return m_definition.SessionSettings.GameMode; }
			set
			{
				if ( m_definition.SessionSettings.GameMode == value ) return;
				m_definition.SessionSettings.GameMode = value;
				return;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the grinder speed multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the inventory size multiplier
		/// </summary>
		public float GrinderSpeedMultiplier
		{
			get { return m_definition.SessionSettings.GrinderSpeedMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.GrinderSpeedMultiplier == value ) return;
				m_definition.SessionSettings.GrinderSpeedMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the welder speed multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the welder speed multiplier
		/// </summary>
		public float WelderSpeedMultiplier
		{
			get { return m_definition.SessionSettings.WelderSpeedMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.WelderSpeedMultiplier == value ) return;
				m_definition.SessionSettings.WelderSpeedMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the inventory size multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the inventory size multiplier
		/// </summary>
		public float InventorySizeMultiplier
		{
			get { return m_definition.SessionSettings.InventorySizeMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.InventorySizeMultiplier == value ) return;
				m_definition.SessionSettings.InventorySizeMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the assembler speed multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the assembler speed multiplier
		/// </summary>
		public float AssemblerSpeedMultiplier
		{
			get { return m_definition.SessionSettings.AssemblerSpeedMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.AssemblerSpeedMultiplier == value ) return;
				m_definition.SessionSettings.AssemblerSpeedMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the assembler efficiency multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the assembler efficiency multiplier
		/// </summary>
		public float AssemblerEfficiencyMultiplier
		{
			get { return m_definition.SessionSettings.AssemblerEfficiencyMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.AssemblerEfficiencyMultiplier == value ) return;
				m_definition.SessionSettings.AssemblerEfficiencyMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the refinery speed multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the refinery speed multiplier
		/// </summary>
		public float RefinerySpeedMultiplier
		{
			get { return m_definition.SessionSettings.RefinerySpeedMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.RefinerySpeedMultiplier == value ) return;
				m_definition.SessionSettings.RefinerySpeedMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the hacking speed multiplier" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the hacking speed multiplier
		/// </summary>
		public float HackSpeedMultiplier
		{
			get { return m_definition.SessionSettings.HackSpeedMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.HackSpeedMultiplier == value ) return;
				m_definition.SessionSettings.HackSpeedMultiplier = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the online mode" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the online mode
		/// </summary>
		public MyOnlineModeEnum OnlineMode
		{
			get { return m_definition.SessionSettings.OnlineMode; }
			set
			{
				if ( m_definition.SessionSettings.OnlineMode == value ) return;
				m_definition.SessionSettings.OnlineMode = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the maximum number of players" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the maximum number of players
		/// </summary>
		public short MaxPlayers
		{
			get { return m_definition.SessionSettings.MaxPlayers; }
			set
			{
				if ( m_definition.SessionSettings.MaxPlayers == value ) return;
				m_definition.SessionSettings.MaxPlayers = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the maximum number of floating object" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set the maximum number of floating object
		/// </summary>
		public short MaxFloatingObject
		{
			get { return m_definition.SessionSettings.MaxFloatingObjects; }
			set
			{
				if ( m_definition.SessionSettings.MaxFloatingObjects == value ) return;
				m_definition.SessionSettings.MaxFloatingObjects = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the environment hostility" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the environment hostility
		/// </summary>
		public MyEnvironmentHostilityEnum EnvironmentHostility
		{
			get { return m_definition.SessionSettings.EnvironmentHostility; }
			set
			{
				if ( m_definition.SessionSettings.EnvironmentHostility == value ) return;
				m_definition.SessionSettings.EnvironmentHostility = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player's health auto heal" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether the player's health auto heal
		/// </summary>
		public bool AutoHealing
		{
			get { return m_definition.SessionSettings.AutoHealing; }
			set
			{
				if ( m_definition.SessionSettings.AutoHealing == value ) return;
				m_definition.SessionSettings.AutoHealing = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player can copy/paste ships" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether the player can copy/paste ships
		/// </summary>
		public bool EnableCopyPaste
		{
			get { return m_definition.SessionSettings.EnableCopyPaste; }
			set
			{
				if ( m_definition.SessionSettings.EnableCopyPaste == value ) return;
				m_definition.SessionSettings.EnableCopyPaste = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server will save regularly the sector" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Determine whether the server will save regularly the sector
		/// </summary>
		public bool AutoSave
		{
			get { return m_definition.SessionSettings.AutoSave; }
			set
			{
				if ( m_definition.SessionSettings.AutoSave == value ) return;
				m_definition.SessionSettings.AutoSave = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the minutes between autosaving the sector" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the minutes between autosaving the world
		/// </summary>
		public uint AutoSaveInMinutes
		{
			get { return m_definition.SessionSettings.AutoSaveInMinutes; }
			set
			{
				if ( m_definition.SessionSettings.AutoSaveInMinutes == value ) return;
				m_definition.SessionSettings.AutoSaveInMinutes = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the weapons are functional" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether the weapons are functional
		/// </summary>
		public bool WeaponsEnabled
		{
			get { return m_definition.SessionSettings.WeaponsEnabled; }
			set
			{
				if ( m_definition.SessionSettings.WeaponsEnabled == value ) return;
				m_definition.SessionSettings.WeaponsEnabled = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player names will show on the HUD" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether the player names will show on the HUD
		/// </summary>
		public bool ShowPlayerNamesOnHud
		{
			get { return m_definition.SessionSettings.ShowPlayerNamesOnHud; }
			set
			{
				if ( m_definition.SessionSettings.ShowPlayerNamesOnHud == value ) return;
				m_definition.SessionSettings.ShowPlayerNamesOnHud = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the thrusters damage blocks" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether the thrusters damage blocks
		/// </summary>
		public bool ThrusterDamage
		{
			get { return m_definition.SessionSettings.ThrusterDamage; }
			set
			{
				if ( m_definition.SessionSettings.ThrusterDamage == value ) return;
				m_definition.SessionSettings.ThrusterDamage = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether random ships spawn on the server" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Determine whether random ships spawn on the server
		/// </summary>
		public bool CargoShipsEnabled
		{
			get { return m_definition.SessionSettings.CargoShipsEnabled; }
			set
			{
				if ( m_definition.SessionSettings.CargoShipsEnabled == value ) return;
				m_definition.SessionSettings.CargoShipsEnabled = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "World Settings" )]
		/// <summary>
		/// 
		/// </summary>
		public bool RealisticSound
		{
			get { return m_definition.SessionSettings.RealisticSound; }
			set
			{
				if ( m_definition.SessionSettings.RealisticSound == value ) return;
				m_definition.SessionSettings.RealisticSound = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "World Settings" )]
		/// <summary>
		/// 
		/// </summary>
		public bool PermanentDeath
		{
			get { return m_definition.SessionSettings.PermanentDeath.GetValueOrDefault( true ); }
			set
			{
				if ( m_definition.SessionSettings.PermanentDeath.GetValueOrDefault( true ) == value ) return;
				m_definition.SessionSettings.PermanentDeath = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// 
		/// </summary>
		public bool ClientCanSave
		{
			get { return m_definition.SessionSettings.ClientCanSave; }
			set
			{
				if ( m_definition.SessionSettings.ClientCanSave == value ) return;
				m_definition.SessionSettings.ClientCanSave = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether spectator mode is enable" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Determine whether spectator mode is enable
		/// </summary>
		public bool EnableSpectator
		{
			get { return m_definition.SessionSettings.EnableSpectator; }
			set
			{
				if ( m_definition.SessionSettings.EnableSpectator == value ) return;
				m_definition.SessionSettings.EnableSpectator = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server will automatically remove debris" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Determine whether the server will automatically remove debris
		/// </summary>
		public bool RemoveTrash
		{
			get { return m_definition.SessionSettings.RemoveTrash; }
			set
			{
				if ( m_definition.SessionSettings.RemoveTrash == value ) return;
				m_definition.SessionSettings.RemoveTrash = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the world borders. Ships and players cannot go further than this" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the world borders. Ships and players cannot go further than this
		/// </summary>
		public int WorldSizeKm
		{
			get { return m_definition.SessionSettings.WorldSizeKm; }
			set
			{
				if ( m_definition.SessionSettings.WorldSizeKm == value ) return;
				m_definition.SessionSettings.WorldSizeKm = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether starter ships are removed after a while" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Determine whether starter ships are removed after a while
		/// </summary>
		public bool RespawnShipDelete
		{
			get { return m_definition.SessionSettings.RespawnShipDelete; }
			set
			{
				if ( m_definition.SessionSettings.RespawnShipDelete == value ) return;
				m_definition.SessionSettings.RespawnShipDelete = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set how long people have to wait to spawn in a respawn ship" )]
		[Category( "Global Settings" )]
		/// <summary>
		/// Get or set how long people have to wait to spawn in a respawn ship
		/// </summary>
		public float RespawnShipSpawnTimeMuliplier
		{
			get { return m_definition.SessionSettings.SpawnShipTimeMultiplier; }
			set
			{
				if ( m_definition.SessionSettings.SpawnShipTimeMultiplier == value ) return;
				m_definition.SessionSettings.SpawnShipTimeMultiplier = value;
			}
		}


		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server should reset the ships ownership" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Determine whether the server should reset the ships ownership
		/// </summary>
		public bool ResetOwnership
		{
			get { return m_definition.SessionSettings.ResetOwnership; }
			set
			{
				if ( m_definition.SessionSettings.ResetOwnership == value ) return;
				m_definition.SessionSettings.ResetOwnership = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The seed value to use for procedural generation" )]
		[Category( "World Settings" )]
		public int ProceduralSeed
		{
			get { return m_definition.SessionSettings.ProceduralSeed; }
			set
			{
				if ( m_definition.SessionSettings.ProceduralSeed == value ) return;
				m_definition.SessionSettings.ProceduralSeed = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The density of procedurally generated asteroids" )]
		[Category( "World Settings" )]
		public float ProceduralDensity
		{
			get { return m_definition.SessionSettings.ProceduralDensity; }
			set
			{
				if ( m_definition.SessionSettings.ProceduralDensity == value ) return;
				m_definition.SessionSettings.ProceduralDensity = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The maximum view distance that a player can see in game" )]
		[Category( "World Settings" )]
		public int ViewDistance
		{
			get { return m_definition.SessionSettings.ViewDistance; }
			set
			{
				if ( m_definition.SessionSettings.ViewDistance == value ) return;
				m_definition.SessionSettings.ViewDistance = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Enable or Disable ingame scripting" )]
		[Category( "World Settings" )]
		public bool EnableIngameScripts
		{
			get { return m_definition.SessionSettings.EnableIngameScripts; }
			set
			{
				if ( m_definition.SessionSettings.EnableIngameScripts == value ) return;
				m_definition.SessionSettings.EnableIngameScripts = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the Scenario's TypeId" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the Scenario's TypeId
		/// </summary>
		public MyObjectBuilderType ScenarioTypeId
		{
			get { return m_definition.Scenario.TypeId; }
			set
			{
				if ( m_definition.Scenario.TypeId == value ) return;
				m_definition.Scenario.TypeId = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the scenario's subtype Id" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the scenario's subtype Id
		/// </summary>
		public string ScenarioSubtypeId
		{
			get { return m_definition.Scenario.SubtypeId; }
			set
			{
				if ( m_definition.Scenario.SubtypeId == value ) return;
				m_definition.Scenario.SubtypeId = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the path of the world to load" )]
		[EditorAttribute( typeof( System.Windows.Forms.Design.FolderNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the path of the world to load
		/// </summary>
		public string LoadWorld
		{
			get { return m_definition.LoadWorld; }
			set
			{
				if ( m_definition.LoadWorld == value ) return;
				m_definition.LoadWorld = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip
		/// </summary>
		public string Ip
		{
			get { return m_definition.IP; }
			set
			{
				if ( m_definition.IP == value ) return;
				m_definition.IP = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the steam port" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the steam port
		/// </summary>
		public int SteamPort
		{
			get { return m_definition.SteamPort; }
			set
			{
				if ( m_definition.SteamPort == value ) return;
				m_definition.SteamPort = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the number of asteroid in the world" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set the number of asteroid in the world
		/// </summary>
		public int AsteroidAmount
		{
			get { return m_definition.AsteroidAmount; }
			set
			{
				if ( m_definition.AsteroidAmount == value ) return;
				m_definition.AsteroidAmount = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of administrators of the server" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the list of administrators of the server
		/// </summary>
		public BindingList<string> Administrators
		{
			get
			{
				return new BindingList<string>( m_definition.Administrators );
			}
			set
			{
				m_definition.Administrators = value.ToList( );
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of banned players" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the list of banned players
		/// </summary>
		public string[ ] Banned
		{
			get { return m_definition.Banned.ConvertAll( x => x.ToString( ) ).ToArray( ); }
			set
			{
				var banned = value.ToList( ).ConvertAll( x => ulong.Parse( x ) );
				if ( m_definition.Banned == banned ) return;
				m_definition.Banned = banned;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of Steam workshop mods" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the list of Steam workshop mods
		/// </summary>
		public string[ ] Mods
		{
			get { return m_definition.Mods.ConvertAll( x => x.ToString( ) ).ToArray( ); }
			set
			{
				var mods = value.ToList( ).ConvertAll( x => ulong.Parse( x ) );
				if ( m_definition.Mods == mods ) return;
				m_definition.Mods = mods;

			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set if the server should pause the game if there is no players online" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set if the server should pause the game if there is no players online
		/// </summary>
		public bool PauseGameWhenEmpty
		{
			get { return m_definition.PauseGameWhenEmpty; }
			set
			{
				if ( m_definition.PauseGameWhenEmpty == value ) return;
				m_definition.PauseGameWhenEmpty = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set if the last session should be ignored" )]
		[Category( "World Settings" )]
		/// <summary>
		/// Get or set if the last session should be ignored
		/// </summary>
		public bool IgnoreLastSession
		{
			get { return m_definition.IgnoreLastSession; }
			set
			{
				if ( m_definition.IgnoreLastSession == value ) return;
				m_definition.IgnoreLastSession = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the name of the world(map)" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the name of the world(map)
		/// </summary>
		public string WorldName
		{
			get { return m_definition.WorldName; }
			set
			{
				if ( m_definition.WorldName == value ) return;
				m_definition.WorldName = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the GroupId of the server.\n" +
	"Only member of this group will be able to join the server.\n" +
	"Set to 0 to open the server to everyone" )]
		[Category( "Server Settings" )]
		/// <summary>
		/// Get or set the GroupId of the server. 
		/// Only member of this group will be able to join the server.
		/// Set to 0 to open the server to everyone
		/// </summary>
		public ulong GroupID
		{
			get { return m_definition.GroupID; }
			set
			{
				if ( m_definition.GroupID == value ) return;
				m_definition.GroupID = value;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Load the dedicated server configuration file
		/// </summary>
		/// <param name="fileInfo">Path to the configuration file</param>
		/// <returns></returns>
		public static MyConfigDedicatedData Load( FileInfo fileInfo )
		{
			object fileContent;

			string filePath = fileInfo.FullName;

			if ( !File.Exists( filePath ) )
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileMissing, filePath );
			}

			try
			{
				var settings = new XmlReaderSettings
				{
					IgnoreComments = true,
					IgnoreWhitespace = true,
				};

				using ( var xmlReader = XmlReader.Create( filePath, settings ) )
				{
					var serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance( typeof( MyConfigDedicatedDataSerializer ) );
					fileContent = serializer.Deserialize( xmlReader );
				}
			}
			catch
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath );
			}

			if ( fileContent == null )
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileEmpty, filePath );
			}

			return (MyConfigDedicatedData)fileContent;
		}

		public bool Save( FileInfo fileInfo )
		{
			if ( fileInfo == null ) return false;

			//Save the definitions container out to the file
			try
			{
				using ( var xmlTextWriter = new XmlTextWriter( fileInfo.FullName, null ) )
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					MyConfigDedicatedDataSerializer serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance( typeof( MyConfigDedicatedDataSerializer ) );
					serializer.Serialize( xmlTextWriter, m_definition );
				}
			}
			catch
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileCorrupted, fileInfo.FullName );
			}

			if ( m_definition == null )
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileEmpty, fileInfo.FullName );
			}

			return true;
		}


		#endregion
	}
}
