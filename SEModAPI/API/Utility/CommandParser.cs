namespace SEModAPI.API.Utility
{
	using System.Collections.Generic;
	using System.Text;

	public static class CommandParser
	{
		public static List<string> GetCommandParts( string input )
		{
			bool quote = false;
			List<string> output = new List<string>( );
			StringBuilder sb = new StringBuilder( );
			for ( int pos = 0; pos < input.Length; pos++ )
			{
				char character = input[ pos ];

				if ( character == '"' )
				{
					if ( !quote )
					{
						quote = true;
					}
					else if ( pos + 1 < input.Length && input[ pos + 1 ] == '"' )
					{
						sb.Append( character );
						pos++;
					}
					else
					{
						quote = false;
					}
				}
				else if ( char.IsWhiteSpace( character ) && !quote )
				{
					output.Add( sb.ToString( ) );
					sb.Clear( );
				}
				else
				{
					sb.Append( character );
				}
			}

			if ( !quote && sb.Length > 0 )
			{
				output.Add( sb.ToString( ) );
			}
			return output;
		}
	}
}
