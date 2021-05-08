using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop
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
