namespace ApiClientLib
{
	public class ConnectionSettings
	{
		public string Login { get; }

		public string Password { get; }

		public ConnectionSettings(string login, string password)
		{
			Login = login;
			Password = password;
		}

		public string OpenIdUrl { get; set; } = ApiDefaultConfig.defaultOpenIdAddress;

		public string ApiUrl { get; set; } = ApiDefaultConfig.defaultApiAddress;
	}
}