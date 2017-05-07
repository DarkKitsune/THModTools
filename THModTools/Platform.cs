using System;

namespace THModTools
{
	public static class Platform
	{
		public const int Unknown = -1;
		public const int Windows = 0;
		public const int Linux = 1;
		public const int Mac = 2;

		public static readonly int ID;

		static Platform()
		{
			switch (Environment.OSVersion.Platform)
			{
				case (PlatformID.Win32NT):
				case (PlatformID.Win32S):
				case (PlatformID.Win32Windows):
				case (PlatformID.WinCE):
					ID = Windows;
					break;
				case (PlatformID.Unix):
					ID = Linux;
					break;
				case (PlatformID.MacOSX):
					ID = Mac;
					break;
				default:
					ID = Unknown;
					break;
			}
		}
	}
}

