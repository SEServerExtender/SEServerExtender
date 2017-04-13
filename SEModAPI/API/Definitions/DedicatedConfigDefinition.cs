using System.Reflection;
using System.Text;
using NLog;
using Sandbox.Definitions;
using Sandbox.Engine.Multiplayer;
using SpaceEngineers.Game;
using SteamSDK;
using VRage.Game;
using VRage.Library.Utils;
using VRage.Serialization;
using VRage.Utils;

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
	using System.Xml.Serialization;
	using global::Sandbox.Common.ObjectBuilders;
	using VRage.ObjectBuilders;

	[DataContract]
	public class DedicatedConfigDefinition
	{
        public static readonly Logger BaseLog = LogManager.GetLogger("BaseLog");
        private readonly MyConfigDedicatedData<MyObjectBuilder_SessionSettings> _definition;
        
        //this is here only so the block limit config screen can get and set this data
        [Browsable(false)]
	    public static Dictionary<string, short> Limits { get; set; }

		public DedicatedConfigDefinition( MyConfigDedicatedData<MyObjectBuilder_SessionSettings> definition )
		{
			_definition = definition;
            //Limits = definition.SessionSettings.BlockTypeLimits.Dictionary;
		    var dic = definition.SessionSettings.GetType().GetField("BlockTypeLimits", BindingFlags.Public | BindingFlags.Instance)?.GetValue(definition.SessionSettings) as SerializableDictionary<string, short>;
		    Limits = dic?.Dictionary;
		}

        #region "Properties"
        private string chatname = "Server";
        // I'll figure out how to save this later. For now it's hardcoded to "Server"
        /// <summary>
        /// Get or set the server's name
        /// </summary>
        [DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Chat messages sent by the server will show this name. You MUST have the Essentials client mod installed for this to work. " +
            "\r\nNote: This value isn't saved between sessions. This is set separately from Essentials, you may want to change them to match." )]
        [Category( "Extender Settings" )]
        [DisplayName( "Server Chat Name" )]
        public string ServerChatName
        {
            get
            {
                return chatname;
            }
            set
            {
                chatname = value;
            }
        }
        

        /// <summary>
        /// Get or set the server's name
        /// </summary>
        [DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the server's name" )]
		[Category( "Server Settings" )]
		[DisplayName( "Server Name" )]
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
		[DisplayName( "Server Port" )]
		[DefaultValue( 27016 )]
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
		[DisplayName( "Game Mode" )]
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
		[DisplayName( "Grinder Speed Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Welder Speed Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Inventory Size Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Assembler Speed Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Assembler Efficiency Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Refinery Speed Multiplier" )]
		[DefaultValue( 1 )]
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
		[DisplayName( "Hacking Speed Multiplier" )]
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
		[Description( "Get or set the online mode. This controls who is able to connect to your server.  If set to public and Group ID is also set, only members of that Steam group will be able to connect." )]
		[Category( "Global Settings" )]
		[DisplayName( "Online Mode" )]
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
		[Description( "Get or set the maximum number of players. This can be changed in real time" )]
		[Category( "Server Settings" )]
		[DisplayName( "Player Limit" )]
		public short MaxPlayers
		{
			get { return _definition.SessionSettings.MaxPlayers; }
			set
			{
				if ( _definition.SessionSettings.MaxPlayers == value ) return;
				_definition.SessionSettings.MaxPlayers = value;

			    if (MyMultiplayer.Static != null)
			        MyMultiplayer.Static.MemberLimit = value;
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
		[DisplayName( "Floating Object Limit" )]
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
		[Description( "Get or set the environment hostility. Controls if or how many meteor storms will occur." )]
		[Category( "World Settings" )]
		[DisplayName( "Environment Hostility" )]
		[DefaultValue( MyEnvironmentHostilityEnum.SAFE )]
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
		/// Determine whether the player's health auto heals
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the player's health auto heals" )]
		[Category( "Global Settings" )]
		[DisplayName( "Auto-Healing" )]
		[DefaultValue( true )]
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
		[DisplayName( "Copy/Paste" )]
		[DefaultValue( true )]
		public bool EnableCopyPaste
		{
			get { return _definition.SessionSettings.EnableCopyPaste; }
			set
			{
				if ( _definition.SessionSettings.EnableCopyPaste == value ) return;
				_definition.SessionSettings.EnableCopyPaste = value;
			}
		}

        /*
         * this property is marked obsolete internally
		/// <summary>
		/// Determine whether the server will save regularly the sector
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "Enable or disable built-in auto-save.  This is automatically disabled when you start the server, to prevent conflicts." )]
		[Category( "Server Settings" )]
		[DisplayName( "Auto-Save" )]
		[DefaultValue( false )]
		public bool AutoSave
		{
			get { return _definition.SessionSettings.AutoSave; }
			set
			{
				if ( _definition.SessionSettings.AutoSave == value ) return;
				_definition.SessionSettings.AutoSave = value;
			}
		}
        */

		/// <summary>
		/// Get or set the minutes between autosaving the world
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The interval for built-in auto-save, in minutes.  Does not affect Extender's auto-save setting." )]
		[Category( "Server Settings" )]
		[DisplayName( "Auto-Save Interval" )]
		[DefaultValue( 0 )]
		public uint AutoSaveInMinutes
		{
			get { return _definition.SessionSettings.AutoSaveInMinutes; }
			set { _definition.SessionSettings.AutoSaveInMinutes = value; }
		}

        /// <summary>
        /// Get or set the number of auto-backup
        /// </summary>
        [DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("The max number of automatic backups the server keeps.")]
        [Category("Server Settings")]
        [DisplayName("Max Backup Count")]
        [DefaultValue(5)]
        public short MaxBackupSaves
	    {
	        get { return _definition.SessionSettings.MaxBackupSaves; }
            set { _definition.SessionSettings.MaxBackupSaves = value; }
	    }

		/// <summary>
		/// Determine whether the weapons are functional
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the weapons are functional" )]
		[Category( "Global Settings" )]
		[DisplayName( "Weapons" )]
		[DefaultValue( true )]
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
		[DisplayName( "Show Player Names On HUD" )]
		[DefaultValue( true )]
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
		[DisplayName( "Thruster Damage" )]
		[DefaultValue( true )]
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
		[DisplayName( "Cargo Ships" )]
		[DefaultValue( true )]
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
		[DisplayName( "Realistic Sound" )]
		[DefaultValue( true )]
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
		[Description( "When a player dies, they are removed from all factions and their ownership is removed from all ships and blocks." )]
		[Category( "World Settings" )]
		[DisplayName( "Permanent Death" )]
		[DefaultValue( false )]
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
		[Description( "Allows or disallows clients saving a local copy of the world." )]
		[Category( "Global Settings" )]
		[DisplayName( "Client Can Save" )]
		[DefaultValue( true )]
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
		[Description( "Determine whether spectator mode is enabled" )]
		[Category( "Global Settings" )]
		[DisplayName( "Allow Spectators" )]
		[DefaultValue( true )]
		public bool EnableSpectator
		{
			get { return _definition.SessionSettings.EnableSpectator; }
			set
			{
				if ( _definition.SessionSettings.EnableSpectator == value ) return;
				_definition.SessionSettings.EnableSpectator = value;
			}
		}

        /*
		/// <summary>
		/// Get or set whether the server will automatically remove debris
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Determine whether the server will automatically remove debris" )]
		[Category( "World Settings" )]
		[DisplayName( "Remove Trash" )]
		[DefaultValue( false )]
		public bool RemoveTrash
		{
			get { return _definition.SessionSettings.RemoveTrash; }
			set
			{
				if ( _definition.SessionSettings.RemoveTrash == value ) return;
				_definition.SessionSettings.RemoveTrash = value;
			}
		}
        */

		/// <summary>
		/// Get or set the world borders. Ships and players cannot go further than this
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Ships and players which travel beyond this limit are destroyed/killed. Set to 0 for infinite." )]
		[Category( "World Settings" )]
		[DisplayName( "World Size (km)" )]
		[DefaultValue( 0 )]
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
		[DisplayName( "Delete Respawn Ships" )]
		[DefaultValue( false )]
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
		[Description( "A multiplier applied to the time a player has to wait to be able to respawn in a ship." )]
		[Category( "Global Settings" )]
		[DisplayName( "Respawn Ship Spawn Time Multiplier" )]
		[DefaultValue( 1 )]
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
		[Description( "Resets all ownership of all blocks and ships, when the server launches. USE WITH CAUTION!" )]
		[Category( "World Settings" )]
		[DisplayName( "Reset Ownership" )]
		[DefaultValue( false )]
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
		[DisplayName( "Procedural Seed" )]
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
		[DisplayName( "Procedural Density" )]
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
		[DisplayName( "View Distance" )]
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
		[Description( "Enable or disable generation of ships/stations at random locations in procedural worlds" )]
		[Category( "World Settings" )]
		[DisplayName( "Enable Encounters" )]
		[DefaultValue( true )]
		public bool EnableEncounters
		{
			get { return _definition.SessionSettings.EnableEncounters; }
			set { _definition.SessionSettings.EnableEncounters = value; }
		}

		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Enable or Disable ingame scripting" )]
		[Category( "World Settings" )]
		[DisplayName( "In-Game Scripts" )]
		[DefaultValue( true )]
		public bool EnableIngameScripts
		{
			get { return _definition.SessionSettings.EnableIngameScripts; }
			set
			{
				if ( _definition.SessionSettings.EnableIngameScripts == value ) return;
				_definition.SessionSettings.EnableIngameScripts = value;
			}
		}
        /*
		/// <summary>
		/// Get or set the Scenario's TypeId
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Only relevant for new worlds. This tells the game which pre-made scenario to load, if a world save location is not specified or does not exist." )]
		[Category( "World Settings - New Worlds Only" )]
		[DisplayName( "Scenario Type ID" )]
		public MyObjectBuilderType ScenarioTypeId
		{
			get { return _definition.Scenario.TypeId; }
			set
			{
				if ( _definition.Scenario.TypeId == value ) return;
				_definition.Scenario.TypeId = value;
			}
		}
        */
        /*
		/// <summary>
		/// Get or set the scenario's subtype Id
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Only relevant for new worlds. This tells the game which pre-made scenario to load, if a world save location is not specified or does not exist." )]
		[Category( "World Settings - New Worlds Only" )]
		[DisplayName( "Scenario Sub-Type ID" )]
		public string ScenarioSubtypeId
		{
			get { return _definition.Scenario.SubtypeId; }
			set
			{
			    if ( MyDefinitionManager.Static.GetScenarioDefinitions().Count == 0 )
			        MyDefinitionManager.Static.LoadScenarios();

                var sb = new StringBuilder();
			    foreach ( var scenario in MyDefinitionManager.Static.GetScenarioDefinitions() )
			    {
			        if ( !scenario.Public )
			            continue;

			        sb.AppendLine( scenario.Id.SubtypeName );
			        if ( scenario.Id.SubtypeName == value )
			        {
			            _definition.Scenario = scenario.Id;
			            return;
			        }
			    }
                throw new ArgumentException($"{value} is not a valid scenario definition type! Try one of these:\r\n{sb}");
			}
		}
        */
		/// <summary>
		/// Get or set the path of the world to load
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the path of the world to load" )]
		[Editor( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		[Category( "World Settings" )]
		[DisplayName( "World Save Location" )]
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
		[Description( "The IP address the server listens on, for new player connections. Set to 0.0.0.0 to listen on all addresses." )]
		[Category( "Server Settings" )]
		[DisplayName( "Server Listener IP" )]
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
		[DisplayName( "Steam Port" )]
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
		[Description( "The number of asteroids generated in a new world. Has no effect on existing worlds" )]
		[Category( "World Settings - New Worlds Only" )]
		[DisplayName( "Asteroid Amount" )]
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
		[Description( "The list of administrators of the server, by steam ID" )]
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
		[Description( "The list of banned players, by steam ID" )]
		[Category( "Server Settings" )]
		[DisplayName( "Banned Users" )]
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
		[Description( "Pause the game if there are no players online" )]
		[Category( "World Settings" )]
		[DisplayName( "Pause Game When Empty" )]
		[DefaultValue( false )]
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
		[DisplayName( "Ignore Last Session" )]
		[DefaultValue( false )]
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
		[DisplayName( "World Name" )]
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
		[DisplayName( "Steam Group ID" )]
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
		/// Get or set whether oxygen mechanics are in use.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set whether oxygen mechanics are in use." )]
		[Category( "World Settings" )]
		[DisplayName( "Oxygen" )]
		[DefaultValue( true )]
		public bool EnableOxygen
		{
			get { return _definition.SessionSettings.EnableOxygen; }
			set
			{
				if ( _definition.SessionSettings.EnableOxygen != value )
					_definition.SessionSettings.EnableOxygen = value;
			}
		}

		/// <summary>
		/// Get or set whether tools cause ships to shake when operating.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set whether tools cause ships to shake when operating." )]
		[Category( "World Settings" )]
		[DisplayName( "Tool Shake" )]
		[DefaultValue( true )]
		public bool EnableToolShake
		{
			get { return _definition.SessionSettings.EnableToolShake; }
			set
			{
				if ( _definition.SessionSettings.EnableToolShake != value )
					_definition.SessionSettings.EnableToolShake = value;
			}
		}

		/// <summary>
		/// Get or set the VoxelGeneratorVersion setting..
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the VoxelGeneratorVersion setting." )]
		[Category( "World Settings" )]
		[DisplayName( "Voxel Generator Version" )]
		[DefaultValue( 1 )]
		public int VoxelGeneratorVersion
		{
			get { return _definition.SessionSettings.VoxelGeneratorVersion; }
			set
			{
				if ( _definition.SessionSettings.VoxelGeneratorVersion != value )
					_definition.SessionSettings.VoxelGeneratorVersion = value;
			}
		}

		/// <summary>
		/// Get or set the EnableRespawnShips setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the EnableRespawnShips setting." )]
		[Category( "World Settings" )]
		[DisplayName( "Enable Respawn Ships" )]
		[DefaultValue( false )]
		public bool DisableRespawnShips
		{
			get { return _definition.SessionSettings.EnableRespawnShips; }
			set
			{
				if ( _definition.SessionSettings.EnableRespawnShips != value )
					_definition.SessionSettings.EnableRespawnShips = value;
			}
		}

		/// <summary>
		/// Get or set the SpawnWithTools setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Get or set the SpawnWithTools setting." )]
		[Category( "World Settings" )]
		[DisplayName( "Spawn With Tools" )]
		[DefaultValue( true )]
		public bool SpawnWithTools
		{
			get { return _definition.SessionSettings.SpawnWithTools; }
			set
			{
				if ( _definition.SessionSettings.SpawnWithTools != value )
					_definition.SessionSettings.SpawnWithTools = value;
			}
		}

		/// <summary>
		/// Get or set the EnableJetpack setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Enable or disable jetpack." )]
		[Category( "World Settings" )]
		[DisplayName( "Enable Jetpack" )]
		[DefaultValue( true )]
		public bool EnableJetpack
		{
			get { return _definition.SessionSettings.EnableJetpack; }
			set
			{
				if ( _definition.SessionSettings.EnableJetpack != value )
					_definition.SessionSettings.EnableJetpack = value;
			}
		}

		/// <summary>
		/// Get or set the EnableSunRotation setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Enable or disable sun rotation." )]
		[Category( "World Settings" )]
		[DisplayName( "Sun Rotation Enabled" )]
		[DefaultValue( true )]
		public bool EnableSunRotation
		{
			get { return _definition.SessionSettings.EnableSunRotation; }
			set
			{
				if ( _definition.SessionSettings.EnableSunRotation != value )
					_definition.SessionSettings.EnableSunRotation = value;
			}
		}

		/// <summary>
		/// Get or set the SunRotationIntervalMinutes setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "Set the time, in minutes, it takes for the sun to make a complete rotation around the skybox. Only effective if sun rotation is enabled." )]
		[Category( "World Settings" )]
		[DisplayName( "Sun Rotation Interval" )]
		[DefaultValue( 240f )]
		public float SunRotationIntervalMinutes
		{
			get { return _definition.SessionSettings.SunRotationIntervalMinutes; }
			set
			{
				if ( _definition.SessionSettings.SunRotationIntervalMinutes != value )
					_definition.SessionSettings.SunRotationIntervalMinutes = value;
			}
		}

		/// <summary>
		/// Get or set the PhysicsIterations setting.
		/// </summary>
		[DataMember]
		[Browsable( true )]
		[ReadOnly( false )]
		[Description( "The number of iterations the physics engine uses per update. Be careful modifying this value, as lower values may cause physics instability (read: explosions and death), and higher values may significantly decrease performance." )]
		[Category( "World Settings" )]
		[DisplayName( "Physics Iterations" )]
		[DefaultValue( 4 )]
		public int PhysicsIterations
		{
			get { return _definition.SessionSettings.PhysicsIterations; }
			set
			{
					_definition.SessionSettings.PhysicsIterations = value;
			}
		}
        
        /// <summary>
		/// Get or set the Wolves setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables Wolves" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Wolves" )]
        [DefaultValue( false )]
        public bool EnableCyberhounds
        {
            get
            {
                return _definition.SessionSettings.EnableWolfs;
            }
            set
            {
                _definition.SessionSettings.EnableWolfs = value;
            }
        }
        
        /// <summary>
		/// Get or set the spiders setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables spiders" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Spiders" )]
        [DefaultValue( true )]
        public bool EnableSpiders
        {
            get
            {
                return _definition.SessionSettings.EnableSpiders;
            }
            set
            {
                _definition.SessionSettings.EnableSpiders = value;
            }
        }

        /// <summary>
		/// Get or set the drones setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables drones" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Drones" )]
        [DefaultValue( true )]
        public bool EnableDrones
        {
            get
            {
                return _definition.SessionSettings.EnableDrones;
            }
            set
            {
                _definition.SessionSettings.EnableDrones = value;
            }
        }

        /// <summary>
		/// Get or set the voxel destruction setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables voxel destruction" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Voxel Destruction" )]
        [DefaultValue( false )]
        public bool EnableVoxelDesctruction
        {
            get
            {
                return _definition.SessionSettings.EnableVoxelDestruction;
            }
            set
            {
                _definition.SessionSettings.EnableVoxelDestruction = value;
            }
        }

        /// <summary>
		/// Get or set the Enable Flora setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables flora" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Flora" )]
        [DefaultValue( true )]
        public bool EnableFlora
        {
            get
            {
                return _definition.SessionSettings.EnableFlora;
            }
            set
            {
                _definition.SessionSettings.EnableFlora = value;
            }
        }

        /// <summary>
		/// Get or set the Flora Density Multiplier setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Sets the Flora Density Multiplier setting" )]
        [Category( "World Settings" )]
        [DisplayName( "FloraDensityMultiplier" )]
        [DefaultValue( 1.00 )]
        public float FloraDensityMultiplier
        {
            get
            {
                return _definition.SessionSettings.FloraDensityMultiplier;
            }
            set
            {
                _definition.SessionSettings.FloraDensityMultiplier = value;
            }
        }

        /// <summary>
		/// Get or set the Flora Density setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Sets Flora Density" )]
        [Category( "World Settings" )]
        [DisplayName( "Flora Density" )]
        [DefaultValue( 20 )]
        public int FloraDensity
        {
            get
            {
                return _definition.SessionSettings.FloraDensity;
            }
            set
            {
                _definition.SessionSettings.FloraDensity = value;
            }
        }

	    /// <summary>
		/// Get or set the Convert to Station setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables Convert Ship to Station button." )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Convert to Station" )]
        [DefaultValue( true )]
        public bool EnableConvertToStation
        {
            get
            {
                return _definition.SessionSettings.EnableConvertToStation;
            }
            set
            {
                _definition.SessionSettings.EnableConvertToStation = value;
            }
        }

        
        /// <summary>
        /// Get or set the Voxel Support setting.
        /// </summary>
        [DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Enables or disables Voxel Support.")]
        [Category("World Settings")]
        [DisplayName("Enable Station Voxel Support")]
        [DefaultValue(true)]
        public bool EnableVoxelSupport
        {
            get
            {
                return _definition.SessionSettings.StationVoxelSupport;

                //FieldInfo memberInfo = _definition.SessionSettings.GetType().GetField("StationVoxelSupport", BindingFlags.Instance | BindingFlags.Public);
                //if ( memberInfo != null )
                //    return (bool)memberInfo.GetValue( _definition.SessionSettings );
                //return false;
            }
            set
            {
                _definition.SessionSettings.StationVoxelSupport = value;

                //FieldInfo memberInfo = _definition.SessionSettings.GetType().GetField("StationVoxelSupport", BindingFlags.Instance | BindingFlags.Public);
                //if (memberInfo != null)
                //    memberInfo.SetValue(_definition.SessionSettings, value);
            }
        }
        

        /// <summary>
        /// Get or set the Enable 3rd Person View setting.
        /// </summary>
        [DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables 3rd Person View" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable 3rd Person View" )]
        [DefaultValue( true )]
        public bool Enable3rdPersonView
        {
            get
            {
                return _definition.SessionSettings.Enable3rdPersonView;
            }
            set
            {
                _definition.SessionSettings.Enable3rdPersonView = value;
            }
        }

        /// <summary>
		/// Get or set the Enable Block Destruction setting.
		/// </summary>
		[DataMember]
        [Browsable( true )]
        [ReadOnly( false )]
        [Description( "Enables or disables Block Destruction" )]
        [Category( "World Settings" )]
        [DisplayName( "Enable Block Destruction" )]
        [DefaultValue( true )]
        public bool EnableBlockDestruction
        {
            get
            {
                return _definition.SessionSettings.DestructibleBlocks;
            }
            set
            {
                _definition.SessionSettings.DestructibleBlocks = value;
            }
        }

        /// <summary>
		/// Get or set the Enable Airtightness setting.
		/// </summary>
		[DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Enables or disables Airtightness.")]
        [Category("World Settings")]
        [DisplayName("Enable Airtightness")]
        [DefaultValue(true)]
        public bool EnableAirtightness
        {
            get { return _definition.SessionSettings.EnableOxygenPressurization; }
            set { _definition.SessionSettings.EnableOxygenPressurization = value; }
        }

	    [DataMember]
	    [Browsable(true)]
	    [ReadOnly(false)]
	    [Description("Enables or disables block limits.")]
	    [Category("Block limits")]
	    [DisplayName("Enable Block limits")]
	    [DefaultValue(true)]
	    public bool EnableBlockLimits
	    {
	        get
	        {
                return _definition.SessionSettings.EnableBlockLimits;
	        }

	        set
	        {
                _definition.SessionSettings.EnableBlockLimits = value;
            }
	    }

	    [DataMember]
	    [Browsable(true)]
	    [ReadOnly(false)]
	    [Description("Lets players delete blocks they own remotely.")]
	    [Category("Block limits")]
	    [DisplayName("Enable Remote Block Removal")]
	    [DefaultValue(true)]
	    public bool EnableRemoval
	    {
	        get
	        {
                return _definition.SessionSettings.EnableRemoteBlockRemoval;
            }

            set
            {
                _definition.SessionSettings.EnableRemoteBlockRemoval = value;
            }
        }

        [DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Max number of blocks per player.")]
        [Category("Block limits")]
        [DisplayName("Max Blocks Per Player")]
        [DefaultValue(true)]
        public int MaxBlocksPerPlayer
        {
            get { return _definition.SessionSettings.MaxBlocksPerPlayer; }
            set { _definition.SessionSettings.MaxBlocksPerPlayer = value; }
        }

        [DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Max number of blocks per grid.")]
        [Category("Block limits")]
        [DisplayName("Max Blocks Per Grid")]
        [DefaultValue(true)]
        public int MaxBlocksPerGrid
        {
            get { return _definition.SessionSettings.MaxGridSize; }
            set { _definition.SessionSettings.MaxGridSize = value; }
        }

        [Browsable(true)]
	    [ReadOnly(false)]
	    [Description("Opens a window to configure block limits.")]
	    [Category("Block limits")]
	    [DisplayName("Block limits")]
	    [Editor(typeof(LimitEditButton), typeof(UITypeEditor))]
	    public string BlockLimits
	    {
	        get { return "Press the button to edit settings ---->"; }
	    }

	    [DataMember]
	    [Browsable(true)]
	    [ReadOnly(false)]
	    [Description("Enables or disables the scripter role")]
	    [Category("World Settings")]
	    [DisplayName("Enable Scripter Role")]
	    [DefaultValue(true)]
	    public bool EnableScripterRole
	    {
	        get { return _definition.SessionSettings.EnableScripterRole; }

	        set { _definition.SessionSettings.EnableScripterRole = value; }
	    }

        // <summary>
        /// Get or set the server's description
        /// </summary>
        [DataMember]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Get or set the server description shown in server browser")]
        [Category("Server Settings")]
        [DisplayName("Server Description")]
        public string ServerDescription
        {
            get { return _definition.ServerDescription; }
            set
            {
                _definition.ServerDescription = value;
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
        public static MyConfigDedicatedData<MyObjectBuilder_SessionSettings> Load( FileInfo fileInfo )
        {
			string filePath = fileInfo.FullName;

			if ( !File.Exists( filePath ) )
			{
				throw new FileNotFoundException( "Game configuration file not found.", filePath );
			}

			try
            {
                using ( TextReader rdr = File.OpenText( filePath ) )
				{
					XmlSerializer deserializer = new XmlSerializer( typeof( MyConfigDedicatedData<MyObjectBuilder_SessionSettings> ) );
					MyConfigDedicatedData<MyObjectBuilder_SessionSettings> config = (MyConfigDedicatedData<MyObjectBuilder_SessionSettings>)deserializer.Deserialize( rdr );
                    if(config == null)
                        throw new Exception("Unknown Error");
					return config;
				}
			}
			catch ( Exception ex )
			{
				throw new ConfigurationErrorsException( "Configuration file not understood. See inner exception for details. Ignore configuration file line number in outer exception.", ex, filePath, -1 );
			}
		}

		public bool Save( FileInfo fileInfo )
		{
			if ( fileInfo == null ) return false;

            //hack
            //_definition.SessionSettings.BlockTypeLimits = new SerializableDictionary<string, short>(DedicatedConfigDefinition.Limits);
		    var dic = _definition.SessionSettings.GetType().GetField("BlockTypeLimits", BindingFlags.Public | BindingFlags.Instance);
            if(dic!=null)
                dic.SetValue(_definition.SessionSettings, new SerializableDictionary<string, short>(Limits));
            //Save the definitions container out to the file
            try
			{
				using ( XmlTextWriter xmlTextWriter = new XmlTextWriter( fileInfo.FullName, null ) )
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					XmlSerializer serializer = new XmlSerializer( typeof( MyConfigDedicatedData<MyObjectBuilder_SessionSettings> ) );
					serializer.Serialize( xmlTextWriter, _definition );
				}
			}
			catch(Exception ex)
			{
                BaseLog.Error( ex );
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
