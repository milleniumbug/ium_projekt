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
		private MainPageViewModel vm;

		public MainPage()
		{
			InitializeComponent();
			vm = (MainPageViewModel) BindingContext;
		}

		private void LoginButtonClicked(object sender, EventArgs e)
		{
			var page = new LoginPage
			{
				BindingContext = new LoginViewModel(async (login, password) => await vm.DoLogin(login, password))
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
	}
}
