namespace SEServerGUI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Timers;
	using System.Windows;
	using System.Windows.Controls;
	using SEComm;
	using SEComm.Plugins;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Chat;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Guid chatSessionGuid;
		Timer chatRefreshTimer = new Timer { AutoReset = true, Enabled = false, Interval = 2000, };
		public MainWindow( )
		{
			InitializeComponent( );
			ServerServiceProxy proxy = new ServerServiceProxy( );
			chatSessionGuid = proxy.BeginChatSession( );
			proxy.Close( );
			proxy.InnerChannel.Dispose( );
			chatRefreshTimer.Elapsed += chatRefreshTimer_Elapsed;
			chatRefreshTimer.Start(  );
		}

		void chatRefreshTimer_Elapsed( object sender, ElapsedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			IEnumerable<ChatMessage> chatMessages = proxy.GetChatMessages( chatSessionGuid );
			proxy.Close(  );
			proxy.InnerChannel.Dispose( );
			foreach ( ChatMessage m in chatMessages )
			{
				ChatHistoryTextBox.Dispatcher.Invoke( ( ) => ChatHistoryTextBox.AppendText( string.Format( "\r\n{0} - {1} - {2}", m.Timestamp, m.User, m.Message ) ) );
			}
		}

		private void StopServer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			proxy.StopServer( );
			proxy.Close( );
			proxy.InnerChannel.Dispose( );
		}

		private void StartServer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			proxy.StartServer( new StartServerRequest { ConfigurationName = "", ProtocolVersion = new Version( 1, 0, 0 ) } );
			proxy.Close( );
			proxy.InnerChannel.Dispose( );
		}

		private void ExitServer( object sender, RoutedEventArgs e )
		{
			try
			{
				ServerServiceProxy proxy = new ServerServiceProxy( );
				proxy.Exit( 0 );
				proxy.Close( );
				proxy.InnerChannel.Dispose( );
			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.Message );
			}
		}

		private void ReloadChatPlayerList( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			List<ChatUserItem> players = proxy.GetPlayersOnline( ).ToList( );
			proxy.Close( );
			proxy.InnerChannel.Dispose( );
			if ( PlayerList.Dispatcher.CheckAccess( ) )
			{
				PlayerList.Items.Clear( );
				foreach ( ChatUserItem player in players )
				{
					PlayerList.Items.Add( new ListViewItem { Content = string.Format( "{0} ({1})", player.Username, player.SteamId ), Tag = player } );
				}
			}
			else
			{
				PlayerList.Dispatcher.Invoke( ( ) =>
											  {
												  PlayerList.Items.Clear( );
												  foreach ( ChatUserItem player in players )
												  {
													  PlayerList.Items.Add( new ListViewItem { Content = string.Format( "{0} ({1})", player.Username, player.SteamId ), Tag = player } );
												  }
											  } );
			}
		}

		private void KickPlayer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy p = new ServerServiceProxy( );
			p.KickPlayer( ( (ChatUserItem)( (ListViewItem)PlayerList.SelectedItem ).Tag ).SteamId );
			p.Close( );
			p.InnerChannel.Dispose( );
		}

		private void BanPlayer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy p = new ServerServiceProxy( );
			p.BanPlayer( ( (ChatUserItem)( (ListViewItem)PlayerList.SelectedItem ).Tag ).SteamId );
			p.Close( );
			p.InnerChannel.Dispose( );
		}

		private void TabChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( ( (TabControl)sender ).SelectedIndex == 1 )
			{
				Task.Run( ( ) => ReloadChatPlayerList( sender, e ) );
			}
		}

		private void RefreshPluginList( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy p = new ServerServiceProxy( );
			IEnumerable<PluginInfo> plugins = p.GetLoadedPluginList( );
			p.Close( );
			p.InnerChannel.Dispose( );

			LoadedPlugins.Items.Clear( );

			foreach ( var plugin in plugins )
			{
				LoadedPlugins.Items.Add( string.Format( "{0} - {1}", plugin.Name, plugin.Version ) );
			}
		}

		private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			ServerServiceProxy p = new ServerServiceProxy( );
			p.EndChatSession( chatSessionGuid );
			p.Close( );
			p.InnerChannel.Dispose( );
		}
	}
}
