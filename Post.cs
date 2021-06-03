using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
	public class Post
	{
		public int PostId { get; set; }
		public string PostName { get; set; }
		public string PostCategory { get; set; }
		public double PostPrice { get; set; }
		public DateTime PostCreationDate { get; set; }
		public string PostCreator { get; set; }


		public override string ToString()
		{
			return $"Product name: {PostName}\nProduct category: {PostCategory}\nProduct price: {PostPrice}\nPost creator: {PostCreator}\nPost creation date: {PostCreationDate}\n";
		}
	}
}
