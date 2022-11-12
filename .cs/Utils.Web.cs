#if UNITY
using UnityEngine;
#endif

using System;
using System.Text;
using System.Net;

namespace Utils.Web 
{
	public static class WebTools 
	{
		public static string ReplaceURLChars(string input) 
		{
			string output = "";
			int i = 0;

			for (i = 0; i < input.Length; i++) 
			{
				if (input[i] == '%' && i + 2 < input.Length) 
				{
					byte b = (byte)Format.HexToUInt(Convert.ToString(input[i + 1]) + Convert.ToString(input[i + 2]));
					if ((b == 0xd0 || b == 0xd1) && i + 5 < input.Length) { output += Encoding.UTF8.GetString(new byte[] { b, (byte)Format.HexToUInt(Convert.ToString(input[i + 4]) + Convert.ToString(input[i + 5])) }); i+= 3; }
					else { output += Convert.ToString((char)b); }
					i += 2;
					continue;
				}

				output += input[i];
			}

			return output;
		}

		public static IPAddress ConvertIP(string address) 
		{
			byte[] bytes = new byte[4];
			string[] octets = address.Split('.');
			try 
			{
				bytes[0] = (byte)Convert.ToInt32(octets[0]);
				bytes[1] = (byte)Convert.ToInt32(octets[1]);
				bytes[2] = (byte)Convert.ToInt32(octets[2]);
				bytes[3] = (byte)Convert.ToInt32(octets[3]);
			}
			catch { return WebTools.ConvertIP("127.0.0.1"); }
			return new IPAddress(bytes);
		}

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