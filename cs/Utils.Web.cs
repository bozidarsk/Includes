using System;
using System.Linq;
using System.Text;
using System.Net;

namespace Utils.Web 
{
	public static class WebTools 
	{
		public static string ReplaceURLChars(string input) 
		{
			for (int i = 0; i < input.Length; i++) 
			{
				if (input[i] == '%' && i + 2 < input.Length) 
				{
					byte b = (byte)Utils.Parsers.Convert.HexToUInt(input.Substring(i + 1, 2));
					input = input.Remove(i, 3).Insert(i, ((char)b).ToString());
				}
			}

			byte[] bytes = Encoding.UTF8.GetBytes(input);
			throw new NotImplementedException();
		}

		public static IPAddress ConvertIP(string address) => new IPAddress(address.Split('.').Select(x => byte.Parse(x)).ToArray());

		public static string GetLocalIP() 
		{
			System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
		    for (int i = 0; i < host.AddressList.Length; i++)
		    {
		        if (host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
		        	&& host.AddressList[i].ToString().StartsWith("192.168.1.")) { return host.AddressList[i].ToString(); }
		    }

		    return "127.0.0.1";
		}
	}
}