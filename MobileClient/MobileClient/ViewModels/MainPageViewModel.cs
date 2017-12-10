using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Api.Models;
using ApiClientLib;
using Xamarin.Forms;

namespace MobileClient.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
		private Product selectedProduct;

		private string openIdAuthority;

		private string appServerAddress;

		public Product SelectedProduct
		{
			get => selectedProduct;
			set
			{
				if(selectedProduct != value)
				{
					selectedProduct = value;
					OnPropertyChanged();
				}
			}
		}

		private int amountDelta;

		public int AmountDelta
		{
			get => amountDelta;
			set
			{
				if(amountDelta != value)
				{
					amountDelta = value;
					OnPropertyChanged();
				}
			}
		}

		private string shopName;

		public string ShopName
		{
			get => shopName;
			set
			{
				if(shopName != value)
				{
					shopName = value;
					OnPropertyChanged();
				}
			}
		}

		private string name;

		public string Name
		{
			get => name;
			set
			{
				if(name != value)
				{
					name = value;
					OnPropertyChanged();
				}
			}
		}

		private decimal price;

		public decimal Price
		{
			get => price;
			set
			{
				if(price != value)
				{
					price = value;
					OnPropertyChanged();
				}
			}
		}

		private IApiClient apiClient;
		private readonly string offlineStorageBasePath;

		public MainPageViewModel(Environment env) :
			this(env, Configuration.AppServerAddress, Configuration.OpenIdAuthority)
		{
			
		}

		public MainPageViewModel(Environment env, string appServerAddress, string openIdAuthority)
		{
			this.offlineStorageBasePath = env.DataBasePath;
			this.appServerAddress = appServerAddress;
			this.openIdAuthority = openIdAuthority;
		}

		private IApiClient ApiClient
		{
			get => apiClient;
			set
			{
				if(apiClient != value)
				{
					apiClient = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(IsLoggedIn));
				}
			}
		}

		public bool IsLoggedIn => ApiClient != null;

		public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public async Task DoLogin(string login, string password)
		{
			ApiClient = await SynchronizingApiClient.Create(offlineStorageBasePath, new ConnectionSettings(login, password)
			{
				ApiUrl = Configuration.AppServerAddress,
				OpenIdUrl = Configuration.OpenIdAuthority
			});
			Products.Clear();
			foreach(var product in await apiClient.GetAll())
			{
				Products.Add(product);
			}
		}

		public async Task Synchronize()
		{
			(ApiClient as SynchronizingApiClient)?.Synchronize();
			Products.Clear();
			foreach(var product in await apiClient.GetAll())
			{
				Products.Add(product);
			}
		}

		public void Save()
		{
			(ApiClient as SynchronizingApiClient)?.Save();
		}

		public async void AddProduct()
		{
			if(ApiClient == null)
				return;
			var p = new Product
			{
				Amount = 0,
				Name = Name,
				Price = Price,
				ShopName = ShopName
			};
			p = await ApiClient.Add(p);
			Products.Add(p);
		}

		public async void DecreaseProduct()
		{
			if(SelectedProduct == null)
				return;
			var product = SelectedProduct;
			var newProduct = await ApiClient.DecreaseAmount(product, amountDelta);
			Products[Products.IndexOf(product)] = newProduct;
			SelectedProduct = newProduct;
		}

		public async void IncreaseProduct()
		{
			if(SelectedProduct == null)
				return;
			var product = SelectedProduct;
			var newProduct = await ApiClient.IncreaseAmount(product, amountDelta);
			Products[Products.IndexOf(product)] = newProduct;
			SelectedProduct = newProduct;
		}

		public void RemoveProduct()
		{
			var product = SelectedProduct;
			ApiClient.Delete(product);
			Products.Remove(product);
			SelectedProduct = null;
		}
	}
}
