using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
	public partial class User
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string UserPassword { get; set; }
		public DateTime LastSeen { get; set; }
		[NotMapped]
		public List<string> Followers { get; set; }

		public void CreatePost()
		{
			Console.Write("Enter the name of the product: ");
			string name = Console.ReadLine();

			Console.Write("Enter the category in which the product will be added: ");
			string category = Console.ReadLine();

			Console.Write("Enter the price of the product: ");
			double price = double.Parse(Console.ReadLine());

			Console.WriteLine("Creating the post...");
			Post post = new Post
			{
				PostName = name,
				PostCategory = category,
				PostPrice = price,
				PostCreationDate = DateTime.Now,
				PostCreator = UserName
			};

			using (var context = new ShopContext())
			{
				context.Posts.Add(post);
				context.SaveChanges();
			}

			Console.WriteLine("The post was created!");

			foreach (var follower in Followers)
			{
				Notify($"{this.UserName} created a post!\n{post.ToString()}", follower);
			}
		}

		public void ShowAllPosts()
		{
			List<Post> allPosts = null;

			using (var context = new ShopContext())
			{
				allPosts = context.Posts
					.Where(u => u.PostCreator != this.UserName)
					.ToList();
			}

			foreach (var post in allPosts)
			{
				Console.WriteLine(post.ToString());
			}
		}

		public void Filter()
		{
			Console.WriteLine("Select from the following categories, that you want to be shown (you can select multiple by separating them with |)");
			HashSet<string> categories = null;

			using (var context = new ShopContext())
			{
				categories = context.Posts.Select(n => n.PostCategory).ToHashSet();
			}

			foreach (var item in categories)
			{
				Console.WriteLine($"-{item}");
			}

			categories = categories
				.Select(n => n.ToLower())
				.ToHashSet();

			string category = Console.ReadLine();
			List<string> selectedCategories = category.Split('|').ToList();

			List<Post> posts = null;

			using (var context = new ShopContext())
			{
				posts = context.Posts
					.Where(n => selectedCategories.Contains(n.PostCategory))
					.ToList();
			}

			foreach (var post in posts)
			{
				Console.WriteLine(post.ToString());
			}
		}

		public void Follow()
		{
		enterUsername:
			Console.Write("Enter the username of the user you want to follow: ");
			string username = Console.ReadLine();

			if (!ValidateUsername(username))
			{
				Console.WriteLine("This user does not exist! Try again!");
				goto enterUsername;
			}

			using (var context = new ShopContext())
			{
				Follower follower = new Follower
				{
					Following = this.UserName,
					Followed = username
				};

				context.Followers.Add(follower);
				context.SaveChanges();
			}

			Console.WriteLine($"You just followed {username}! Now you can get notifications from them!");
		}

		public void BuyItems()
		{
			chooseItems:
			Console.WriteLine("Select which items you want to buy (you can select multiple by separating them with |):");

			List<Post> items = null;
			using (var context = new ShopContext())
			{
				items = context.Posts
					.Where(p => p.PostCreator != this.UserName)
					.ToList();

				int num = 1;
				foreach (var item in items)
				{
					Console.WriteLine($"{num}. {item.PostName}: {item.PostPrice}");
					num++;
				}
			}

			List<int> itemID = Console.ReadLine()
				.Split('|')
				.Select(int.Parse)
				.ToList();

			foreach (var item in itemID)
			{
				if (item > items.Count)
				{
					Console.WriteLine($"There's no item with ID number {item}! Try again!\n");
					goto chooseItems;
				}
			}

			List<Post> selectedItems = new List<Post>();

			for (int i = 0; i < itemID.Count; i++)
			{
				for (int j = 0; j < items.Count; j++)
				{
					if (itemID[i] == (j + 1))
					{
						selectedItems.Add(items[j]);
						break;
					}
				}
			}

			double sumPrice = 0d;

			foreach (var item in selectedItems)
			{
				sumPrice += item.PostPrice;
			}

			Console.WriteLine($"The price is {sumPrice:f2}");
			Console.Write("Do you wish to pay? [Y/N] ");
			string answer = Console.ReadLine().ToLower();

			if (answer == "y")
			{
				Console.WriteLine("Purchasing...");

				using (var context = new ShopContext())
				{
					foreach (var item in selectedItems)
					{
						context.Posts.Remove(item);
					}

					context.SaveChanges();
				}

				foreach (var item in selectedItems)
				{
					Notify($"{this.UserName} bought your item\n{item.ToString()}", item.PostCreator);
				}

				Console.WriteLine("Purchase was successful!");
			}

			else if (answer == "n")
			{
				Console.WriteLine("Purchase was cancelled");
				Console.WriteLine("The cart is empthy");
			}
		}

		public void UpdateUsername()
		{
			newUsername:
			Console.Write("Enter your new username: ");
			string username = Console.ReadLine();

			if (ValidateUsername(username))
			{
				Console.WriteLine("This username is already taken! Try again!\n");
				goto newUsername;
			}

			using (var context = new ShopContext())
			{
				var user = context.Users.SingleOrDefault(u => u.UserName == this.UserName);

				if (user != null)
				{
					this.UserName = username;
					user.UserName = username;
					context.SaveChanges();

					Console.WriteLine("Username was successfuly changed!");
					Console.WriteLine($"Welcome back, {this.UserName}");
				}
			}
		}

		public void UpdatePassword()
		{
			oldPassword:
			Console.Write("Enter your old password: ");
			string oldPassword = Console.ReadLine();

			if (!ValidatePassword(this.UserName, oldPassword))
			{
				Console.WriteLine("The password was incorrect! Try again!\n");
				goto oldPassword;
			}

			Console.Write("Enter new password: ");
			string newPassword = Console.ReadLine();

			using (var context = new ShopContext())
			{
				var user = context.Users
					.SingleOrDefault(u => u.UserName == this.UserName);

				if (user != null)
				{
					this.UserPassword = newPassword;
					user.UserPassword = newPassword;
					context.SaveChanges();

					Console.WriteLine("Password was successfully changed");
				}
			}
		}

		public override string ToString()
		{
			int count = 0;

			using (var context = new ShopContext())
			{
				var posts = context.Posts
					.Where(p => p.PostCreator == this.UserName)
					.ToList();

				count = posts.Count;
			}

			return $"Username: {UserName}" +
				$"\nFollowers: {Followers.Count}" +
				$"\nPosts: {count}";
		}

		private bool ValidateUsername(string username)
		{
			using (var context = new ShopContext())
			{
				var usernames = context.Users
					.Where(u => u.UserName == username)
					.ToList();

				if (usernames.Any())
				{
					return true;
				}
			}

			return false;
		}

		private bool ValidatePassword(string username, string password)
		{
			using (var context = new ShopContext())
			{
				var usersWithUsername = context.Users
					.Where(u => u.UserName == username)
					.ToList();

				if (usersWithUsername[0].UserPassword == password)
				{
					return true;
				}
			}

			return false;
		}

		private void Notify(string message, string receiver)
		{
			using (var context = new ShopContext())
			{
				Notification notification = new Notification
				{
					NotificationSender = UserName,
					NotificationReceiver = receiver,
					NotificationContent = message,
					IsNotificationRead = false
				};

				context.Notifications.Add(notification);

				context.SaveChanges();
			}
		}
	}
}
