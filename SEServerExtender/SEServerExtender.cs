using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using SEServerExtender.Utility;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Ingame;
using VRage.Groups;
using IMyInventory = VRage.Game.ModAPI.IMyInventory;

namespace SEServerExtender
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using Sandbox;
	using Sandbox.Common;
    using Sandbox.Common.ObjectBuilders;
	using Sandbox.Definitions;
	using Sandbox.Game.Multiplayer;
	using Sandbox.Game.World;
	using SEModAPI.API;
	using SEModAPI.API.Definitions;
	using SEModAPI.API.Sandbox;
	using SEModAPI.API.Utility;
	using SEModAPI.Support;
	using SEModAPIExtensions.API;
	using SEModAPIExtensions.API.Plugin;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
	using SEModAPIInternal.Support;
	using VRage.ModAPI;
	using VRage.ObjectBuilders;
	using VRage.Utils;
	using VRageMath;
	using Timer = System.Windows.Forms.Timer;

	public sealed partial class SEServerExtender : Form
	{
		#region "Attributes"

		//General
		private static SEServerExtender m_instance;
		private readonly Server m_server;
		private HashSet<MyEntity> m_sectorEntities;
		private readonly List<MyCubeGrid> m_cubeGridEntities;
		private readonly List<MyCharacter> m_characterEntities;
		private readonly List<MyVoxelBase> m_voxelMapEntities;
        //floating object is MyEntity so we can count MyFloatingObjects and MyInventoryBagEntity
		private readonly List<MyEntity> m_floatingObjectEntities;
        //meteors are MyEntity because MyMeteor is private and IMyMeteor is empty
		private readonly List<MyEntity> m_meteorEntities;

		private int m_chatLineCount = 0;
		private int m_sortBy = 0;

		//Timers
		private Timer m_entityTreeRefreshTimer;
		private Timer m_chatViewRefreshTimer;
		private Timer m_factionRefreshTimer;
		private Timer m_pluginManagerRefreshTimer;
		private Timer m_statusCheckTimer;
		private Timer m_statisticsTimer;
		private Timer m_playersTimer;
		private Timer _genericUpdateTimer = new Timer { Interval = 1000 };

		//Utilities Page
		private int m_floatingObjectsCount;

		#endregion

		#region "Constructors and Initializers"

		public SEServerExtender( Server server )
		{
			m_instance = this;
			m_server = server;
			m_sectorEntities = new HashSet<MyEntity>( );
			m_cubeGridEntities = new List<MyCubeGrid>( );
			m_characterEntities = new List<MyCharacter>( );
			m_voxelMapEntities = new List<MyVoxelBase>( );
			m_floatingObjectEntities = new List<MyEntity>( );
			m_meteorEntities = new List<MyEntity>( );

			//Run init functionsS
			InitializeComponent( );
			if ( !SetupTimers( ) )
				Close( );
			if ( !SetupControls( ) )
				Close( );
			if ( !m_server.IsRunning )
				m_server.LoadServerConfig( );
			UpdateControls( );
			PG_Control_Server_Properties.SelectedObject = m_server.Config;

			//Update the title bar text with the assembly version
			Text = string.Format( "SEServerExtender {0}", Assembly.GetExecutingAssembly( ).GetName( ).Version );

			FormClosing += OnFormClosing;

			if ( m_server.IsRunning )
				_genericUpdateTimer.Start( );
		}

		private bool SetupTimers( )
		{
			m_entityTreeRefreshTimer = new Timer { Interval = 500 };
			m_entityTreeRefreshTimer.Tick += TreeViewRefresh;

			m_chatViewRefreshTimer = new Timer { Interval = 1000 };
			m_chatViewRefreshTimer.Tick += ChatViewRefresh;

			m_factionRefreshTimer = new Timer { Interval = 5000 };
			m_factionRefreshTimer.Tick += FactionRefresh;

			m_pluginManagerRefreshTimer = new Timer { Interval = 10000 };
			m_pluginManagerRefreshTimer.Tick += PluginManagerRefresh;

			m_statusCheckTimer = new Timer { Interval = 5000 };
			m_statusCheckTimer.Tick += StatusCheckRefresh;
			m_statusCheckTimer.Start( );

			m_statisticsTimer = new Timer { Interval = 1000 };
			m_statisticsTimer.Tick += StatisticsRefresh;

			_genericUpdateTimer.Tick += GenericTimerTick;

			/*
			m_playersTimer = new Timer { Interval = 2000 };
			m_playersTimer.Tick += PlayersRefresh;
			*/

			return true;
		}

		private void GenericTimerTick( object sender, EventArgs e )
		{
			SS_Bottom.Items[ 0 ].Text = string.Format( "Updates Per Second: {0}", WorldManager.GetUpdatesPerSecond( ) );
		}

		private bool SetupControls( )
		{
			try
			{
				if ( string.IsNullOrEmpty( m_server.CommandLineArgs.InstancePath ) )
				{
					List<string> instanceList = GameInstallationInfo.GetCommonInstanceList( );
					instanceList.Sort();
					CMB_Control_CommonInstanceList.BeginUpdate( );
					CMB_Control_CommonInstanceList.Items.AddRange( instanceList.ToArray( ) );
					if ( CMB_Control_CommonInstanceList.Items.Count > 0 )
						CMB_Control_CommonInstanceList.SelectedIndex = 0;
					CMB_Control_CommonInstanceList.EndUpdate( );
				}

				CB_Entity_Sort.SelectedIndex = 0;

				CMB_Control_AutosaveInterval.BeginUpdate( );
				CMB_Control_AutosaveInterval.Items.Add( 1 );
				CMB_Control_AutosaveInterval.Items.Add( 2 );
				CMB_Control_AutosaveInterval.Items.Add( 5 );
				CMB_Control_AutosaveInterval.Items.Add( 10 );
				CMB_Control_AutosaveInterval.Items.Add( 15 );
				CMB_Control_AutosaveInterval.Items.Add( 30 );
				CMB_Control_AutosaveInterval.Items.Add( 60 );
				CMB_Control_AutosaveInterval.EndUpdate( );
			}
			catch ( AutoException )
			{
				return false;
			}

			return true;
		}

		private void OnFormClosing( object sender, EventArgs e )
		{
			m_entityTreeRefreshTimer.Stop( );
			m_chatViewRefreshTimer.Stop( );
			m_factionRefreshTimer.Stop( );
			m_pluginManagerRefreshTimer.Stop( );
			m_statusCheckTimer.Stop( );
			m_statisticsTimer.Stop( );
			m_playersTimer.Stop( );
		}

		#endregion

		#region "Methods"

		#region "General"

		private void StatisticsRefresh( object sender, EventArgs e )
		{
			StringBuilder sb = new StringBuilder( );
			Stats.Generic.WriteTo( sb );
			Stats.Network.WriteTo( sb );
			Stats.Timing.WriteTo( sb );

			TB_Statistics.Text = sb.ToString( );
		}

		private void StatusCheckRefresh( object sender, EventArgs e )
		{
			UpdateControls( );

			if ( m_server.IsRunning )
			{
				if ( !m_entityTreeRefreshTimer.Enabled )
					m_entityTreeRefreshTimer.Start( );
				if ( !m_chatViewRefreshTimer.Enabled )
					m_chatViewRefreshTimer.Start( );
				if ( !m_factionRefreshTimer.Enabled )
					m_factionRefreshTimer.Start( );
				if ( !m_pluginManagerRefreshTimer.Enabled )
					m_pluginManagerRefreshTimer.Start( );

				if ( !m_statisticsTimer.Enabled )
					m_statisticsTimer.Start( );

				if ( PG_Control_Server_Properties.SelectedObject != m_server.Config )
					PG_Control_Server_Properties.SelectedObject = m_server.Config;
			}
		}

		#endregion

		#region "Control"

		internal void BTN_ServerControl_Start_Click( object sender, EventArgs e )
		{
			m_chatViewRefreshTimer.Start( );
			m_factionRefreshTimer.Start( );
			m_pluginManagerRefreshTimer.Start( );
			_genericUpdateTimer.Start( );

			if ( m_server.IsRunning )
				return;

			if ( m_server.Config != null )
			{
				//Enforce built-in auto-save being off
				if ( m_server.Config.AutoSave )
				{
					ApplicationLog.BaseLog.Warn( "SE built-in autosave was enabled in configuration. It has been disabled to prevent conflicts." );
					m_server.Config.AutoSave = false;
				}
				m_server.SaveServerConfig( );
			}

			m_server.StartServer( );
		}

		internal void BTN_ServerControl_Stop_Click( object sender, EventArgs e )
		{
			if ( m_entityTreeRefreshTimer.Enabled )
				m_entityTreeRefreshTimer.Stop( );
			m_chatViewRefreshTimer.Stop( );
			m_factionRefreshTimer.Stop( );
			m_pluginManagerRefreshTimer.Stop( );
			_genericUpdateTimer.Stop( );

			m_server.StopServer( );
		}

		private void CHK_Control_Debugging_CheckedChanged( object sender, EventArgs e )
		{
			ExtenderOptions.IsDebugging = CHK_Control_Debugging.CheckState == CheckState.Checked;
		}

		private void CHK_Control_CommonDataPath_CheckedChanged( object sender, EventArgs e )
		{
			ExtenderOptions.UseCommonProgramData = CHK_Control_CommonDataPath.CheckState == CheckState.Checked;
			CMB_Control_CommonInstanceList.Enabled = ExtenderOptions.UseCommonProgramData;

			m_server.InstanceName = ExtenderOptions.UseCommonProgramData ? CMB_Control_CommonInstanceList.Text : string.Empty;

			m_server.LoadServerConfig( );

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void BTN_Control_Server_Reset_Click( object sender, EventArgs e )
		{
			//Refresh the loaded config
			m_server.LoadServerConfig( );
			UpdateControls( );

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void BTN_Control_Server_Save_Click( object sender, EventArgs e )
		{
			if ( m_server.ServerHasRan )
				m_server.Config = (DedicatedConfigDefinition)PG_Control_Server_Properties.SelectedObject;

			//Save the loaded config
			m_server.SaveServerConfig( );
			UpdateControls( );
		}

		private void PG_Control_Server_Properties_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			UpdateControls( );
		}

		internal void ChangeConfigurationName( string configurationName )
		{
			CMB_Control_CommonInstanceList.SelectedItem = configurationName;
		}

		private void CMB_Control_CommonInstanceList_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( !CMB_Control_CommonInstanceList.Enabled || CMB_Control_CommonInstanceList.SelectedIndex == -1 ) return;

			m_server.InstanceName = CMB_Control_CommonInstanceList.Text;
			FileSystem.InitMyFileSystem( CMB_Control_CommonInstanceList.Text );

			m_server.LoadServerConfig( );

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void CMB_Control_AutosaveInterval_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( !CMB_Control_AutosaveInterval.Enabled || CMB_Control_AutosaveInterval.SelectedIndex == -1 ) return;

			double interval;
			if ( !double.TryParse( CMB_Control_AutosaveInterval.Text, out interval ) )
				MessageBox.Show( this, "Invalid input for auto-save interval." );

			if ( interval < 1 )
				interval = 2;

			m_server.AutosaveInterval = interval * 60000;
		}

		private void UpdateControls( )
		{
			if ( m_server.Config == null )
				ExtenderOptions.UseCommonProgramData = true;

			if ( m_server.InstanceName.Length != 0 )
			{
				CHK_Control_CommonDataPath.Checked = true;
				foreach ( object item in CMB_Control_CommonInstanceList.Items )
				{
					if ( item.ToString( ).Equals( m_server.InstanceName ) )
					{
						CMB_Control_CommonInstanceList.SelectedItem = item;
						break;
					}
				}
			}

			CHK_Control_Debugging.Checked = ExtenderOptions.IsDebugging;

			if ( !CMB_Control_CommonInstanceList.ContainsFocus && m_server.InstanceName.Length > 0 )
				CMB_Control_CommonInstanceList.SelectedText = m_server.InstanceName;

			BTN_ServerControl_Stop.Enabled = m_server.IsRunning;
			BTN_ServerControl_Start.Enabled &= !m_server.IsRunning;
			BTN_Chat_Send.Enabled = m_server.IsRunning;
			if ( !m_server.IsRunning )
				BTN_Entities_New.Enabled = false;

			if ( CHK_Control_CommonDataPath.CheckState == CheckState.Checked )
				CMB_Control_CommonInstanceList.Enabled = !m_server.IsRunning;
			else
				CMB_Control_CommonInstanceList.Enabled = false;

			TXT_Chat_Message.Enabled = m_server.IsRunning;

			PG_Entities_Details.Enabled = m_server.IsRunning;
			PG_Factions.Enabled = m_server.IsRunning;
			PG_Plugins.Enabled = m_server.IsRunning;

			if ( m_server.Config != null )
			{
				if ( string.IsNullOrEmpty( m_server.CommandLineArgs.InstancePath ) && CMB_Control_CommonInstanceList.Items.Count > 0 )
					CHK_Control_CommonDataPath.Enabled = !m_server.IsRunning;
				else
					CHK_Control_CommonDataPath.Enabled = false;
			}
			else
			{
				CHK_Control_CommonDataPath.Checked = true;

				//				BTN_Control_Server_Save.Enabled = false;
				//				BTN_Control_Server_Reset.Enabled = false;
			}

			if ( !m_server.IsRunning )
			{
				//BTN_Plugins_Refresh.Enabled = false;
				//BTN_Plugins_Load.Enabled = false;
				//BTN_Plugins_Unload.Enabled = false;
				BTN_Plugins_Reload.Enabled = false;
				BTN_Plugins_Enable.Enabled = false;
			}
			else
			{
				//BTN_Plugins_Refresh.Enabled = true;
				BTN_Plugins_Reload.Enabled = true;
				BTN_Plugins_Enable.Enabled = true;
			}

			if ( !CMB_Control_AutosaveInterval.ContainsFocus )
				CMB_Control_AutosaveInterval.SelectedItem = (int)Math.Round( m_server.AutosaveInterval / 60000.0 );
		}

		#endregion

		#region "Entities"

		private void UpdateNodeInventoryItemBranch<T>( TreeNode node, List<T> source )
			where T : IMyInventoryItem
		{
			try
			{
				bool entriesChanged = ( node.Nodes.Count != source.Count );
				if ( entriesChanged )
				{
					node.Nodes.Clear( );
					node.Text = string.Format( "{0} ({1})", node.Name, source.Count );
				}

				int index = 0;
				foreach ( T item in source )
				{
					TreeNode itemNode;
					if ( entriesChanged )
					{
						itemNode = node.Nodes.Add( item.GetDefinitionId().SubtypeId.ToString() );
						itemNode.Tag = item;
					}
					else
					{
						itemNode = node.Nodes[ index ];
						itemNode.Text = item.GetDefinitionId().SubtypeId.ToString();
						itemNode.Tag = item;
					}

					index++;
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private void TreeViewRefresh( object sender, EventArgs e )
		{
			m_entityTreeRefreshTimer.Enabled = false;

			try
			{
				if ( !MySandboxGameWrapper.IsGameStarted )
					return;

				if ( TAB_MainTabs.SelectedTab != TAB_Entities_Page )
					return;

				TRV_Entities.BeginUpdate( );

				TreeNode sectorObjectsNode;
				//TreeNode sectorEventsNode;

				if ( TRV_Entities.Nodes.Count < 1 )
				{
					sectorObjectsNode = TRV_Entities.Nodes.Add( "Sector Objects" );
					//sectorEventsNode = TRV_Entities.Nodes.Add( "Sector Events" );

					sectorObjectsNode.Name = sectorObjectsNode.Text;
					//sectorEventsNode.Name = sectorEventsNode.Text;
				}
				else
				{
					sectorObjectsNode = TRV_Entities.Nodes[ 0 ];
					//sectorEventsNode = TRV_Entities.Nodes[ 1 ];
				}

				RenderSectorObjectChildNodes( sectorObjectsNode );
				sectorObjectsNode.Text = $"{sectorObjectsNode.Name} ({m_sectorEntities.Count})";
				sectorObjectsNode.Tag = SectorObjectManager.Instance;


				TRV_Entities.EndUpdate( );
			}
			finally
			{
				m_entityTreeRefreshTimer.Interval = 500;
				m_entityTreeRefreshTimer.Enabled = true;
			}
		}

		private void RenderSectorObjectChildNodes( TreeNode objectsNode )
		{
			if ( TRV_Entities.IsDisposed )
				return;

			TreeNode cubeGridsNode;
			TreeNode charactersNode;
			TreeNode voxelMapsNode;
			TreeNode floatingObjectsNode;
			TreeNode meteorsNode;

			if ( objectsNode.Nodes.Count < 5 )
			{
				objectsNode.Nodes.Clear( );

				cubeGridsNode = objectsNode.Nodes.Add( "Cube Grids" );
				charactersNode = objectsNode.Nodes.Add( "Characters" );
				voxelMapsNode = objectsNode.Nodes.Add( "Voxel Maps" );
				floatingObjectsNode = objectsNode.Nodes.Add( "Floating Objects" );
				meteorsNode = objectsNode.Nodes.Add( "Meteors" );

				cubeGridsNode.Name = cubeGridsNode.Text;
				charactersNode.Name = charactersNode.Text;
				voxelMapsNode.Name = voxelMapsNode.Text;
				floatingObjectsNode.Name = floatingObjectsNode.Text;
				meteorsNode.Name = meteorsNode.Text;
			}
			else
			{
				cubeGridsNode = objectsNode.Nodes[ 0 ];
				charactersNode = objectsNode.Nodes[ 1 ];
				voxelMapsNode = objectsNode.Nodes[ 2 ];
				floatingObjectsNode = objectsNode.Nodes[ 3 ];
				meteorsNode = objectsNode.Nodes[ 4 ];
			}
            
		    SandboxGameAssemblyWrapper.Instance.GameAction(() => m_sectorEntities = MyEntities.GetEntities() );
            MyEntity[] entitiesCopy = new MyEntity[m_sectorEntities.Count];
            m_sectorEntities.CopyTo( entitiesCopy );
			//m_sectorEntities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
			//foreach ( BaseEntity entry in m_sectorEntities )
            foreach(MyEntity entity in entitiesCopy)
			{
			    if (entity is MyCubeGrid)
			        m_cubeGridEntities.Add(entity as MyCubeGrid);
                else if (entity is MyCharacter)
                    m_characterEntities.Add(entity as MyCharacter);
                else if(entity is MyVoxelBase)
                    m_voxelMapEntities.Add(entity as MyVoxelBase);
                else if (entity is MyFloatingObject || entity is MyInventoryBagEntity)
                    m_floatingObjectEntities.Add(entity);
                else if (entity is IMyMeteor)
                    m_meteorEntities.Add(entity);
			}

			RenderCubeGridNodes( cubeGridsNode );
			RenderCharacterNodes( charactersNode );
			RenderVoxelMapNodes( voxelMapsNode );
			RenderFloatingObjectNodes( floatingObjectsNode );
			RenderMeteorNodes( meteorsNode );
		}

		private void RenderCubeGridNodes( TreeNode rootNode )
		{
			if ( rootNode == null )
				return;

			//Get cube grids
			List<MyCubeGrid> list = m_cubeGridEntities;
			SortCubeGrids( list );

			//Cleanup and update the existing nodes
			foreach ( TreeNode node in rootNode.Nodes )
			{
				try
				{
					if ( node == null )
						continue;
					if ( node.Tag == null )
					{
						node.Remove( );
						continue;
					}

					MyCubeGrid item = node.Tag as MyCubeGrid;
				    if ( item == null )
				    {
				        node.Remove();
				        continue;
				    }

				    bool foundMatch = false;
					foreach ( MyCubeGrid listItem in list )
					{
					    if ( listItem == null )
					        continue;

						if ( listItem.EntityId == item.EntityId )
						{
							foundMatch = true;
							string newNodeText = GenerateCubeNodeText( item );
							node.Text = newNodeText;
							list.Remove( listItem );

							break;
						}
					}

					if ( !foundMatch )
					{
						node.Remove( );
						continue;
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Add new nodes
			foreach ( MyCubeGrid item in list )
			{
				try
				{
					if ( item == null )
						continue;

					string nodeKey = item.EntityId.ToString( );

					TreeNode newNode = rootNode.Nodes.Add( nodeKey, GenerateCubeNodeText( item ) );
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Update node text
			rootNode.Text = string.Format( "{0} ({1})", rootNode.Name, rootNode.Nodes.Count );
		}

		private string GenerateCubeNodeText( MyCubeGrid item )
		{
			string text = item.DisplayName;
		        
			int sortBy = CB_Entity_Sort.SelectedIndex;
			switch ( sortBy )
			{
				case 0:
					//text += $" | {item.Name}";
					break;
				case 1:
                    text += $" | {(string.IsNullOrEmpty(item.GetOwner()) ? "No Owner" : $"Owner: {item.GetOwner()}")}";
                    break;
                case 2:
			        text += $" | Blocks: {item.BlocksCount}";
			        break;
				case 4:
					text += $" | Mass: {(item.Physics.IsStatic ? "[Station]" : Math.Floor( item.Physics.Mass ) + "kg")}";
					break;
                case 5:
			        text += $" | EntityID: {item.EntityId}";
			        break;
			}

			text += string.Format( " | Dist: {0}m", Math.Round( ( (Vector3D)item.PositionComp.GetPosition() ).Length( ), 0 ) );

			return text;
		}

		public void SortCubeGrids( List<MyCubeGrid> list )
		{
			int sortBy = CB_Entity_Sort.SelectedIndex;

			if ( sortBy == 0 ) // Display Name
			{
				list.Sort( ( x, y ) => x.DisplayName.CompareTo( y.DisplayName ) );
			}
			else if ( sortBy == 1 ) // Owner Name
			{
                list.Sort((x, y) =>
               {
                   string yOwn = y.GetOwner();
                   string xOwn = x.GetOwner();
                   if (String.IsNullOrEmpty(yOwn) && !String.IsNullOrEmpty(xOwn))
                   {
                       return -1;
                   }
                   else if (!String.IsNullOrEmpty(yOwn) && String.IsNullOrEmpty(xOwn))
                   {
                       return 1;
                   }
                   else
                   {
                       return String.Compare(xOwn, yOwn);
                   }
               });
            }
			else if ( sortBy == 2 ) // Block Count
			{
                list.Sort((x,y) => x.BlocksCount.CompareTo( y.BlocksCount ));
			}
			else if ( sortBy == 3 ) // Distance from center
            {
                list.Sort((x, y) =>
                   {
                       if (x == null || x.Closed)
                           return -1;

                       if (y == null || y.Closed)
                           return 1;

                       return Vector3D.Distance(x.PositionComp.GetPosition(), Vector3D.Zero).CompareTo(Vector3D.Distance(y.PositionComp.GetPosition(), Vector3D.Zero));
                   });
            }
            else if ( sortBy == 4 ) // Weight
            {
                list.Sort( ( x, y ) =>
                {
                    if ( x?.Physics == null && y?.Physics != null )
                        return 1;
                    else if ( x?.Physics != null && y?.Physics == null )
                        return -1;
                    else if ( x?.Physics == null && y?.Physics == null )
                        return 1;
                    else if (x?.Physics!=null && y?.Physics!=null)
                    {
                        if ( x.Physics.IsStatic && !y.Physics.IsStatic )
                            return 1;
                        else if ( !x.Physics.IsStatic && y.Physics.IsStatic )
                            return -1;
                        else if ( x.Physics.IsStatic && y.Physics.IsStatic )
                            return 1;
                        else if ( !x.Physics.IsStatic && !y.Physics.IsStatic )
                        {
                            if ( Math.Floor( x.Physics.Mass ) > Math.Floor( y.Physics.Mass ) )
                                return 1;
                            else
                                return -1;
                        }
                    }
                    return 1;
                });
            }
            else if (sortBy == 5 ) // Entity ID
            {
                list.Sort( ( x, y ) => x.EntityId.CompareTo( y.EntityId ));
            }
		}

		private void RenderCharacterNodes( TreeNode rootNode )
		{
			if ( rootNode == null )
				return;

			//Get entities from sector object manager
			List<MyCharacter> list = m_characterEntities;

			//Cleanup and update the existing nodes
			foreach ( TreeNode node in rootNode.Nodes )
			{
				try
				{
					if ( node == null )
						continue;

					if ( node.Tag != null && list.Contains( node.Tag ) )
					{
						MyCharacter item = (MyCharacter)node.Tag;

						if ( !item.Closed )
						{
							Vector3D rawPosition = item.PositionComp.GetPosition();
							double distance = Math.Round( rawPosition.Length( ), 0 );
							string newNodeText = $"{item.DisplayName} | Dist: {distance}m";
							node.Text = newNodeText;
						}
						list.Remove( item );
					}
					else
					{
						node.Remove( );
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Add new nodes
			foreach ( MyCharacter item in list )
			{
				try
				{
					if ( item == null )
						continue;

					Vector3D rawPosition = item.PositionComp.GetPosition();
					double distance = rawPosition.Length( );

					string nodeKey = item.EntityId.ToString( );

					TreeNode newNode = rootNode.Nodes.Add( nodeKey, $"{item.DisplayName} | Dist: {distance}m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Update node text
			rootNode.Text = string.Format( "{0} ({1})", rootNode.Name, rootNode.Nodes.Count );
		}

		private void RenderVoxelMapNodes( TreeNode rootNode )
		{
			if ( rootNode == null )
				return;

			//Get entities from sector object manager
			List<MyVoxelBase> list = m_voxelMapEntities;

			//Cleanup and update the existing nodes
			foreach ( TreeNode node in rootNode.Nodes )
			{
				try
				{
					if ( node == null )
						continue;
                    
				    if ( node.Tag != null && list.Contains( node.Tag ) )
					{
						MyVoxelBase item = (MyVoxelBase)node.Tag;

						if ( !item.Closed )
						{
							Vector3D rawPosition = item.PositionComp.GetPosition();
							double distance = Math.Round( rawPosition.Length( ), 0 );
							string newNodeText = $"{item.AsteroidName} | Dist: {distance}m";
							node.Text = newNodeText;
						}
						list.Remove( item );
					}
					else
					{
						node.Remove( );
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Add new nodes
			foreach ( MyVoxelBase item in list )
			{
				try
				{
					if ( item == null )
						continue;

					Vector3D rawPosition = item.PositionComp.GetPosition();
					double distance = rawPosition.Length( );

                    string nodeKey = item.EntityId.ToString( );

					TreeNode newNode = rootNode.Nodes.Add( nodeKey, $"{item.AsteroidName} | Dist: {distance}m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}
            
			//Update node text
			rootNode.Text = string.Format( "{0} ({1})", rootNode.Name, rootNode.Nodes.Count );
		}

		private void RenderFloatingObjectNodes( TreeNode rootNode )
		{
			if ( rootNode == null )
				return;
            
            //items in this list can be MyFloatingObject or MyInvenoryBagEntity
			List<MyEntity> list = m_floatingObjectEntities;

			//Cleanup and update the existing nodes
			foreach ( TreeNode node in rootNode.Nodes )
			{
				try
				{
					if ( node == null )
						continue;

					if ( node.Tag != null && list.Contains( node.Tag ) )
					{
						MyEntity item = (MyEntity)node.Tag;

					    if (item != null && !item.Closed)
					    {
					        Vector3D rawPosition = item.PositionComp.GetPosition();
					        double distance = Math.Round(rawPosition.Length(), 0);
					        string newNodeText = "";

                            if (item is MyFloatingObject)
					        {
					            var myFloating = (MyFloatingObject) item;
					            newNodeText = $"{myFloating.Item.Content.SubtypeName} | Amount: {myFloating.Item.Amount} | Dist: {distance}m";
					        }
                            else if (item is MyInventoryBagEntity)
                            {
                                var myBag = (MyInventoryBagEntity)item;
                                newNodeText = $"{myBag.DisplayName} | Mass: {myBag.GetInventory( ).CurrentMass} | Dist: {distance}m";
                            }
					        node.Text = newNodeText;
					    }
					    list.Remove( item );
					}
					else
					{
						node.Remove( );
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Add new nodes
			foreach ( MyEntity item in list )
			{
				try
				{
					if ( item == null )
						continue;
					if ( item.Closed )
						continue;

					Vector3D rawPosition = item.PositionComp.GetPosition();
					double distance = rawPosition.Length( );

					string nodeKey = item.EntityId.ToString( );

					TreeNode newNode;
				    if (item is MyFloatingObject)
				    {
				        var myFloating = (MyFloatingObject) item;
				        newNode = rootNode.Nodes.Add(nodeKey, $"{myFloating.Item.Content.SubtypeName} | Amount: {myFloating.Item.Amount} | Dist: {distance}m");
				    }
				    else
				    {
				        var myBag = (MyInventoryBagEntity) item;
				        newNode = rootNode.Nodes.Add(nodeKey, $"{myBag.DisplayName} | Mass: {myBag.GetInventory( ).CurrentMass} | Dist: {distance}m");
				    }
				    newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Update node text
			rootNode.Text = string.Format( "{0} ({1})", rootNode.Name, rootNode.Nodes.Count );

			// Update a var for the Utilities Floating object cleaner.
			m_floatingObjectsCount = rootNode.Nodes.Count;
		}

		private void RenderMeteorNodes( TreeNode rootNode )
		{
			if ( rootNode == null )
				return;

			//Get entities from sector object manager
			List<MyEntity> list = m_meteorEntities;

			//Cleanup and update the existing nodes
			foreach ( TreeNode node in rootNode.Nodes )
			{
				try
				{
					if ( node == null )
						continue;

					if ( node.Tag != null && list.Contains( node.Tag ) )
					{
						MyEntity item = (MyEntity)node.Tag ;
                        
						if ( !item.Closed )
						{
							Vector3D rawPosition = item.PositionComp.GetPosition();
							double distance = Math.Round( rawPosition.Length( ), 0 );
							string newNodeText = string.Format( "{0} | Dist: {1}m", item.DisplayName, distance );
							node.Text = newNodeText;
						}
						list.Remove( item );
					}
					else
					{
						node.Remove( );
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Add new nodes
			foreach ( MyEntity item in list )
			{
				try
				{
					if ( item == null )
						continue;

					Vector3D rawPosition = item.PositionComp.GetPosition();
					double distance = rawPosition.Length( );

					string nodeKey = item.EntityId.ToString( );
					TreeNode newNode = rootNode.Nodes.Add( nodeKey, string.Format( "{0} | Dist: {1}m", item.Name, distance ) );
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}

			//Update node text
			rootNode.Text = string.Format( "{0} ({1})", rootNode.Name, rootNode.Nodes.Count );
		}

            private DateTime renderTimer;
		private void RenderCubeGridChildNodes( MyCubeGrid cubeGrid, TreeNode blocksNode )
		{
            renderTimer=DateTime.Now;
		    Dictionary<string, MyGuiBlockCategoryDefinition> categories = MyDefinitionManager.Static.GetCategories();
            var taskResults = new List<Task<KeyValuePair<string, HashSet<MySlimBlock>>>>();

            //process the categories in parallel because there are 14 categories minimum
		    foreach ( var category in categories )
		    {
               taskResults.Add( Task.Run( () => ProcessBlockCategories( cubeGrid,category ) ) );
		    }

		    Task.WaitAll( taskResults.ToArray() );
            if (ExtenderOptions.IsDebugging)
                ApplicationLog.BaseLog.Info("TaskTime: " + (DateTime.Now - renderTimer));

            foreach (var task in taskResults)
		    {
                //don't create an empty category
		        if ( task.Result.Value.Count == 0 )
		            continue;

		        //see if this category already exists, if not create it
		        TreeNode categoryNode;
		        var searchNodes = blocksNode.Nodes.Find(task.Result.Key, false);
		        if (searchNodes.Length == 0)
		        {
		            categoryNode = blocksNode.Nodes.Add(task.Result.Key);
		            categoryNode.Name = task.Result.Key;
		        }
		        else
		        {
		            categoryNode = searchNodes[0];
		        }


		        //add the blocks to the tree structure
                //cargo containers are lumped in with conveyors, let's create a separate category for those
		        if ( task.Result.Key == "Conveyors" )
		        {
		            int cargoCount = 0;
		            int conveyorCount = 0;
                    //see if this category already exists, if not create it
                    TreeNode cargoNode;
                    var cargoSearchNodes = blocksNode.Nodes.Find("Cargo Containers", false);
                    if (cargoSearchNodes.Length == 0)
                    {
                        cargoNode = blocksNode.Nodes.Add("Cargo Containers");
                        cargoNode.Name = "Cargo Containers";
                    }
                    else
                    {
                        cargoNode = cargoSearchNodes[0];
                    }

                    foreach (MySlimBlock block in task.Result.Value)
                    {
                        TreeNode cubeNode;
                        if ( block.FatBlock == null )
                        {
                            cubeNode = categoryNode.Nodes.Add( block.BlockDefinition.Id.SubtypeName );
                            cubeNode.Name = cubeNode.Text;
                            cubeNode.Tag = block;
                        }
                        else
                        {
                            if ( block.FatBlock.HasInventory )
                            {
                                cargoCount++;
                                cubeNode = cargoNode.Nodes.Add( block.FatBlock.DisplayNameText );
                            }

                            else
                            {
                                conveyorCount++;
                                cubeNode = categoryNode.Nodes.Add( block.FatBlock.DisplayNameText );
                            }

                            cubeNode.Name = cubeNode.Text;
                            cubeNode.Tag = block.FatBlock;
                        }
                    }
		            categoryNode.Text = $"{categoryNode.Name} ({conveyorCount})";
		            if ( cargoCount > 0 )
		                cargoNode.Text =$"{cargoNode.Text} ({cargoCount})";
		        }
                    else
                {
                    //update node text with the count of blocks in this category
                    categoryNode.Text = $"{categoryNode.Name} ({task.Result.Value.Count})";
                    foreach ( MySlimBlock block in task.Result.Value )
		            {
		                TreeNode cubeNode;
		                if ( block.FatBlock == null )
		                {
		                    cubeNode = categoryNode.Nodes.Add( block.BlockDefinition.Id.SubtypeName );
		                    cubeNode.Name = cubeNode.Text;
		                    cubeNode.Tag = block;
		                }
		                else
		                {
		                    cubeNode = categoryNode.Nodes.Add( block.FatBlock.DisplayNameText );
		                    cubeNode.Name = cubeNode.Text;
		                    cubeNode.Tag = block.FatBlock;
		                }
		            }
		        }
		    }

		    taskResults.Clear();
            
            if(ExtenderOptions.IsDebugging)
                ApplicationLog.BaseLog.Info( "RenderNodeTime: " + (DateTime.Now - renderTimer) );
            
		}

	    private KeyValuePair<string, HashSet<MySlimBlock>> ProcessBlockCategories( MyCubeGrid grid, KeyValuePair<string,MyGuiBlockCategoryDefinition> definition )
	    {
            //DateTime processTime = DateTime.Now;
            var blockResult = new HashSet<MySlimBlock>();

	        foreach ( MySlimBlock block in grid.CubeBlocks )
	        {
	            if ( definition.Value.HasItem( block.BlockDefinition.Id.ToString() )) 
	                blockResult.Add( block );
            }
            /*
	        if ( ExtenderOptions.IsDebugging )
            {
                ApplicationLog.BaseLog.Info("RenderNodeTime: " + (DateTime.Now - renderTimer));
                ApplicationLog.BaseLog.Info($"Process{definition.Key}Time: {(DateTime.Now - processTime)}");
            }
            */
	        return new KeyValuePair<string, HashSet<MySlimBlock>>(definition.Key, blockResult);
	    }

		private void TRV_Entities_NodeRefresh( object sender, TreeNodeMouseClickEventArgs e )
		{
			if ( e.Clicks < 2 )
				return;
			if ( e.Node == null )
				return;
			if ( e.Node.Tag == null )
				return;

			//Clear the child nodes
			e.Node.Nodes.Clear( );

			//Call the main node select event handler to populate the node
			TreeViewEventArgs newEvent = new TreeViewEventArgs( e.Node );
			TRV_Entities_AfterSelect( sender, newEvent );
		}

		private void TRV_Entities_AfterSelect( object sender, TreeViewEventArgs e )
		{
			BTN_Entities_Export.Enabled = false;
			BTN_Entities_New.Enabled = false;
			BTN_Entities_Delete.Enabled = false;
			btnRepairEntity.Enabled = false;

			TreeNode selectedNode = e.Node;

			if ( selectedNode == null )
				return;

			TreeNode parentNode = e.Node.Parent;

			if ( parentNode == null )
				return;

			if ( parentNode.Tag is SectorObjectManager )
			{
				if ( selectedNode == parentNode.Nodes[ 0 ] )
				{
					//BTN_Entities_New.Enabled = true;
				}
			}

			if ( parentNode.Tag is MyCubeGrid )
			{
				//BTN_Entities_New.Enabled = false;
			}

			if ( selectedNode.Tag == null )
				return;

			object linkedObject = selectedNode.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;


            var grid = linkedObject as MyCubeGrid;
		    if (grid != null)
		    {
		        //BTN_Entities_Export.Enabled = true;
		        btnRepairEntity.Enabled = true;
		        BTN_Entities_Delete.Enabled = true;

		        TRV_Entities.BeginUpdate();

		        RenderCubeGridChildNodes(grid, e.Node);

		        TRV_Entities.EndUpdate();
		        return;
		    }

            /*maybe one day...
		    var map = linkedObject as MyVoxelBase;
			if ( map != null )
			{
				List<MyVoxelMaterialDefinition> materialDefs = new List<MyVoxelMaterialDefinition>( MyDefinitionManager.Static.GetVoxelMaterialDefinitions( ) );

				ThreadPool.QueueUserWorkItem( state =>
											 {
												 Dictionary<MyVoxelMaterialDefinition, float> totalMaterials = voxelMap.Materials;

												 Invoke( new Action( ( ) =>
																		{
																			TRV_Entities.BeginUpdate( );
																			if ( e.Node.Nodes.Count < materialDefs.Count )
																			{
																				e.Node.Nodes.Clear( );

																				foreach ( MyVoxelMaterialDefinition material in materialDefs )
																				{
																					TreeNode newNode = e.Node.Nodes.Add( material.Id.SubtypeName );
																					newNode.Name = newNode.Text;
																					newNode.Tag = material;
																				}
																			}

																			foreach ( TreeNode node in e.Node.Nodes )
																			{
																				Object tag = node.Tag;
																				if ( !( tag is MyVoxelMaterialDefinition ) )
																					continue;
																				MyVoxelMaterialDefinition material = (MyVoxelMaterialDefinition)tag;
																				float total;
																				if ( totalMaterials.TryGetValue( material, out total ) )
																				{
																					node.Text = string.Format( "{0} ({1})", node.Name, total );
																				}
																			}

																			TRV_Entities.EndUpdate( );
																		} ) );
											 } );

			}
            */

			var character = linkedObject as MyCharacter;
			if ( character != null )
			{
				if ( e.Node.Nodes.Count < 1 )
				{
					TRV_Entities.BeginUpdate( );

					e.Node.Nodes.Clear( );
					TreeNode itemsNode = e.Node.Nodes.Add( "Items" );
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = character.GetInventory();

					TRV_Entities.EndUpdate( );
				}
			    return;
			}

            var block = linkedObject as MyCubeBlock;
            if (block != null)
            {
                btnRepairEntity.Enabled = true;
                BTN_Entities_Delete.Enabled = true;
            }

		    var cockpit = linkedObject as MyCockpit;
			if ( cockpit?.Pilot != null)
			{
				if ( e.Node.Nodes.Count < 1 )
				{
					TRV_Entities.BeginUpdate( );

					e.Node.Nodes.Clear( );
					TreeNode node = e.Node.Nodes.Add( "Pilot" );
					node.Name = node.Text;
					node.Tag = cockpit.Pilot;

					TRV_Entities.EndUpdate( );
				}
				else
				{
					TRV_Entities.BeginUpdate( );

					TreeNode node = e.Node.Nodes[ 0 ];
					node.Tag = cockpit.Pilot;

					TRV_Entities.EndUpdate( );
				}
			    return;
			}
            
			var inventory = linkedObject as MyInventory;
			if ( inventory != null )
			{
				BTN_Entities_New.Enabled = true;
                
				UpdateNodeInventoryItemBranch( e.Node, inventory.GetItems() );

			    return;
			}
            
			if ( linkedObject is MyPhysicalInventoryItem )
			{
				BTN_Entities_New.Enabled = true;
				BTN_Entities_Delete.Enabled = true;

			    return;
			}

            var entity = linkedObject as MyEntity;
            if (entity != null && entity.HasInventory)
            {
                BTN_Entities_Delete.Enabled = true;
                if (entity.InventoryCount == 1)
                {
                    if (e.Node.Nodes.Count < 1)
                    {
                        TRV_Entities.BeginUpdate();

                        e.Node.Nodes.Clear();
                        TreeNode itemsNode = e.Node.Nodes.Add("Inventory");
                        itemsNode.Name = itemsNode.Text;
                        itemsNode.Tag = entity.GetInventory();

                        TRV_Entities.EndUpdate();
                    }
                }
                if (entity.InventoryCount == 2)     //production blocks
                {
                    if (e.Node.Nodes.Count < 2)
                    {
                        TRV_Entities.BeginUpdate();

                        e.Node.Nodes.Clear();
                        TreeNode inputNode = e.Node.Nodes.Add("Input");
                        inputNode.Name = inputNode.Text;
                        inputNode.Tag = entity.GetInventory(0);
                        TreeNode outputNode = e.Node.Nodes.Add("Output");
                        outputNode.Name = outputNode.Text;
                        outputNode.Tag = entity.GetInventory(1);

                        TRV_Entities.EndUpdate();
                    }
                }
            }
		}

        private void BTN_Entities_Delete_Click( object sender, EventArgs e )
		{
			try
			{
				object linkedObject = TRV_Entities.SelectedNode.Tag;
                
                //if object is an inventory item, remove it from the inventory in the parent node
			    if (linkedObject is MyPhysicalInventoryItem)
			    {
			        var item = (MyPhysicalInventoryItem) linkedObject;
			        var inventory = TRV_Entities.SelectedNode.Parent.Tag as MyInventory;
			        SandboxGameAssemblyWrapper.Instance.GameAction(() => inventory?.Remove(item, item.Amount));
			    }

                /*
                //raze blocks on grid
			    var block = linkedObject as MyCubeBlock;
			    if (block != null)
			    {
			       SandboxGameAssemblyWrapper.Instance.GameAction( ()=>block.CubeGrid.RazeBlock(block.Position));
			        return;
			    }
                */

                //entities can just be Closed
			    var entity = linkedObject as MyEntity;
			    if (entity != null)
			    {
			        if (entity.Closed)
			        {
			            ApplicationLog.BaseLog.Info("Object cannot be deleted.");
			            return;
			        }
			        SandboxGameAssemblyWrapper.Instance.GameAction(() => entity.Close());
			    }

			    TreeNode parentNode = TRV_Entities.SelectedNode.Parent;
				TRV_Entities.SelectedNode.Tag = null;
				TreeNode newSelectedNode = ( TRV_Entities.SelectedNode.NextVisibleNode ?? TRV_Entities.SelectedNode.PrevVisibleNode ) ?? parentNode.FirstNode;

				TRV_Entities.SelectedNode.Remove( );
				if ( newSelectedNode != null )
				{
					TRV_Entities.SelectedNode = newSelectedNode;
					PG_Entities_Details.SelectedObject = newSelectedNode.Tag;
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				MessageBox.Show( ex.ToString( ) );
			}
		}

		private void BTN_Entities_New_Click( object sender, EventArgs e )
		{
			try
			{
				TreeNode selectedNode = TRV_Entities.SelectedNode;

				if ( selectedNode == null )
					return;

				TreeNode parentNode = selectedNode.Parent;

				if ( parentNode == null )
					return;

			    object linkedObject = TRV_Entities.SelectedNode.Tag;

			    var inventory = linkedObject as MyInventory;
			    if ( inventory!=null)
			    {
                    InventoryItemDialog newItemDialog = new InventoryItemDialog { InventoryContainer = inventory };
                    newItemDialog.ShowDialog(this);

                    TreeViewEventArgs newEvent = new TreeViewEventArgs(selectedNode);
                    TRV_Entities_AfterSelect(sender, newEvent);

                    return;
                }

			    if (linkedObject is MyPhysicalInventoryItem)
			    {
                    InventoryItemDialog newItemDialog = new InventoryItemDialog { InventoryContainer = (MyInventory)parentNode.Tag };
                    newItemDialog.ShowDialog(this);

                    TreeViewEventArgs newEvent = new TreeViewEventArgs(selectedNode);
                    TRV_Entities_AfterSelect(sender, newEvent);
                }

                /*
				SectorObjectManager sectorObjectManager = parentNode.Tag as SectorObjectManager;
				if ( sectorObjectManager != null )
				{
					if ( selectedNode == parentNode.Nodes[ 0 ] )
					{
						CreateCubeGridImportDialog( );
						return;
					}
				}

				CubeGridEntity cubeGridEntity = parentNode.Tag as CubeGridEntity;
				if ( cubeGridEntity != null )
				{
					CubeBlockDialog dialog = new CubeBlockDialog { ParentCubeGrid = cubeGridEntity };
					dialog.ShowDialog( this );
					return;
				}

				if ( selectedNode.Tag == null )
					return;

				if ( !( selectedNode.Tag is BaseObject ) )
					return;

				BaseObject linkedObject = (BaseObject)selectedNode.Tag;

				InventoryEntity inventoryEntity = linkedObject as InventoryEntity;
				if ( inventoryEntity != null )
				{
					InventoryItemDialog newItemDialog = new InventoryItemDialog { InventoryContainer = inventoryEntity };
					newItemDialog.ShowDialog( this );

					TreeViewEventArgs newEvent = new TreeViewEventArgs( selectedNode );
					TRV_Entities_AfterSelect( sender, newEvent );

					return;
				}

				InventoryItemEntity inventoryItemEntity = linkedObject as InventoryItemEntity;
				if ( inventoryItemEntity != null )
				{
					InventoryItemDialog newItemDialog = new InventoryItemDialog { InventoryContainer = inventoryItemEntity.Container };
					newItemDialog.ShowDialog( this );

					TreeViewEventArgs newEvent = new TreeViewEventArgs( parentNode );
					TRV_Entities_AfterSelect( sender, newEvent );

					return;
				}

				if ( linkedObject is CubeGridEntity )
				{
					CreateCubeGridImportDialog( );
					return;
				}*/
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private void BTN_Entities_Export_Click( object sender, EventArgs e )
		{
			try
			{
				if ( TRV_Entities.SelectedNode == null )
					return;
				Object linkedObject = TRV_Entities.SelectedNode.Tag;
				
				if ( linkedObject == null )
					return;
				ApplicationLog.BaseLog.Trace( "Object Type: {0}", linkedObject.GetType( ).Name );
				if ( !( linkedObject is BaseObject ) )
					return;

				BaseObject objectToExport = (BaseObject)linkedObject;

				SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "sbc file (*.sbc)|*.sbc|All files (*.*)|*.*", InitialDirectory = GameInstallationInfo.GamePath };

				if ( saveFileDialog.ShowDialog( ) == DialogResult.OK )
				{
					FileInfo fileInfo = new FileInfo( saveFileDialog.FileName );
					try
					{
						MyObjectBuilderSerializer.SerializeXML( fileInfo.FullName, false, ( (IMyEntity) objectToExport.BackingObject ).GetObjectBuilder( ) );
						objectToExport.Export( fileInfo );
					}
					catch ( Exception ex )
					{
						ApplicationLog.BaseLog.Error( ex );
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private void PG_Entities_Details_Click( object sender, EventArgs e )
		{
			TreeNode node = TRV_Entities.SelectedNode;
			if ( node == null )
				return;
			object linkedObject = node.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;
		}

		private void CreateCubeGridImportDialog( )
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					InitialDirectory = GameInstallationInfo.GamePath,
					DefaultExt = "sbc file (*.sbc)"
				};

				if ( openFileDialog.ShowDialog( this ) == DialogResult.OK )
				{
					FileInfo fileInfo = new FileInfo( openFileDialog.FileName );
					if ( fileInfo.Exists )
					{
						try
						{
							CubeGridEntity cubeGrid = new CubeGridEntity( fileInfo );

							SectorObjectManager.Instance.AddEntity( cubeGrid );
						}
						catch ( Exception ex )
						{
							ApplicationLog.BaseLog.Error( ex );
						}
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private void CB_Entity_Sort_SelectionChangeCommitted( object sender, EventArgs e )
		{
			TreeNode cubeNode = TRV_Entities.Nodes.Find( "Cube Grids", true ).FirstOrDefault( );
			if ( cubeNode == null )
				return;

			m_sortBy = CB_Entity_Sort.SelectedIndex;
			cubeNode.Nodes.Clear( );
			TreeViewEventArgs newEvent = new TreeViewEventArgs( cubeNode );
			TRV_Entities_AfterSelect( sender, newEvent );

		}

		#endregion

		#region "Chat"

		private void ChatViewRefresh( object sender, EventArgs e )
		{
			//Refresh the chat history
			List<ChatManager.ChatEvent> chatHistory = ChatManager.Instance.ChatHistory;
			if ( chatHistory.Count != m_chatLineCount )
			{
				int pos = 0;
				foreach ( ChatManager.ChatEvent entry in chatHistory )
				{
					if ( pos >= m_chatLineCount )
					{

						string timestamp = entry.Timestamp.ToLongTimeString( );
						string playerName = "Server";
						if ( entry.SourceUserId != 0 )
							playerName = PlayerMap.Instance.GetPlayerNameFromSteamId( entry.SourceUserId );
						string formattedMessage = timestamp + " - " + playerName + " - " + entry.Message + "\r\n";
						RTB_Chat_Messages.AppendText( formattedMessage );
					}

					pos++;
				}

				m_chatLineCount = chatHistory.Count;
			}

			//Refresh the connected players list
			LST_Chat_ConnectedPlayers.BeginUpdate( );
			List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;

			if ( connectedPlayers.Count != LST_Chat_ConnectedPlayers.Items.Count || CheckRequireNameUpdate( ) )
			{
				int selected = LST_Chat_ConnectedPlayers.SelectedIndex;
				LST_Chat_ConnectedPlayers.DataSource = null;
				LST_Chat_ConnectedPlayers.Items.Clear( );
				List<ChatUserItem> connectedPlayerList = new List<ChatUserItem>( );
				foreach ( ulong remoteUserId in connectedPlayers )
				{
					ChatUserItem item = new ChatUserItem( );
					string playerName = PlayerMap.Instance.GetPlayerNameFromSteamId( remoteUserId );

					item.Username = playerName;
					item.SteamId = remoteUserId;
					connectedPlayerList.Add( item );
				}

				LST_Chat_ConnectedPlayers.DataSource = connectedPlayerList;
				LST_Chat_ConnectedPlayers.DisplayMember = "Username";

				if ( selected >= connectedPlayerList.Count && connectedPlayerList.Count > 0 )
					LST_Chat_ConnectedPlayers.SelectedIndex = 0;
				else
					LST_Chat_ConnectedPlayers.SelectedIndex = selected;

			}
			LST_Chat_ConnectedPlayers.EndUpdate( );
		}

		private bool CheckRequireNameUpdate( )
		{
			foreach ( object item in LST_Chat_ConnectedPlayers.Items )
			{
				ChatUserItem chatItem = (ChatUserItem)item;

				if ( chatItem.SteamId.ToString( ) == chatItem.Username )
					return true;
			}

			return false;
		}

		private void BTN_Chat_Send_Click( object sender, EventArgs e )
		{
			string message = TXT_Chat_Message.Text;
			if ( !string.IsNullOrEmpty( message ) )
			{
				ChatManager.Instance.SendPublicChatMessage( message );
				TXT_Chat_Message.Text = string.Empty;
			}
		}

		private void TXT_Chat_Message_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Enter )
			{
				string message = TXT_Chat_Message.Text;
				if ( !string.IsNullOrEmpty( message ) )
				{
					ChatManager.Instance.SendPublicChatMessage( message );
					TXT_Chat_Message.Text = string.Empty;
				}
			}
		}

		private void BTN_Chat_KickSelected_Click( object sender, EventArgs e )
		{
			if ( LST_Chat_ConnectedPlayers.SelectedItem != null )
			{
				ChatUserItem item = (ChatUserItem)LST_Chat_ConnectedPlayers.SelectedItem;
				ChatManager.Instance.SendPublicChatMessage( string.Format( "/kick {0}", item.SteamId ) );
			}
		}

		private void BTN_Chat_BanSelected_Click( object sender, EventArgs e )
		{
			if ( LST_Chat_ConnectedPlayers.SelectedItem != null )
			{
				ChatUserItem item = (ChatUserItem)LST_Chat_ConnectedPlayers.SelectedItem;
				ChatManager.Instance.SendPublicChatMessage( string.Format( "/ban {0}", item.SteamId ) );
			}
		}
		#endregion

		#region "Factions"

		private void FactionRefresh( object sender, EventArgs e )
		{
			try
			{
				if ( MySandboxGameWrapper.IsGameStarted )
				{
					TRV_Factions.BeginUpdate( );

					List<MyFaction> list = MySession.Static.Factions.Select( f => f.Value ).ToList( );

					//Cleanup and update the existing nodes
					foreach ( TreeNode node in TRV_Factions.Nodes )
					{
						try
						{
							if ( node == null )
								continue;
							if ( node.Tag == null )
							{
								node.Remove( );
								continue;
							}

							MyFaction item = (MyFaction)node.Tag;
							bool foundMatch = false;
							foreach ( MyFaction faction in list )
							{
								if ( faction.FactionId == item.FactionId )
								{
									foundMatch = true;

									string newNodeText = string.Format( "{0} ({1})", item.Name, item.Members.Count() );
									node.Text = newNodeText;

									TreeNode membersNode = node.Nodes[ 0 ];
									TreeNode joinRequestsNode = node.Nodes[ 1 ];

									if ( membersNode.Nodes.Count != item.Members.Count() )
									{
										membersNode.Nodes.Clear( );
										foreach ( MyFactionMember member in item.Members.Select( m=>m.Value ) )
										{
											TreeNode memberNode = membersNode.Nodes.Add( member.PlayerId.ToString( ), member.PlayerId.ToString( ) );
											memberNode.Name = member.PlayerId.ToString( );
											memberNode.Tag = member;
										}
									}
									if ( joinRequestsNode.Nodes.Count != item.JoinRequests.Count() )
									{
										joinRequestsNode.Nodes.Clear( );
										foreach ( MyFactionMember member in item.JoinRequests.Select( j=>j.Value ) )
										{
											TreeNode joinRequestNode = joinRequestsNode.Nodes.Add( member.PlayerId.ToString( ), member.PlayerId.ToString( ) );
											joinRequestNode.Name = member.PlayerId.ToString( );
											joinRequestNode.Tag = member;
										}
									}

									list.Remove( faction );

									break;
								}
							}

							if ( !foundMatch )
							{
								node.Remove( );
								continue;
							}
						}
						catch ( Exception ex )
						{
							ApplicationLog.BaseLog.Error( ex );
						}
					}

					//Add new nodes
					foreach ( MyFaction item in list )
					{
						try
						{
							if ( item == null )
								continue;

							string nodeKey = item.FactionId.ToString( );

							TreeNode newNode = TRV_Factions.Nodes.Add( nodeKey, string.Format( "{0} ({1})", item.Name, item.Members.Count( ) ) );
							newNode.Name = item.Name;
							newNode.Tag = item;

							TreeNode membersNode = newNode.Nodes.Add( "Members" );
							TreeNode joinRequestsNode = newNode.Nodes.Add( "Join Requests" );

							foreach ( MyFactionMember member in item.Members.Select( m => m.Value ) )
							{
								TreeNode memberNode = membersNode.Nodes.Add( member.PlayerId.ToString( ), member.PlayerId.ToString( ) );
								memberNode.Name = member.PlayerId.ToString( );
								memberNode.Tag = member;
							}
							foreach ( MyFactionMember member in item.JoinRequests.Select( j => j.Value ) )
							{
								TreeNode memberNode = membersNode.Nodes.Add( member.PlayerId.ToString( ), member.PlayerId.ToString( ) );
								memberNode.Name = member.PlayerId.ToString( );
								memberNode.Tag = member;
							}
						}
						catch ( Exception ex )
						{
							ApplicationLog.BaseLog.Error( ex );
						}
					}

					TRV_Factions.EndUpdate( );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private void TRV_Factions_AfterSelect( object sender, TreeViewEventArgs e )
		{
			BTN_Factions_Delete.Enabled = false;

			if ( e.Node == null )
				return;
			if ( e.Node.Tag == null )
				return;

			object linkedObject = e.Node.Tag;

			BTN_Factions_Delete.Enabled = true;

			PG_Factions.SelectedObject = linkedObject;

			//DEBUG
			if ( e.Node.Text.Equals( "Join Requests" ) )
				BTN_Factions_Delete.Enabled = false;
		}

		private void BTN_Factions_Delete_Click( object sender, EventArgs e )
		{
			TreeNode node = TRV_Factions.SelectedNode;
			if ( node == null )
				return;
			if ( node.Tag == null )
				return;

			object linkedObject = node.Tag;

			MyFaction faction = linkedObject as MyFaction;
			if ( faction != null )
			{
				MyFactionCollection.RemoveFaction( faction.FactionId );
			}
			if ( linkedObject is MyFactionMember )
			{
				MyFactionMember factionMember = (MyFactionMember) linkedObject;
				MySession.Static.Factions.KickPlayerFromFaction( factionMember.PlayerId );
			}
		}

		#endregion

		#region "Plugins"

		private void PluginManagerRefresh( object sender, EventArgs e )
		{
			if ( PluginManager.Instance.Initialized )
			{
				if ( PluginManager.Instance.Plugins.Count == LST_Plugins.Items.Count )
					return;

				int selectedIndex = LST_Plugins.SelectedIndex;
				if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
					return;

				LST_Plugins.BeginUpdate( );
				LST_Plugins.Items.Clear( );
				foreach ( Guid key in PluginManager.Instance.Plugins.Keys )
				{
					IPlugin plugin = PluginManager.Instance.Plugins[ key ];
					LST_Plugins.Items.Add( string.Format( "{0} - {1}", plugin.Name, plugin.Version.ToString( 4 ) ) );
				}
				LST_Plugins.SelectedIndex = selectedIndex;
				LST_Plugins.EndUpdate( );
			}
		}

		private void LST_Plugins_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( LST_Plugins.SelectedItem == null )
				return;

			int selectedIndex = LST_Plugins.SelectedIndex;
			if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
				return;

			Guid selectedItem = PluginManager.Instance.Plugins.Keys.ElementAt( selectedIndex );
			Object plugin = PluginManager.Instance.Plugins[ selectedItem ];

			// This section allows plugins to have a customized settings form inside the settings
			// panel of a plugin.  
			AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			//foreach (var type in Assembly.GetAssembly(plugin.GetType()).GetTypes()) //??
			//{
			PropertyInfo info = plugin.GetType( ).GetProperty( "PluginControlForm" );
			if ( info != null )
			{
				PG_Plugins.Visible = false;
				Form value = (Form)info.GetValue( plugin, null );

				foreach ( Control control in SC_Plugins.Panel2.Controls )
				{
					if ( control.Visible )
						control.Visible = false;
				}

				if ( !SC_Plugins.Panel2.Controls.Contains( value ) )
				{
					value.TopLevel = false;
					SC_Plugins.Panel2.Controls.Add( value );
				}

				value.Dock = DockStyle.Fill;
				value.FormBorderStyle = FormBorderStyle.None;
				value.Visible = true;
			}
			else // Default PropertyGrid view
			{
				foreach ( Control ctl in SC_Plugins.Panel2.Controls )
				{
					if ( ctl.Visible )
					{
						ctl.Visible = false;
					}
				}

				PG_Plugins.Visible = true;
				PG_Plugins.SelectedObject = plugin;
			}
			//}

			// Set state
			bool pluginState = PluginManager.Instance.GetPluginState( selectedItem );
			if ( pluginState )
			{
				BTN_Plugins_Reload.Enabled = true;
				BTN_Plugins_Enable.Text = "Disable";
			}
			else
			{
				BTN_Plugins_Reload.Enabled = false;
				BTN_Plugins_Enable.Text = "Enable";
			}
		}

		/// <summary>
		/// If a plugin uses reference .dlls and puts them in their plugin dir, the appdomain won't
		/// know where to find those dlls as the path won't be resolvable.  So we can just scan here
		/// and return the assembly if we have it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		{
			string modsPath = Path.Combine( Server.Instance.Path, "Mods" );
			string[ ] subDirectories = Directory.GetDirectories( modsPath );
			foreach ( string path in subDirectories )
			{
				string[ ] files = Directory.GetFiles( path );
				foreach ( string file in files )
				{
					FileInfo fileInfo = new FileInfo( file );
					if ( !fileInfo.Extension.ToLower( ).Equals( ".dll" ) )
						continue;

					string[ ] names = args.Name.Split( new char[ ] { ',' } );
					if ( !fileInfo.Name.ToLower( ).Equals( names[ 0 ].ToLower( ).Trim( ) + ".dll" ) )
						continue;

					byte[ ] b = File.ReadAllBytes( file );
					return Assembly.Load( b );
				}
			}

			return null;
		}

		private void BTN_Plugins_Reload_Click( object sender, EventArgs e )
		{
			if ( LST_Plugins.SelectedItem == null )
				return;

			int selectedIndex = LST_Plugins.SelectedIndex;
			if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
				return;

			Guid selectedPluginId = PluginManager.Instance.Plugins.Keys.ElementAt( selectedIndex );
			IPlugin plugin = PluginManager.Instance.Plugins[ selectedPluginId ];
			PluginManager.Instance.UnloadPlugin( selectedPluginId );
			PluginManager.Instance.LoadPlugins( true );
			PluginManager.Instance.InitPlugin( selectedPluginId );
			LST_Plugins.Items[ LST_Plugins.SelectedIndex ] = string.Format( "{0} - {1}", plugin.Name, plugin.Version.ToString( 4 ) );
			LST_Plugins_SelectedIndexChanged( this, EventArgs.Empty );
		}

		private void BTN_Plugins_Enable_Click( object sender, EventArgs e )
		{
			if ( LST_Plugins.SelectedItem == null )
				return;

			int selectedIndex = LST_Plugins.SelectedIndex;
			if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
				return;

			Guid selectedItem = PluginManager.Instance.Plugins.Keys.ElementAt( selectedIndex );
			bool pluginState = PluginManager.Instance.GetPluginState( selectedItem );
			if ( pluginState )
			{
				PluginManager.Instance.UnloadPlugin( selectedItem );
				PluginManager.Instance.LoadPlugins( true );
			}
			else
			{
				PluginManager.Instance.LoadPlugins( true );
				PluginManager.Instance.InitPlugin( selectedItem );
			}

			LST_Plugins_SelectedIndexChanged( this, EventArgs.Empty );
		}

		private void BTN_Plugins_Unload_Click( object sender, EventArgs e )
		{
			if ( LST_Plugins.SelectedItem == null )
				return;

			int selectedIndex = LST_Plugins.SelectedIndex;
			if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
				return;

			Guid selectedItem = PluginManager.Instance.Plugins.Keys.ElementAt( selectedIndex );
			PluginManager.Instance.UnloadPlugin( selectedItem );
		}

		private void BTN_Plugins_Load_Click( object sender, EventArgs e )
		{
			if ( LST_Plugins.SelectedItem == null )
				return;

			int selectedIndex = LST_Plugins.SelectedIndex;
			if ( selectedIndex >= PluginManager.Instance.Plugins.Count )
				return;

			Guid selectedItem = PluginManager.Instance.Plugins.Keys.ElementAt( selectedIndex );
			PluginManager.Instance.InitPlugin( selectedItem );
		}

		private void BTN_Plugins_Refresh_Click( object sender, EventArgs e )
		{
			PluginManager.Instance.LoadPlugins( true );
		}

		#endregion

		private void TSM_Kick_Click( object sender, EventArgs e )
		{
			if ( LST_Chat_ConnectedPlayers.SelectedItem != null )
			{
				ChatUserItem item = (ChatUserItem)LST_Chat_ConnectedPlayers.SelectedItem;
				ChatManager.Instance.SendPublicChatMessage( string.Format( "/kick {0}", item.SteamId ) );
			}
		}

		#endregion

		private void TSM_Ban_Click( object sender, EventArgs e )
		{
			if ( LST_Chat_ConnectedPlayers.SelectedItem != null )
			{
				ChatUserItem item = (ChatUserItem)LST_Chat_ConnectedPlayers.SelectedItem;
				ChatManager.Instance.SendPublicChatMessage( string.Format( "/ban {0}", item.SteamId ) );
			}
		}

		/// <summary>
		/// Repairs the selected <see cref="MyCubeBlock"/> or <see cref="MyCubeGrid"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRepairEntity_Click( object sender, EventArgs e )
		{
			bool previousExportButtonState = BTN_Entities_Export.Enabled;
			bool previousNewButtonState = BTN_Entities_New.Enabled;
			bool previousDeleteButtonState = BTN_Entities_Delete.Enabled;
			BTN_Entities_Export.Enabled = false;
			BTN_Entities_New.Enabled = false;
			BTN_Entities_Delete.Enabled = false;
			btnRepairEntity.Enabled = false;

			TreeNode selectedNode = TRV_Entities.SelectedNode;

			if ( selectedNode == null )
			{
				MessageBox.Show( this, "Cannot repair that." );
				return;
			}

			TreeNode parentNode = TRV_Entities.SelectedNode.Parent;

			if ( parentNode == null )
			{
				MessageBox.Show( this, "Cannot repair that." );
				return;
			}

			if ( selectedNode.Tag == null )
			{
				MessageBox.Show( this, "Cannot repair that." );
				return;
			}

			object linkedObject = selectedNode.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;

			MyCubeGrid grid = linkedObject as MyCubeGrid;
			if ( grid != null )
			{
				grid.Repair( );

				TRV_Entities.BeginUpdate( );

				RenderCubeGridChildNodes( grid, selectedNode );

				TRV_Entities.EndUpdate( );
			}

			MySlimBlock block = linkedObject as MySlimBlock;
			if ( block != null )
			{
				block.Repair( );
			}

			BTN_Entities_Export.Enabled = previousExportButtonState;
			BTN_Entities_New.Enabled = previousNewButtonState;
			BTN_Entities_Delete.Enabled = previousDeleteButtonState;
			btnRepairEntity.Enabled = true;
		}

		private void TAB_MainTabs_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( TAB_MainTabs.SelectedTab.Name == "TAB_Entities_Page" )
			{
				m_entityTreeRefreshTimer.Start( );
			}
			else
			{
				m_entityTreeRefreshTimer.Stop( );
			}
		}

		private void PG_Entities_Details_Click( object s, PropertyValueChangedEventArgs e )
		{

		}

		private void PG_Entities_Details_Click( object sender, SelectedGridItemChangedEventArgs e )
		{

		}
	}
}
