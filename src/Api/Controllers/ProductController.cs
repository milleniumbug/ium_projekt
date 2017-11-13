using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("products")]
	public class ProductController : Controller
	{
		private readonly AppDbContext db;

		private string GetUserId(ClaimsPrincipal user)
		{
			return User.Claims.First(claim => claim.Type == "sub").Value;
		}

		public ProductController(AppDbContext db)
		{
			this.db = db;
		}

		// GET api/products
		[HttpGet]
		public IEnumerable<Product> Get()
		{
			var userId = GetUserId(User);
			return db.Products.Where(product => product.User == userId).Select(product => new Product(product));
		}

		// GET api/products/5
		[HttpGet("{id}", Name = nameof(GetById))]
		public IActionResult GetById(long id)
		{
			var userId = GetUserId(User);
			var product = db.Products.Where(p => p.User == userId).FirstOrDefault(p => p.Id == id);
			if(product != null)
			{
				return new ObjectResult(new Product(product));
			}
			return NotFound();
		}

		// POST api/products
		[HttpPost]
		public IActionResult Post([FromBody]Product inputProduct)
		{
			if(inputProduct == null)
			{
				return BadRequest();
			}

			var userId = GetUserId(User);
			var ent = db.Products.Add(new UserProduct
			{
				User = userId,
				Amount = inputProduct.Amount,
				Price = inputProduct.Price,
				ShopName = inputProduct.ShopName,
				Name = inputProduct.Name
			});
			db.SaveChanges();

			return CreatedAtRoute(nameof(GetById), new { id = ent.Entity.Id }, new Product(ent.Entity));
		}

		// PATCH api/products
		[HttpPatch("{id}")]
		public IActionResult Patch(long id, [FromBody]IEnumerable<ProductPatch> patches)
		{
			if(patches == null)
			{
				return BadRequest();
			}

			var userId = GetUserId(User);
			var product = db.Products.Where(p => p.User == userId).FirstOrDefault(p => p.Id == id);
			if(product == null)
			{
				return NotFound();
			}

			foreach(var patch in patches)
			{
				if(patch.What != ProductPatch.TargetField.Amount)
				{
					return new UnsupportedMediaTypeResult();
				}
				switch(patch.Operation)
				{
					case ProductPatch.OperationType.Increase:
						product.Amount += patch.Value;
						break;
					case ProductPatch.OperationType.Decrease:
						product.Amount -= patch.Value;
						break;
				}
			}

			db.Products.Update(product);
			db.SaveChanges();

			return Ok(new Product(product));
		}

		// DELETE api/products/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var userId = GetUserId(User);
			var product = db.Products.Where(p => p.User == userId).FirstOrDefault(p => p.Id == id);
			if(product == null)
			{
				return NotFound();
			}
			db.Products.Remove(product);
			db.SaveChanges();

			return NoContent();
		}
	}
}
