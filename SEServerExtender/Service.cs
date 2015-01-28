using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace SEServerExtender
{
	[RunInstaller(true)]
	public class WindowsServiceInstaller : Installer
	{
		private readonly ServiceInstaller _serviceInstaller;

		public WindowsServiceInstaller()
		{
			ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
			_serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.LocalSystem;
			_serviceInstaller.StartType = ServiceStartMode.Manual;
			_serviceInstaller.ServiceName = "SEServerExtender";
			_serviceInstaller.Description = "Service for running SEServerExtender";

			Installers.Add(_serviceInstaller);
			Installers.Add(processInstaller);
		}

		public override void Install(IDictionary stateSaver)
		{
			RetrieveServiceName();
			base.Install(stateSaver);
		}

		public override void Uninstall(IDictionary savedState)
		{
			RetrieveServiceName();
			base.Uninstall(savedState);
		}

		private void RetrieveServiceName()
		{
			string nameParam = Context.Parameters["name"];
			if (string.IsNullOrEmpty(nameParam))
				return;

			_serviceInstaller.ServiceName = nameParam;
			_serviceInstaller.DisplayName = nameParam;
		}
	}
}
