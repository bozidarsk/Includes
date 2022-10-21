using System;
using System.Linq;

using Utils;
using Utils.Collections;
using Utils.Collections.Generic;
using Utils.Web;

class main 
{
	static void Main() 
	{
		List<string> list = new List<string>();
		list.AddRange(new string[] {
			"az",
			"sum",
			"test"
		});

		foreach (string item in list) 
		{
			Console.WriteLine(item);
		}

		// Console.WriteLine(list.Where(x => x[0] == 'a').ToArray()[0]);

		Console.WriteLine("OK");
	}
}