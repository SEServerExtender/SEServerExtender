namespace SEServerExtender.ServerService
{
	using System;
	using System.Net.Mime;
	using System.Reflection;
	using System.ServiceModel;
	using SEModAPIInternal.API.Common;

	[ServiceBehavior( ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
	public class ServerService : IServerService
	{
		private static readonly Version ProtocolVersion = new Version( 1, 0, 0 );

		/// <summary>
		/// Starts the SE server with the given configuration name.
		/// </summary>
		/// <param name="request"></param>
		/// <remarks>Should not be called if a server is already running for this instance of SESE.</remarks>
		public StartServerResponse StartServer( StartServerRequest request )
		{
			if ( Program.ServerExtenderForm != null && Program.ServerExtenderForm.Visible )
			{
				//If the local GUI is running, indicate failure for that reason.
				return new StartServerResponse
					   {
						   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
						   ProtocolVersion = ProtocolVersion,
						   Status = "Not supported while GUI visible on server.",
						   StatusCode = 500
					   };
			}
			if ( Program.Server != null && Program.Server.Config != null )
			{
				Program.Server.InstanceName = request.ConfigurationName;
				SandboxGameAssemblyWrapper.Instance.InitMyFileSystem( request.ConfigurationName );

				Program.Server.LoadServerConfig( );
				Program.Server.SaveServerConfig( );

				Program.Server.StartServer( );

				return new StartServerResponse
					   {
						   StatusCode = 200,
						   Status = "OK",
						   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
						   ProtocolVersion = ProtocolVersion
					   };
			}

			return new StartServerResponse
				   {
					   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
					   ProtocolVersion = ProtocolVersion,
					   Status = "Unknown error attempting to start server.",
					   StatusCode = 500
				   };
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
