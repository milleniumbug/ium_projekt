using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.ViewModels
{
	public class LoginViewModel : INotifyPropertyChanged
	{
		private readonly Func<string, string, Task> loginCallback;

		public Task IssueALogin()
		{
			return loginCallback(login, password);
		}

		public LoginViewModel(Func<string, string, Task> loginCallback)
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
