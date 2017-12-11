using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Api.Models;
using Functional.Maybe;

[assembly: InternalsVisibleTo("Tests")]

namespace ApiClientLib
{
	internal class FailureInducingMockApiClient : IApiClient2
	{
		private readonly IApiClient2 apiClient;
		private readonly Random random;

		private double faultProbability;
		public double FaultProbability
		{
			get => faultProbability;
			set => faultProbability = Math.Min(1.0, Math.Max(0.0, value));
		}

		public FailureInducingMockApiClient(IApiClient2 apiClient, Random random)
		{
			this.apiClient = apiClient;
			this.random = random;
			FaultProbability = 0.0;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			apiClient.Dispose();
		}

		/// <inheritdoc />
		public Task<IEnumerable<Product>> GetAll()
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.GetAll();
		}

		/// <inheritdoc />
		public Task<Product> Add(Product product)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.Add(product);
		}

		/// <inheritdoc />
		public Task Delete(Product product)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.Delete(product);
		}

		/// <inheritdoc />
		public Task<Product> IncreaseAmount(Product product, int howMuch)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.IncreaseAmount(product, howMuch);
		}

		/// <inheritdoc />
		public Task<Product> DecreaseAmount(Product product, int howMuch)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.DecreaseAmount(product, howMuch);
		}

		/// <inheritdoc />
		public Task<Maybe<Product>> Add(Product product, Guid requestId)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.Add(product, requestId);
		}

		/// <inheritdoc />
		public Task Delete(Product product, Guid requestId)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.Delete(product, requestId);
		}

		/// <inheritdoc />
		public Task<Maybe<Product>> IncreaseAmount(Product product, int howMuch, Guid requestId)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.IncreaseAmount(product, howMuch, requestId);
		}

		/// <inheritdoc />
		public Task<Maybe<Product>> DecreaseAmount(Product product, int howMuch, Guid requestId)
		{
			if(random.NextDouble() < FaultProbability)
				throw new ConnectionErrorException("Mock-induced fault");
			return apiClient.DecreaseAmount(product, howMuch, requestId);
		}
	}
}