using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileClient.ViewModels;
using Xamarin.Forms;

namespace MobileClient
{
	public partial class App : Application
	{
		public App (Environment env)
		{
			InitializeComponent();

			MainPage = new MobileClient.MainPage(new MainPageViewModel(env));
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			var vm = (MainPageViewModel)MainPage.BindingContext;
			vm.Save();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
