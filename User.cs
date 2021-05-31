using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.ApplicationSetup
{
	public class User
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

			using (var context= new ShopContext())
			{
				context.Posts.Add(post);
				context.SaveChanges();
			}

			Console.WriteLine("The post was created!");

			Notify($"{this.UserName} created a post!");
		}

		public void ShowAllPosts()
		{
			List<Post> allPosts = null;

			using (var context = new ShopContext())
			{
				allPosts = context.Posts.ToList();
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

		private void Notify(string message)
		{
			using (var context = new ShopContext())
			{
				foreach (var follower in Followers)
				{
					Notification notification = new Notification
					{
						NotificationSender = this.UserName,
						NotificationReceiver = follower,
						NotificationContent = message,
						IsNotificationRead = false
					};

					context.Notifications.Add(notification);
				}

				context.SaveChanges();
			}
		}
	}
}
