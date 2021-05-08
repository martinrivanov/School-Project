using System;
using System.Collections.Generic;
using System.IO;

namespace OnlineShop
{
	class Program
	{
		static void Main(string[] args)
		{
			User currentUser = null;

		start:
			Console.WriteLine("Welcome!");
			Console.WriteLine("If you're new here, please type Register");
			Console.WriteLine("If you already have a profile, please type Log in");

			string command = Console.ReadLine().ToLower();

			if (command == "register")
			{
				currentUser = Register();
			}

			else if (command == "log in")
			{
				currentUser = LogIn();
			}

			Console.WriteLine($"Welcome, {currentUser.Username}!");

			while ((command = Console.ReadLine().ToLower()) != "end")
			{
				if (command.Equals("create post"))
				{
					currentUser.CreatePost();
				}

				else if (command.Equals("get all posts"))
				{
					currentUser.GetAllPosts();
				}

				else if (command.Equals("log out"))
				{
					Console.WriteLine($"Bye, {currentUser.Username}\n");
					currentUser = null;
					goto start;
				}
			}
		}

		/// <summary>
		/// We make a registration, so we can create a new user
		/// </summary>
		/// <returns>The info of the new user</returns>
		static User Register()
		{
		username:
			Console.Write("Enter username: ");
			string username = Console.ReadLine();

			if (File.Exists($"Backup Info\\Users\\{username}.txt"))
			{
				Console.WriteLine("There's already a user with this username!");
				Console.WriteLine("Please, try again!");
				goto username;
			}

			Console.Write("Enter password: ");
			string password = Console.ReadLine();

			string newFile = $"Backup Info\\Users\\{username}.txt";
			File.WriteAllText(newFile, password);
			FileInfo info = new FileInfo(newFile);

			if (!info.IsReadOnly)
			{
				info.IsReadOnly = true;
			}

			FileStream fs = File.Create($"Backup Info\\Posts\\{username}.txt");
			info = new FileInfo($"Backup Info\\Posts\\{username}.txt");

			if (!info.IsReadOnly)
			{
				info.IsReadOnly = true;
			}

			fs.Close();
			return new User(username, password, new List<Post>());
		}

		/// <summary>
		/// Logging into an already existing user
		/// </summary>
		/// <returns>The user we're logging in</returns>
		static User LogIn()
		{
		username:
			Console.Write("Enter username: ");
			string username = Console.ReadLine();
			string password = " ";

			if (File.Exists($"Backup Info\\Users\\{username}.txt"))
			{
			enterPassword:
				Console.Write("Enter password: ");
				password = Console.ReadLine();

				StreamReader stream = new StreamReader($"Backup Info\\Users\\{username}.txt");
				string correctPassword;

				using (stream)
				{
					correctPassword = stream.ReadLine();
				}

				if (password != correctPassword)
				{
					Console.WriteLine("Incorrect password!");
					Console.WriteLine("Please, try again!");
					goto enterPassword;
				}
			}

			else
			{
				Console.WriteLine("There's no such username in the database!");
				Console.WriteLine("Please, try again!");
				goto username;
			}

			return new User(username, password, LoadPosts(username));
		}

		/// <summary>
		/// Loading posts when logging in
		/// </summary>
		/// <param name="username">The username of the account, that we try to log in</param>
		/// <returns>All of the posts, that were created by this profile</returns>
		static List<Post> LoadPosts(string username)
		{
			List<Post> posts = new List<Post>();

			using (StreamReader reader = new StreamReader($"Backup Info\\Posts\\{username}.txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] input = line.Split('|');
					Post post = new Post(input[0], input[1], double.Parse(input[2]));
					posts.Add(post);
				}
			}

			return posts;
		}
	}
}
