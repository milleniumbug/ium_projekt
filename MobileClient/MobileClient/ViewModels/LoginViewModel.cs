using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MobileClient.ViewModels
{
	public class LoginViewModel : INotifyPropertyChanged
	{
		private readonly Action<string, string> loginCallback;

		public void IssueALogin()
		{
			loginCallback(login, password);
		}

		public LoginViewModel(Action<string, string> loginCallback)
		{
			this.loginCallback = loginCallback;
		}

		private string login;

		public string Login
		{
			get => login;
			set
			{
				if(login != value)
				{
					login = value;
					OnPropertyChanged();
				}
			}
		}

		private string password;

		public string Password
		{
			get => password;
			set
			{
				if(password != value)
				{
					password = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
