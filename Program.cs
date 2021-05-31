﻿using System;
using System.Collections.Generic;
using System.Linq;
using OnlineShop.ApplicationSetup;

namespace OnlineShop
{
	class Program
	{
		static void Main()
		{
			logOff:
			Console.WriteLine("Welcome to my Online Shop!");

			commands:
			Console.WriteLine("If you already have an account, type \"log in\"");
			Console.WriteLine("If you don't have an account, type \"register\" ");
			Console.WriteLine("If you already want to exit, type \"exit\"");
			Console.Write("Type here: ");
			string command = Console.ReadLine().ToLower();

			User currentUser = null;
			bool exit = false;

			if (command == "log in")
			{
				currentUser = LogIn();
			}

			else if (command == "register")
			{
				currentUser = Register();
			}

			else if (command == "exit")
			{
				exitAnswer:
				Console.Write("Are you sure? [Y/N] ");
				string answer = Console.ReadLine().ToLower();

				if (answer == "y")
				{
					exit = true;
					Console.WriteLine("Bye!");
				}

				else if (answer == "n")
				{
					Console.WriteLine();
					goto commands;
				}

				else
				{
					Console.WriteLine("We couldn't understand that! Try again!");
					Console.WriteLine();
					goto exitAnswer;
				}
			}

			else
			{
				Console.WriteLine("There's no such command");
				Console.WriteLine("Try again!");
				Console.WriteLine();
				goto commands;
			}

			while (!exit)
			{
				List<Notification> notificationList = null;
				using (var context = new ShopContext())
				{
					notificationList = context.Notifications
						.Where(n => n.NotificationReceiver == currentUser.UserName && !n.IsNotificationRead)
						.ToList();

					if (notificationList != null)
					{
						foreach (var item in notificationList)
						{
							item.IsNotificationRead = true;
						}

						context.SaveChanges();
					}
				}

				if (notificationList.Any())
				{
					Console.WriteLine($"You have {notificationList.Count} unread notifications!");
					Console.Write("Do you wish to open them? [Y/N] ");

					string answer = Console.ReadLine().ToLower();

					if (answer == "y")
					{
						Console.WriteLine("Notifications: ");

						foreach (var item in notificationList)
						{
							Console.WriteLine($"{item.NotificationSender}: {item.NotificationContent}");
						}

						Console.WriteLine("Press key when you're ready!");
						Console.ReadKey();
						Console.WriteLine();
					}
				}

				Console.WriteLine();
				Console.WriteLine("Commands: ");
				Console.WriteLine("1. Creating post");
				Console.WriteLine("2. Viewing all posts");
				Console.WriteLine("3. Filtering posts by category");
				Console.WriteLine("4. Buying other's product");
				Console.WriteLine("5. Follow user");
				Console.WriteLine("6. Account settings");
				Console.WriteLine("7. Log out");
				Console.WriteLine("8. Exit");

				command = Console.ReadLine();

				if (command == "1")
				{
					currentUser.CreatePost();
				}

				else if (command == "2")
				{
					currentUser.ShowAllPosts();
				}

				else if (command == "3")
				{
					currentUser.Filter();
				}

				else if (command == "5")
				{
					currentUser.Follow();
				}

				else if (command == "7")
				{
					Console.WriteLine($"Bye, {currentUser.UserName}\n");
					goto logOff;
				}

				else if (command == "8")
				{
					exit = true;

					Console.WriteLine("Bye!");
				}
			}
		}

		static User LogIn()
		{
			validatingUsername:
			Console.Write("Enter username: ");
			string username = Console.ReadLine();

			Console.WriteLine("Checking username...");
			bool isUsernameExisting = ValidateUsername(username);

			if (isUsernameExisting)
			{
				Console.WriteLine("User with this username does not exist. Try again!");
				Console.WriteLine();
				goto validatingUsername;
			}

			Console.WriteLine("Username check finished!");

			passwordCheck:
			Console.Write("Enter password: ");
			string password = Console.ReadLine();

			Console.WriteLine("Checking password...");
			bool isPasswordCorrect = ValidatePassword(username, password);

			if (!isPasswordCorrect)
			{
				Console.WriteLine("Incorrect password! Try again!");
				goto passwordCheck;
			}

			User user = new User
			{
				UserName = username,
				UserPassword = password,
				LastSeen = DateTime.Now
			};

			using (var context = new ShopContext())
			{
				var result = context.Users
					.SingleOrDefault(u => u.UserName == username);

				if (result != null)
				{
					result.LastSeen = DateTime.Now;
					context.SaveChanges();
				}

				var followers = context.Followers
					.Where(u => u.Followed == username)
					.ToList();

				user.Followers = new List<string>();

				foreach (var follower in followers)
				{
					user.Followers.Add(follower.Following);
				}
			}

			Console.WriteLine("Logging in was successful!");
			Console.WriteLine($"Welcome, {username}");

			return user;
		}

		static User Register()
		{
			creatingUsername:
			Console.Write("Enter username: ");
			string username = Console.ReadLine();

			Console.WriteLine("Checking username...");
			bool isUsernameExisting = !ValidateUsername(username);

			if (isUsernameExisting)
			{
				Console.WriteLine("User with this username already exists. Try again!");
				Console.WriteLine();
				goto creatingUsername;
			}

			Console.WriteLine("Username check finished!");
			Console.Write("Enter password: ");
			string password = Console.ReadLine();

			Console.WriteLine("Creating user...");
			User user = new User
			{
				UserName = username,
				UserPassword = password,
				LastSeen = DateTime.Now
			};

			using (var context = new ShopContext())
			{
				context.Users.Add(user);
				context.SaveChanges();
			}

			Console.WriteLine("User was successfully created!");
			Console.WriteLine($"Welcome, {username}");

			return user;
		}

		static bool ValidateUsername(string username)
		{
			using (var context = new ShopContext())
			{
				var usersWithUsername = context.Users
					.Where(u => u.UserName == username)
					.ToList();

				if (usersWithUsername.Any())
				{
					return false;
				}
			}

			return true;
		}

		static bool ValidatePassword(string username, string password)
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
	}
}
