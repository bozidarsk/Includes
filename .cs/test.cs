using System;
using System.IO;
using System.Linq;

using Utils;
using Utils.Collections;
using Utils.Collections.Generic;
using Utils.Web;

class main 
{
	static void Main() 
	{
		// List<string> list = new List<string>();
		// list.AddRange(new string[] {
		// 	"az",
		// 	"sum",
		// 	"test"
		// });

		// foreach (string item in list) 
		// {
		// 	Console.WriteLine(item);
		// }


		new HTTPServer(
			new HTTPServer.ResponseMethod(HTTPServer.Response.GenerateDirectoryResponse),
			Directory.GetCurrentDirectory(),
			8000,
			WebTools.GetLocalIP(),
			"Test Server"
		).Start();
	}
}