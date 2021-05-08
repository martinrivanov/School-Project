using System;
using System.Collections.Generic;
using System.IO;

namespace OnlineShop
{
	/// <summary>
	/// the implementation of the user
	/// </summary>
	public class User
	{
		private string username;
		private string password;
		private List<Post> posts;

		/// <summary>
		/// Creating a user
		/// </summary>
		/// <param name="username">User's username</param>
		/// <param name="password">User's password</param>
		public User(string username, string password, List<Post> posts)
		{
			this.username = username;
			this.password = password;
			this.posts = posts;
		}

		/// <summary>
		/// We make the username accessible for the user
		/// </summary>
		public string Username
		{
			get
			{
				return this.username;
			}
		}

		/// <summary>
		/// Creating post, that's going to be public for all users
		/// </summary>
		public void CreatePost()
		{
			Console.Write("\nEnter name of the product: ");
			string product = Console.ReadLine();
			Console.Write("Enter category of the product: ");
			string category = Console.ReadLine();
			Console.Write("Enter price of the product: ");
			double price = double.Parse(Console.ReadLine());

			FileInfo file = new FileInfo($"Backup Info\\Posts\\{username}.txt");
			file.IsReadOnly = false;

			using (StreamWriter writer = new StreamWriter($"Backup Info\\Posts\\{username}.txt", true))
			{
				writer.WriteLine($"{product}|{category}|{price}");
				file.IsReadOnly = true;
			}

			file = new FileInfo($"Backup Info\\Posts\\{username}.txt");
			file.IsReadOnly = false;

			using (StreamWriter writer = new StreamWriter($"Backup Info\\Posts\\posts.txt", true))
			{
				DateTime creation = DateTime.Now;
				writer.WriteLine($"{username} - {product}|{category}|{price}|{creation}");
				file.IsReadOnly = true;
			}

			Post post = new Post(product, category, price);
			posts.Add(post);
			Console.WriteLine();
		}

		public void GetAllPosts()
		{
			Console.WriteLine();
			FileInfo file = new FileInfo($"Backup Info\\Posts\\{username}.txt");
			file.IsReadOnly = false;

			using (StreamReader reader = new StreamReader($"Backup Info\\Posts\\posts.txt", true))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string userPost = line.Split(" - ")[0];

					if (!userPost.Equals(username))
					{
						Console.WriteLine(line);
					}
				}

				file.IsReadOnly = true;
			}

			Console.WriteLine();
		}
	}
}
