using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.ApplicationSetup
{
	public class ShopContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Follower> Followers { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"server=MARTINIVANOV;database=ShopDB;trusted_connection=true");
		}
	}
}
