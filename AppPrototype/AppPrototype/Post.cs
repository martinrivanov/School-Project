using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPrototype
{
	public class Post
	{
		private string name;
		private string category;
		private double price;

		public Post(string name, string category, double price)
		{
			this.name = name;
			this.category = category.ToLower();
			this.price = price;
		}
	}
}
