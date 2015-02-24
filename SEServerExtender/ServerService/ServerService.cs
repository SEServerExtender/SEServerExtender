namespace SEServerExtender.ServerService
{
	using System;
	using System.ServiceModel;
	using SEModAPIInternal.API.Common;

	[ServiceBehavior( ConcurrencyMode = ConcurrencyMode.Multiple, ConfigurationName = "ServerService", IncludeExceptionDetailInFaults = true )]
	class ServerService : IServerService
	{
		/// <summary>
		/// Starts the SE server with the given configuration name.
		/// </summary>
		/// <param name="configurationName"></param>
		/// <remarks>Should not be called if a server is already running for this instance of SESE.</remarks>
		public void StartServer( string configurationName )
		{
			if ( Program.ServerExtenderForm != null && Program.ServerExtenderForm.Visible )
			{
				Action<string> changeConfigDelegate = Program.ServerExtenderForm.ChangeConfigurationName;
				Program.ServerExtenderForm.Invoke( changeConfigDelegate, configurationName );

				Action<object, EventArgs> buttonClickDelegate = Program.ServerExtenderForm.BTN_ServerControl_Start_Click;
				Program.ServerExtenderForm.Invoke( buttonClickDelegate, this, new EventArgs( ) );
			}
			else
			{
				if ( Program.Server != null && Program.Server.Config != null )
				{
					Program.Server.InstanceName = configurationName;
					SandboxGameAssemblyWrapper.Instance.InitMyFileSystem( configurationName );

					Program.Server.LoadServerConfig( );
					Program.Server.SaveServerConfig( );

					Program.Server.StartServer( );
				}
			}
		}

		/// <summary>
		/// Stops the SE server, if it is running.
		/// </summary>
		public void StopServer( )
		{
			if ( Program.Server == null )
				return;
			if ( !Program.Server.IsRunning )
				return;
			if ( Program.ServerExtenderForm != null && Program.ServerExtenderForm.Visible )
			{
				Program.ServerExtenderForm.BTN_ServerControl_Stop_Click( this, new EventArgs( ) );
			}
			else
				Program.Server.StopServer( );
		}
	}
}
