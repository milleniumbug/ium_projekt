using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
	public class ProductPatch
	{
		public enum OperationType
		{
			Increase,
			Decrease
		}

		public enum TargetField
		{
			Amount
		}

		public OperationType Operation { get; set; }

		public TargetField What { get; set; }

		public int Value { get; set; }
	}
}
