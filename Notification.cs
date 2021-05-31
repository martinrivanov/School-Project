using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.ApplicationSetup
{
	public class Notification
	{
		public int NotificationId { get; set; }
		public string NotificationSender { get; set; }
		public string NotificationReceiver { get; set; }
		public string NotificationContent { get; set; }
		public bool IsNotificationRead { get; set; }
	}
}
