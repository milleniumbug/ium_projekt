using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
	public class Product
	{
		public long Id { get; set; }

		public int Amount { get; set; }

		public string Name { get; set; }

		public string ShopName { get; set; }

		public decimal Price { get; set; }

		public Product()
		{

		}

		public Product(Product other)
		{
			Id = other.Id;
			Amount = other.Amount;
			Price = other.Price;
			ShopName = other.ShopName;
			Name = other.Name;
		}
	}
}
