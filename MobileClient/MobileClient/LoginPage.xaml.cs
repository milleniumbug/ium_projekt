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
			InitializeComponent ();
		}

		private void OnLoginButtonClicked(object sender, EventArgs e)
		{
			var vm = (LoginViewModel) BindingContext;
			vm.IssueALogin();
			Navigation.PopModalAsync();
		}
	}
}