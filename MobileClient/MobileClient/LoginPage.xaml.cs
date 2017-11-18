using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public static LoginViewModel BindingContextDummyInstance => null;

		public LoginPage()
		{
			InitializeComponent();
		}

		private async void OnLoginButtonClicked(object sender, EventArgs e)
		{
			var vm = (LoginViewModel)BindingContext;
			try
			{
				await vm.IssueALogin();
				await Navigation.PopModalAsync();
			}
			catch(Exception ex)
			{
				await DisplayAlert("Invalid Login", $"Login failed: {ex.Message}", "OK");
			}
		}
	}
}