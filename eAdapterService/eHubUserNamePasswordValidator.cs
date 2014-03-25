using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CargoWise.eHub.Common;

namespace OTE.eAdapter
{
	internal class eHubUserNamePasswordValidator : UserNamePasswordValidator
	{
		public override void Validate(string userName, string password)
		{
            if (!ValidatePassword(userName, SHA512Encryptor.Encrypt(userName + password)))
            {
                throw new FaultException("ClientID or Password invalid.");
            }
		}

		private bool ValidatePassword(string userName, string hash)
		{
			return true;
		}
	}
}
