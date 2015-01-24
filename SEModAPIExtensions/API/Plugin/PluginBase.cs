using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SEModAPIExtensions.API.Plugin
{
	public abstract class PluginBase : IPlugin
	{
		#region "Attributes"

		protected Guid m_pluginId;
		protected string m_name = "";
		protected string m_version = "";

		#endregion

		#region "Constructors and Initializers"

		protected PluginBase( )
		{
			Assembly assembly = Assembly.GetCallingAssembly( );
			GuidAttribute guidAttr = (GuidAttribute)assembly.GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ];
			m_pluginId = new Guid( guidAttr.Value );
			AssemblyName asmName = assembly.GetName( );
			m_name = asmName.Name;
			m_version = asmName.Version.ToString( );
		}

		#endregion

		#region "Properties"

		public Guid Id
		{
			get { return m_pluginId; }
		}

		public string Name
		{
			get { return m_name; }
		}

		public string Version
		{
			get { return m_version; }
		}

		#endregion

		#region "Methods"

		public abstract void Init( );
		//public void InitWithPath(String modPath)
		//{
		//}

		public abstract void Update( );
		public abstract void Shutdown( );

		#endregion
	}
}
