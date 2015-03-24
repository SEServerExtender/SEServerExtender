namespace SEModAPI.API.Definitions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Configuration;
	using System.Drawing.Design;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Windows.Forms.Design;
	using System.Xml;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class DedicatedConfigDefinition
	{
		private readonly MyConfigDedicatedData _definition;

		public DedicatedConfigDefinition( MyConfigDedicatedData definition )
		{
			_definition = definition;
		}

		#region "Properties"

		/// <summary>
		/// Get or set the server's name
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the server's name" )]
		[Category( "Server Settings" )]
		public string ServerName
		{
			get { return _definition.ServerName; }
			set
			{
				if ( _definition.ServerName == value ) return;
				_definition.ServerName = value;
			}
		}

		/// <summary>
		/// Get or set the server's port
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the server's port" )]
		[Category( "Server Settings" )]
		public int ServerPort
		{
			get { return _definition.ServerPort; }
			set
			{
				if ( _definition.ServerPort == value ) return;
				_definition.ServerPort = value;
			}
		}

		/// <summary>
		/// Get or set the game mode
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the game mode" )]
		[Category( "Global Settings" )]
		public MyGameModeEnum GameMode
		{
			get { return _definition.SessionSettings.GameMode; }
			set
			{
				if ( _definition.SessionSettings.GameMode == value ) return;
				_definition.SessionSettings.GameMode = value;
				return;
			}
		}

		/// <summary>
		/// Get or set the inventory size multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the grinder speed multiplier" )]
		[Category( "Global Settings" )]
		public float GrinderSpeedMultiplier
		{
			get { return _definition.SessionSettings.GrinderSpeedMultiplier; }
			set
			{
				if ( _definition.SessionSettings.GrinderSpeedMultiplier == value ) return;
				_definition.SessionSettings.GrinderSpeedMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the welder speed multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the welder speed multiplier" )]
		[Category( "Global Settings" )]
		public float WelderSpeedMultiplier
		{
			get { return _definition.SessionSettings.WelderSpeedMultiplier; }
			set
			{
				if ( _definition.SessionSettings.WelderSpeedMultiplier == value ) return;
				_definition.SessionSettings.WelderSpeedMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the inventory size multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the inventory size multiplier" )]
		[Category( "Global Settings" )]
		public float InventorySizeMultiplier
		{
			get { return _definition.SessionSettings.InventorySizeMultiplier; }
			set
			{
				if ( _definition.SessionSettings.InventorySizeMultiplier == value ) return;
				_definition.SessionSettings.InventorySizeMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the assembler speed multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the assembler speed multiplier" )]
		[Category( "Global Settings" )]
		public float AssemblerSpeedMultiplier
		{
			get { return _definition.SessionSettings.AssemblerSpeedMultiplier; }
			set
			{
				if ( _definition.SessionSettings.AssemblerSpeedMultiplier == value ) return;
				_definition.SessionSettings.AssemblerSpeedMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the assembler efficiency multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the assembler efficiency multiplier" )]
		[Category( "Global Settings" )]
		public float AssemblerEfficiencyMultiplier
		{
			get { return _definition.SessionSettings.AssemblerEfficiencyMultiplier; }
			set
			{
				if ( _definition.SessionSettings.AssemblerEfficiencyMultiplier == value ) return;
				_definition.SessionSettings.AssemblerEfficiencyMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the refinery speed multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the refinery speed multiplier" )]
		[Category( "Global Settings" )]
		public float RefinerySpeedMultiplier
		{
			get { return _definition.SessionSettings.RefinerySpeedMultiplier; }
			set
			{
				if ( _definition.SessionSettings.RefinerySpeedMultiplier == value ) return;
				_definition.SessionSettings.RefinerySpeedMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the hacking speed multiplier
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the hacking speed multiplier" )]
		[Category( "Global Settings" )]
		public float HackSpeedMultiplier
		{
			get { return _definition.SessionSettings.HackSpeedMultiplier; }
			set
			{
				if ( _definition.SessionSettings.HackSpeedMultiplier == value ) return;
				_definition.SessionSettings.HackSpeedMultiplier = value;
			}
		}

		/// <summary>
		/// Get or set the online mode
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the online mode" )]
		[Category( "Global Settings" )]
		public MyOnlineModeEnum OnlineMode
		{
			get { return _definition.SessionSettings.OnlineMode; }
			set
			{
				if ( _definition.SessionSettings.OnlineMode == value ) return;
				_definition.SessionSettings.OnlineMode = value;
			}
		}

		/// <summary>
		/// Get or set the maximum number of players
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the maximum number of players" )]
		[Category( "Server Settings" )]
		public short MaxPlayers
		{
			get { return _definition.SessionSettings.MaxPlayers; }
			set
			{
				if ( _definition.SessionSettings.MaxPlayers == value ) return;
				_definition.SessionSettings.MaxPlayers = value;
			}
		}

		/// <summary>
		/// Get or set the maximum number of floating object
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the maximum number of floating object" )]
		[Category( "Global Settings" )]
		public short MaxFloatingObject
		{
			get { return _definition.SessionSettings.MaxFloatingObjects; }
			set
			{
				if ( _definition.SessionSettings.MaxFloatingObjects == value ) return;
				_definition.SessionSettings.MaxFloatingObjects = value;
			}
		}

		/// <summary>
		/// Get or set the environment hostility
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the environment hostility" )]
		[Category( "World Settings" )]
		public MyEnvironmentHostilityEnum EnvironmentHostility
		{
			get { return _definition.SessionSettings.EnvironmentHostility; }
			set
			{
				if ( _definition.SessionSettings.EnvironmentHostility == value ) return;
				_definition.SessionSettings.EnvironmentHostility = value;
			}
		}

		/// <summary>
		/// Determine whether the player's health auto heal
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player's health auto heal" )]
		[Category( "Global Settings" )]
		public bool AutoHealing
		{
			get { return _definition.SessionSettings.AutoHealing; }
			set
			{
				if ( _definition.SessionSettings.AutoHealing == value ) return;
				_definition.SessionSettings.AutoHealing = value;
			}
		}

		/// <summary>
		/// Determine whether the player can copy/paste ships
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player can copy/paste ships" )]
		[Category( "Global Settings" )]
		public bool EnableCopyPaste
		{
			get { return _definition.SessionSettings.EnableCopyPaste; }
			set
			{
				if ( _definition.SessionSettings.EnableCopyPaste == value ) return;
				_definition.SessionSettings.EnableCopyPaste = value;
			}
		}

		/// <summary>
		/// Determine whether the server will save regularly the sector
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server will save regularly the sector" )]
		[Category( "Server Settings" )]
		public bool AutoSave
		{
			get { return _definition.SessionSettings.AutoSave; }
			set
			{
				if ( _definition.SessionSettings.AutoSave == value ) return;
				_definition.SessionSettings.AutoSave = value;
			}
		}

		/// <summary>
		/// Get or set the minutes between autosaving the world
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the minutes between autosaving the sector" )]
		[Category( "Server Settings" )]
		public uint AutoSaveInMinutes
		{
			get { return _definition.SessionSettings.AutoSaveInMinutes; }
			set
			{
				if ( _definition.SessionSettings.AutoSaveInMinutes == value ) return;
				_definition.SessionSettings.AutoSaveInMinutes = value;
			}
		}

		/// <summary>
		/// Determine whether the weapons are functional
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the weapons are functional" )]
		[Category( "Global Settings" )]
		public bool WeaponsEnabled
		{
			get { return _definition.SessionSettings.WeaponsEnabled; }
			set
			{
				if ( _definition.SessionSettings.WeaponsEnabled == value ) return;
				_definition.SessionSettings.WeaponsEnabled = value;
			}
		}

		/// <summary>
		/// Determine whether the player names will show on the HUD
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player names will show on the HUD" )]
		[Category( "Global Settings" )]
		public bool ShowPlayerNamesOnHud
		{
			get { return _definition.SessionSettings.ShowPlayerNamesOnHud; }
			set
			{
				if ( _definition.SessionSettings.ShowPlayerNamesOnHud == value ) return;
				_definition.SessionSettings.ShowPlayerNamesOnHud = value;
			}
		}

		/// <summary>
		/// Determine whether the thrusters damage blocks
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the thrusters damage blocks" )]
		[Category( "Global Settings" )]
		public bool ThrusterDamage
		{
			get { return _definition.SessionSettings.ThrusterDamage; }
			set
			{
				if ( _definition.SessionSettings.ThrusterDamage == value ) return;
				_definition.SessionSettings.ThrusterDamage = value;
			}
		}

		/// <summary>
		/// Determine whether random ships spawn on the server
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether random ships spawn on the server" )]
		[Category( "World Settings" )]
		public bool CargoShipsEnabled
		{
			get { return _definition.SessionSettings.CargoShipsEnabled; }
			set
			{
				if ( _definition.SessionSettings.CargoShipsEnabled == value ) return;
				_definition.SessionSettings.CargoShipsEnabled = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "World Settings" )]
		public bool RealisticSound
		{
			get { return _definition.SessionSettings.RealisticSound; }
			set
			{
				if ( _definition.SessionSettings.RealisticSound == value ) return;
				_definition.SessionSettings.RealisticSound = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "World Settings" )]
		public bool PermanentDeath
		{
			get { return _definition.SessionSettings.PermanentDeath.GetValueOrDefault( true ); }
			set
			{
				if ( _definition.SessionSettings.PermanentDeath.GetValueOrDefault( true ) == value ) return;
				_definition.SessionSettings.PermanentDeath = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "" )]
		[Category( "Global Settings" )]
		public bool ClientCanSave
		{
			get { return _definition.SessionSettings.ClientCanSave; }
			set
			{
				if ( _definition.SessionSettings.ClientCanSave == value ) return;
				_definition.SessionSettings.ClientCanSave = value;
			}
		}

		/// <summary>
		/// Get or set whether spectator mode is enabled
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether spectator mode is enable" )]
		[Category( "Global Settings" )]
		public bool EnableSpectator
		{
			get { return _definition.SessionSettings.EnableSpectator; }
			set
			{
				if ( _definition.SessionSettings.EnableSpectator == value ) return;
				_definition.SessionSettings.EnableSpectator = value;
			}
		}

		/// <summary>
		/// Get or set whether the server will automatically remove debris
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server will automatically remove debris" )]
		[Category( "World Settings" )]
		public bool RemoveTrash
		{
			get { return _definition.SessionSettings.RemoveTrash; }
			set
			{
				if ( _definition.SessionSettings.RemoveTrash == value ) return;
				_definition.SessionSettings.RemoveTrash = value;
			}
		}

		/// <summary>
		/// Get or set the world borders. Ships and players cannot go further than this
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the world borders. Ships and players cannot go further than this" )]
		[Category( "World Settings" )]
		public int WorldSizeKm
		{
			get { return _definition.SessionSettings.WorldSizeKm; }
			set
			{
				if ( _definition.SessionSettings.WorldSizeKm == value ) return;
				_definition.SessionSettings.WorldSizeKm = value;
			}
		}

		/// <summary>
		/// Determine whether starter ships are removed after a while
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether starter ships are removed after a while" )]
		[Category( "World Settings" )]
		public bool RespawnShipDelete
		{
			get { return _definition.SessionSettings.RespawnShipDelete; }
			set
			{
				if ( _definition.SessionSettings.RespawnShipDelete == value ) return;
				_definition.SessionSettings.RespawnShipDelete = value;
			}
		}

		/// <summary>
		/// Get or set how long people have to wait to spawn in a respawn ship
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set how long people have to wait to spawn in a respawn ship" )]
		[Category( "Global Settings" )]
		public float RespawnShipSpawnTimeMuliplier
		{
			get { return _definition.SessionSettings.SpawnShipTimeMultiplier; }
			set
			{
				if ( _definition.SessionSettings.SpawnShipTimeMultiplier == value ) return;
				_definition.SessionSettings.SpawnShipTimeMultiplier = value;
			}
		}


		/// <summary>
		/// Determine whether the server should reset the ships ownership
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server should reset the ships ownership" )]
		[Category( "World Settings" )]
		public bool ResetOwnership
		{
			get { return _definition.SessionSettings.ResetOwnership; }
			set
			{
				if ( _definition.SessionSettings.ResetOwnership == value ) return;
				_definition.SessionSettings.ResetOwnership = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The seed value to use for procedural generation" )]
		[Category( "World Settings" )]
		public int ProceduralSeed
		{
			get { return _definition.SessionSettings.ProceduralSeed; }
			set
			{
				if ( _definition.SessionSettings.ProceduralSeed == value ) return;
				_definition.SessionSettings.ProceduralSeed = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The density of procedurally generated asteroids" )]
		[Category( "World Settings" )]
		public float ProceduralDensity
		{
			get { return _definition.SessionSettings.ProceduralDensity; }
			set
			{
				if ( _definition.SessionSettings.ProceduralDensity == value ) return;
				_definition.SessionSettings.ProceduralDensity = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The maximum view distance that a player can see in game" )]
		[Category( "World Settings" )]
		public int ViewDistance
		{
			get { return _definition.SessionSettings.ViewDistance; }
			set
			{
				if ( _definition.SessionSettings.ViewDistance == value ) return;
				_definition.SessionSettings.ViewDistance = value;
			}
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Enable or Disable ingame scripting" )]
		[Category( "World Settings" )]
		public bool EnableIngameScripts
		{
			get { return _definition.SessionSettings.EnableIngameScripts; }
			set
			{
				if ( _definition.SessionSettings.EnableIngameScripts == value ) return;
				_definition.SessionSettings.EnableIngameScripts = value;
			}
		}

		/// <summary>
		/// Get or set the Scenario's TypeId
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the Scenario's TypeId" )]
		[Category( "World Settings" )]
		public MyObjectBuilderType ScenarioTypeId
		{
			get { return _definition.Scenario.TypeId; }
			set
			{
				if ( _definition.Scenario.TypeId == value ) return;
				_definition.Scenario.TypeId = value;
			}
		}

		/// <summary>
		/// Get or set the scenario's subtype Id
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the scenario's subtype Id" )]
		[Category( "World Settings" )]
		public string ScenarioSubtypeId
		{
			get { return _definition.Scenario.SubtypeId; }
			set
			{
				if ( _definition.Scenario.SubtypeId == value ) return;
				_definition.Scenario.SubtypeId = value;
			}
		}

		/// <summary>
		/// Get or set the path of the world to load
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the path of the world to load" )]
		[Editor( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		[Category( "World Settings" )]
		public string LoadWorld
		{
			get { return _definition.LoadWorld; }
			set
			{
				if ( _definition.LoadWorld == value ) return;
				_definition.LoadWorld = value;
			}
		}

		/// <summary>
		/// Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip" )]
		[Category( "Server Settings" )]
		public string Ip
		{
			get { return _definition.IP; }
			set
			{
				if ( _definition.IP == value ) return;
				_definition.IP = value;
			}
		}

		/// <summary>
		/// Get or set the steam port
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the steam port" )]
		[Category( "Server Settings" )]
		public int SteamPort
		{
			get { return _definition.SteamPort; }
			set
			{
				if ( _definition.SteamPort == value ) return;
				_definition.SteamPort = value;
			}
		}

		/// <summary>
		/// Get or set the number of asteroid in the world
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the number of asteroid in the world" )]
		[Category( "World Settings" )]
		public int AsteroidAmount
		{
			get { return _definition.AsteroidAmount; }
			set
			{
				if ( _definition.AsteroidAmount == value ) return;
				_definition.AsteroidAmount = value;
			}
		}

		/// <summary>
		/// Get or set the list of administrators of the server
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of administrators of the server" )]
		[Category( "Server Settings" )]
		public BindingList<string> Administrators
		{
			get
			{
				return new BindingList<string>( _definition.Administrators );
			}
			set
			{
				_definition.Administrators = value.ToList( );
			}
		}

		/// <summary>
		/// Get or set the list of banned players
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of banned players" )]
		[Category( "Server Settings" )]
		public string[ ] Banned
		{
			get { return _definition.Banned.ConvertAll( x => x.ToString( ) ).ToArray( ); }
			set
			{
				List<ulong> banned = value.ToList( ).ConvertAll( ulong.Parse );
				if ( _definition == null )
				{
					return;
				}
				if ( _definition.Banned == banned )
					return;
				_definition.Banned = banned;
			}
		}

		/// <summary>
		/// Get or set the list of Steam workshop mods
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the list of Steam workshop mods" )]
		[Category( "Server Settings" )]
		public string[ ] Mods
		{
			get { return _definition.Mods.ConvertAll( x => x.ToString( ) ).ToArray( ); }
			set
			{
				List<ulong> mods = value.ToList( ).ConvertAll( ulong.Parse );
				if ( _definition.Mods == mods )
					return;
				_definition.Mods = mods;

			}
		}

		/// <summary>
		/// Get or set if the server should pause the game if there is no players online
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set if the server should pause the game if there is no players online" )]
		[Category( "World Settings" )]
		public bool PauseGameWhenEmpty
		{
			get { return _definition.PauseGameWhenEmpty; }
			set
			{
				if ( _definition.PauseGameWhenEmpty == value ) return;
				_definition.PauseGameWhenEmpty = value;
			}
		}

		/// <summary>
		/// Get or set if the last session should be ignored
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set if the last session should be ignored" )]
		[Category( "World Settings" )]
		public bool IgnoreLastSession
		{
			get { return _definition.IgnoreLastSession; }
			set
			{
				if ( _definition.IgnoreLastSession == value ) return;
				_definition.IgnoreLastSession = value;
			}
		}

		/// <summary>
		/// Get or set the name of the world(map)
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the name of the world(map)" )]
		[Category( "Server Settings" )]
		public string WorldName
		{
			get { return _definition.WorldName; }
			set
			{
				if ( _definition.WorldName == value ) return;
				_definition.WorldName = value;
			}
		}

		/// <summary>
		/// Get or set the GroupId of the server. 
		/// Only members of this group will be able to join the server.
		/// Set to 0 to open the server to everyone
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the GroupId of the server.\n" +
	"Only members of this group will be able to join the server.\n" +
	"Set to 0 to open the server to everyone" )]
		[Category( "Server Settings" )]
		public ulong GroupID
		{
			get { return _definition.GroupID; }
			set
			{
				if ( _definition.GroupID == value ) return;
				_definition.GroupID = value;
			}
		}

		/// <summary>
		/// Get or set the whether oxygen mechanics are in use.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the whether oxygen mechanics are in use." )]
		[Category( "World Settings" )]
		public bool EnableOxygen
		{
			get { return _definition.SessionSettings.EnableOxygen; }
			set
			{
				if ( _definition.SessionSettings.EnableOxygen != value )
					_definition.SessionSettings.EnableOxygen = value;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Load the dedicated server configuration file
		/// </summary>
		/// <param name="fileInfo">Path to the configuration file</param>
		/// <exception cref="FileNotFoundException">Thrown if configuration file cannot be found at the path specified.</exception>
		/// <returns></returns>
		/// <exception cref="ConfigurationErrorsException">Configuration file not understood. See inner exception for details. Ignore configuration file line number in outer exception.</exception>
		public static MyConfigDedicatedData Load( FileInfo fileInfo )
		{
			object fileContent;

			string filePath = fileInfo.FullName;

			if ( !File.Exists( filePath ) )
			{
				throw new FileNotFoundException( "Game configuration file not found.", filePath );
			}

			try
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					IgnoreComments = true,
					IgnoreWhitespace = true,
				};

				using ( XmlReader xmlReader = XmlReader.Create( filePath, settings ) )
				{
					MyConfigDedicatedDataSerializer serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance( typeof( MyConfigDedicatedDataSerializer ) );
					fileContent = serializer.Deserialize( xmlReader );
				}
			}
			catch(Exception ex)
			{
				throw new ConfigurationErrorsException( "Configuration file not understood. See inner exception for details. Ignore configuration file line number in outer exception.", ex, filePath, -1 );
			}

			if ( fileContent == null )
			{
				throw new ConfigurationErrorsException( "Configuration file empty." );
			}

			return (MyConfigDedicatedData)fileContent;
		}

		public bool Save( FileInfo fileInfo )
		{
			if ( fileInfo == null ) return false;

			//Save the definitions container out to the file
			try
			{
				using ( XmlTextWriter xmlTextWriter = new XmlTextWriter( fileInfo.FullName, null ) )
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					MyConfigDedicatedDataSerializer serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance( typeof( MyConfigDedicatedDataSerializer ) );
					serializer.Serialize( xmlTextWriter, _definition );
				}
			}
			catch
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileCorrupted, fileInfo.FullName );
			}

			if ( _definition == null )
			{
				throw new GameInstallationInfoException( GameInstallationInfoExceptionState.ConfigFileEmpty, fileInfo.FullName );
			}

			return true;
		}


		#endregion
	}
}
