namespace SEModAPIExtensions.API.IPC
{
	using System;
	using System.IdentityModel.Selectors;
	using System.ServiceModel;

	public class UserNameValidator : UserNamePasswordValidator
	{
		public override void Validate(string userName, string password)
		{
			if (null == userName || null == password)
			{
				throw new ArgumentNullException();
			}

			//TODO - Create a dynamic password system or abandon this and just use windows username+password instead
			if (!(userName == "test1" && password == "1tset"))
			{
				throw new FaultException("Unknown Username or Incorrect Password");
			}
		}
	}
}
