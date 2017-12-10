using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using MobileClient.ViewModels;
using Xamarin.Forms;

namespace MobileClient
{
	public partial class MainPage : ContentPage
	{
		public static MainPageViewModel BindingContextDummyInstance => null;

		private readonly MainPageViewModel vm;

		public MainPage(MainPageViewModel vm)
		{
			this.vm = vm;
			InitializeComponent();
			BindingContext = vm;
		}

		private void LoginButtonClicked(object sender, EventArgs e)
		{
			var page = new LoginPage
			{
				BindingContext = new LoginViewModel(async (login, password) => await vm.DoLogin(login, password))
				{
					Login = vm.PreviousLogin()
				}
			};
			Navigation.PushModalAsync(page);
		}

		private void AddProduct(object sender, EventArgs e)
		{
			vm.AddProduct();
		}

		private void RemoveProduct(object sender, EventArgs e)
		{
			vm.RemoveProduct();
		}

		private void IncreaseProduct(object sender, EventArgs e)
		{
			vm.IncreaseProduct();
		}

		private void DecreaseProduct(object sender, EventArgs e)
		{
			vm.DecreaseProduct();
		}

		private async void Synchronize(object sender, EventArgs e)
		{
			await vm.Synchronize();
		}
	}
}
