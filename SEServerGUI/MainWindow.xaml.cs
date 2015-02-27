using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SEServerGUI
{
	using System.Data;
	using SEComm;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow( )
		{
			InitializeComponent( );
		}

		private void StopServer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			proxy.StopServer( );
		}

		private void StartServer( object sender, RoutedEventArgs e )
		{
			ServerServiceProxy proxy = new ServerServiceProxy( );
			proxy.StartServer( new StartServerRequest { ConfigurationName = "Temporal Engineering", ProtocolVersion = new Version( 1, 0, 0 ) } );
		}

		private void ExitServer( object sender, RoutedEventArgs e )
		{
			try
			{
				ServerServiceProxy proxy = new ServerServiceProxy( );
				proxy.Exit( 0 );
			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.Message );
			}
		}
	}
}
