#if UNITY
using UnityEngine;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Utils.Web 
{
	public class HTTPServer 
	{
		public string Version { private set; get; }
		public string Name { private set; get; }
		public string CurrentDirectory { private set; get; }

		public string HostName { private set; get; }
		public int Port { private set; get; }
		public bool IsRunning { private set; get; }

		public delegate Response ResponseMethod(HTTPServer server, Request request);
		private ResponseMethod responseMethod;

		private string lastHTML;
		private TcpListener listener;
		private Thread thread;

		public static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>() 
		{
			{ ".html", "text/html" },
			{ ".htm", "text/html" },
			{ ".css", "text/css" },
			{ ".js", "text/javascript" },

			{ ".ico", "image/x-icon" },
			{ ".png", "image/png" },
			{ ".jpg", "image/jpeg" },
			{ ".jpeg", "image/jpeg" },
			{ ".gif", "image/gif" },
			{ ".webp", "image/webp" },

			{ ".txt", "text/plain" },
			{ ".pdf", "application/pdf" },
			{ ".json", "application/json" },
			{ ".csv", "text/csv" },				

			{ ".mp4", "video/mp4" },
			{ ".mkv", "video/x-matroska" },
			{ ".webm", "video/webm" },
			{ ".mp3", "audio/mpeg" },
			{ ".wav", "audio/wav" },
			{ ".ttf", "font/ttf" },

			{ ".c", "text/plain" },
			{ ".cpp", "text/plain" },
			{ ".cs", "text/plain" },
			{ ".h", "text/plain" },
			{ ".hpp", "text/plain" },
			{ ".java", "text/plain" },
			{ ".py", "text/plain" },
			{ ".s", "text/plain" },
			{ ".asm", "text/plain" },
			{ ".gitignore", "text/plain" },
			{ ".gitattributes", "text/plain" },
			{ ".yaml", "text/plain" },
			{ ".xaml", "text/plain" },
			{ ".xml", "text/plain" },
			{ ".csproj", "text/plain" },
			{ ".sln", "text/plain" },
			{ ".hlsl", "text/plain" },
			{ ".log", "text/plain" },
			{ ".sh", "text/plain" },
			{ ".bat", "text/plain" },
			{ ".md", "text/markdown" },

			{ ".shader", "text/plain" },
			{ ".meta", "text/plain" },
			{ ".prefab", "text/plain" },
			{ ".mat", "text/plain" },
			{ ".asset", "text/plain" },

			{ ".exe", "application/x-download" },
			{ ".dll", "application/x-download" },
			{ ".bin", "application/x-download" },
			{ ".out", "application/x-download" },
			{ ".o", "application/x-download" },
			{ ".apk", "application/x-download" },
			{ ".jar", "application/x-download" },
			{ ".doc", "application/x-download" },
			{ ".ppt", "application/x-download" },
			{ ".xls", "application/x-download" },
			{ ".docx", "application/x-download" },
			{ ".pptx", "application/x-download" },
			{ ".xlsx", "application/x-download" },
			{ ".blend", "application/x-download" },
			{ ".fbx", "application/x-download" },
			{ ".deb", "application/x-download" },
			{ ".psd", "application/x-download" },
			{ ".rar", "application/x-download" },
			{ ".zip", "application/x-download" },
			{ ".7z", "application/x-download" },
			{ ".lnk", "application/x-download" },
		};

		private void HandleClient(TcpClient client) 
		{
			if (client == null) { return; }

			StreamReader reader = new StreamReader(client.GetStream());
			string message = "";

			while (reader.Peek() != -1) { message += reader.ReadLine() + "\n"; }

			Request request = Request.GenerateRequest(message);
			Response response = responseMethod(this, request);

			int errorCode = Convert.ToInt32(response.Status.Split(' ')[0]);
			string color = (errorCode >= 400) ? ColorConsole.RedF : ((errorCode == 200) ? ColorConsole.GreenF : ColorConsole.GrayF);

			if (request != null) { ColorConsole.WriteLine("Request: " + request.Type + " " + request.Host + request.URL + " - Response: " + response.Mime + " " + color + errorCode.ToString() + ColorConsole.GrayF); }
			else { ColorConsole.WriteLine(ColorConsole.RedF + "Null request." + ColorConsole.GrayF); }
			response.Post(this, client.GetStream());
		}

		private void Run() 
		{
			IsRunning = true;
			listener.Start();

			while (IsRunning) 
			{
				if (listener == null) { IsRunning = false; return; }
				TcpClient client = listener.AcceptTcpClient();
				HandleClient(client);
				if (client != null) { client.Close(); }
			}

			listener.Stop();
			IsRunning = false;
		}

		public void Start() { thread.Start(); Console.WriteLine("Server started on " + HostName + ":" + Port.ToString()); }
		public void Stop() { thread.Abort(); Console.WriteLine("Server stopped."); IsRunning = false; }

		public HTTPServer(
			ResponseMethod responseMethod = null,
			string CurrentDirectory = null,
			int Port = 8000,
			string HostName = null,
			string Name = null
		) {
			this.Version = "HTTP/1.1";
			this.Name = (Name != null) ? Name : "HTTP Server";
			this.HostName = (HostName != null) ? HostName : "127.0.0.1";
			this.Port = Port;
			this.CurrentDirectory = (CurrentDirectory != null) ? CurrentDirectory : Directory.GetCurrentDirectory();
			this.responseMethod = (responseMethod != null) ? responseMethod : new ResponseMethod(Response.GenerateDirectoryResponse);
			this.listener = new TcpListener(WebTools.ConvertIP(this.HostName), this.Port);
			this.thread = new Thread(new ThreadStart(Run));
		}



		public class Request 
		{
			public string Type { private set; get; }
			public string URL { private set; get; }
			public string Host { private set; get; }

			public static Request GenerateRequest(string request) 
			{
				if (request == "" || request == null) { return null; }

				string[] tokens = request.Split(' ', '\n');
				string type = tokens[0];
				string url = tokens[1];
				string host = tokens[4];

				return new Request(type, url, host);
			}

			public Request() {}
			public Request(string Type, string URL, string Host) 
			{
				this.Type = Type;
				this.URL = URL;
				this.Host = Host;
			}
		}



		public class Response 
		{
			public string Status { set; get; }
			public string Mime { set; get; }
			public byte[] Data { set; get; }
			public string Headers { private set; get; }

			private static Response Basic(string type, string message) { return new Response(type, "text/html", Encoding.UTF8.GetBytes("<h1>" + type + "</h1><p>" + message + "</p>")); }
			public static Response OK(string message = "") { return Response.Basic("200 OK", message); }
			public static Response BadRequest(string message = "") { return Response.Basic("400 Bad Request", message); }
			public static Response Forbidden(string message = "") { return Response.Basic("403 Forbidden", message); }
			public static Response NotFound(string message = "") { return Response.Basic("404 Not Found", message); }
			public static Response MethodNotAllowed(string message = "") { return Response.Basic("405 Method Not Allowed", message); }
			public static Response NotAcceptable(string message = "") { return Response.Basic("406 Not Acceptable", message); }
			public static Response UnsupportedMediaType(string message = "") { return Response.Basic("415 Unsupported Media Type", message); }
			public static Response NotImplemented(string message = "") { return Response.Basic("501 Not Implemented", message); }

			private static string contextHTML = 
				"<div class=\"hide\" id=\"context\">" + 
					"<p>Open</p>" + 
					"<p>Open in new tab</p>" + 
					"<p>Copy name</p>" + 
					"<p>Copy link</p>" + 
					"<p>Download</p>" + 
				"</div>"
			;
			private static string contextCSS = 
				".hide { display: none; }" + 
				".show " + 
				"{" + 
					"background-color: #292a2d;" + 
					"color: #ffffff;" + 
				"}" + 
				".show { position: absolute; }" + 
				".show p:hover { background-color: #3f4042; }" + 
				".show p " + 
				"{" + 
					"font-family: monospace;" + 
					"padding-left: 15px;" + 
					"padding-right: 15px;" + 
					"padding-top: 5px;" + 
					"padding-bottom: 5px;" + 
					"margin: 0;" + 
				"}"
			;
			private static string contextJS = 
				"var context = document.getElementById(\"context\");" + 
				"var lastItem;" + 

				"var items = document.getElementsByClassName(\"item\");" + 
				"for (var i = 0; i < items.length; i++) " + 
				"{" + 
					"items[i].addEventListener('contextmenu', function(e) " + 
					"{" + 
						"context.className = \"show\";" + 
						"context.style.left = e.pageX + \"px\";" + 
						"context.style.top = e.pageY + \"px\";" + 
						"lastItem = e.target;" + 
						"e.preventDefault();" + 
					"}, false);" + 
				"}" + 

				"document.addEventListener('click', function(e) " + 
				"{" + 
					"context.className = \"hide\";" + 
					"run(e.target);" + 
				"}, false);" + 

				"function run(command) " + 
				"{" + 
					"switch (command.innerText) " + 
					"{" + 
						"case \"Open\":" + 
							"window.open(lastItem.getAttribute(\"href\"), '_self');" + 
							"break;" + 
						"case \"Open in new tab\":" + 
							"window.open(lastItem.getAttribute(\"href\"), '_blank');" + 
							"break;" + 
						"case \"Copy name\":" + 
							"console.log(lastItem.innerText);" + 
							// "navigator.clipboard.writeText(lastItem.innerText);" + // requires https
							"break;" + 
						"case \"Copy link\":" + 
							"console.log(lastItem.getAttribute(\"href\"));" + 
							// "navigator.clipboard.writeText(lastItem.getAttribute(\"href\"));" + // requires https
							"break;" + 
						"case \"Download\":" + 
							"window.open(lastItem.getAttribute(\"href\") + \"?download=true\", '_self');" + 
							"break;" + 
					"}" + 
				"}"
			;

			public static Response GenerateHTMLResponse(HTTPServer server, Request request) 
			{
				if (request == null) { return Response.BadRequest(); }

				if (request.Type == "GET") 
				{
					Response response = Response.OK();
					int paramsIndex = request.URL.IndexOf("?");
					string path = (paramsIndex >= 0) ? request.URL.Remove(paramsIndex) : request.URL;
					if (path[path.Length - 1] == '/') { path = path.TrimEnd('/'); }
					path = WebTools.ReplaceURLChars(path.Replace("/", "\\"));

					int dotIndex = path.LastIndexOf(".");
					string dir = server.CurrentDirectory + path;
					if (File.Exists(dir) || dotIndex >= 0) 
					{
						string extention = (dotIndex >= 0) ? path.Substring(dotIndex, path.Length - dotIndex).ToLower() : "";
						try { response.Mime = HTTPServer.MimeTypes[extention]; }
						catch (KeyNotFoundException) 
						{
							ColorConsole.WriteLine(ColorConsole.YellowF + "MIME not avaliable for URL: " + request.URL + ColorConsole.GrayF);
							return Response.NotAcceptable("MIME not found for '" + request.URL + "'.");
						}

						string file = dir;
						if (!File.Exists(file)) 
						{
							file = file.Replace(path, server.lastHTML + path);
							if (!File.Exists(file)) { return Response.NotFound("File not found. - '" + file + "'"); }
						}

						response.Data = File.ReadAllBytes(file);
						return response;
					}
					else 
					{
						if (!Directory.Exists(dir)) { return Response.NotFound("Directory not found. - '" + dir + "'"); }
						string[] files = { "index.html", "index.htm", "default.html", "default.htm" };

						for (int i = 0; i < files.Length; i++) { if (File.Exists(dir + files[i])) { response.Data = File.ReadAllBytes(dir + files[i]); server.lastHTML = path; return response; } }

						files = Directory.GetFiles(dir, "*.html", SearchOption.TopDirectoryOnly);
						if (files.Length > 0) { response.Data = File.ReadAllBytes(files[0]); server.lastHTML = path; return response; }

						files = Directory.GetFiles(dir, "*.htm", SearchOption.TopDirectoryOnly);
						if (files.Length > 0) { response.Data = File.ReadAllBytes(files[0]); server.lastHTML = path; return response; }

						return Response.NotFound("HTML file not found.");
					}
				}
				
				return Response.MethodNotAllowed("Only GET method is allowed.");
			}

			public static Response GenerateDirectoryResponse(HTTPServer server, Request request) 
			{
				if (request == null) { return Response.BadRequest(); }

				if (request.Type == "GET") 
				{
					Response response = Response.OK();
					bool forceDownload = request.URL.EndsWith("?download=true");
					int paramsIndex = request.URL.IndexOf("?");
					string path = (paramsIndex >= 0) ? request.URL.Remove(paramsIndex) : request.URL;
					if (path[path.Length - 1] == '/') { path = path.TrimEnd('/'); }
					path = WebTools.ReplaceURLChars(path.Replace("/", "\\"));

					if (File.Exists(server.CurrentDirectory + path)) 
					{
						int dotIndex = path.LastIndexOf(".");
						string extention = (dotIndex >= 0) ? path.Substring(dotIndex, path.Length - dotIndex).ToLower() : "";
						try { response.Mime = HTTPServer.MimeTypes[extention]; }
						catch (KeyNotFoundException) 
						{
							ColorConsole.WriteLine(ColorConsole.YellowF + "MIME not avaliable for URL: " + request.URL + ColorConsole.GrayF);
							response.Mime = "text/html";
						}

						string file = server.CurrentDirectory + path;
						if (!File.Exists(file)) 
						{
							file = file.Replace(server.CurrentDirectory, server.lastHTML);
							if (!File.Exists(file)) { return Response.NotFound("File not found. - '" + file + "'"); }
						}

						if (response.Mime == "application/pdf") { response.AddHeader("Content-Disposition: inline"); response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("image/")) { response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("audio/")) { response.Mime = "audio/mpeg"; response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("video/")) { response.Mime = "video/mp4"; response.Data = File.ReadAllBytes(file); return response; }
						if (forceDownload || response.Mime == "application/x-download") 
						{
							response.AddHeader("Content-Disposition: attachment; filename=\"" + String2.GetStringAt(file, file.LastIndexOf("\\") + 1, file.Length - 1) + "\"");
							response.Data = File.ReadAllBytes(file);
							return response;
						}

						response.Mime = "text/plain";
						response.Data = Encoding.UTF8.GetBytes(File.ReadAllText(file));
						return response;
					}
					else 
					{
						string dir = server.CurrentDirectory + path;
						if (!Directory.Exists(dir)) { return Response.NotFound(((dir.LastIndexOf(".") - dir.LastIndexOf("\\") > 1) ? "File" : "Directory") + " not found. - '" + dir + "'"); }

						bool isRoot = server.CurrentDirectory == dir;
						int t = (!isRoot) ? 1 : 0;
						string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
						string[] directories = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
						string[] items = new string[files.Length + directories.Length + t];

						if (!isRoot) { items[0] = " "; }
						for (int i = 0; i < files.Length; i++) { items[t++] = files[i].Replace(dir + "\\", ""); }
						for (int i = 0; i < directories.Length; i++) { items[t++] = directories[i].Replace(dir + "\\", ""); }
						Array.Sort(items, (x, y) => String.Compare(x, y));
						if (!isRoot) { items[0] = ".."; }

						string output = 
						"<html><head>" +
						"<meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\">" +
						"<meta name=\"color-scheme\" content=\"light dark\">" +
						"<style type=\"text/css\">" + contextCSS + "</style>" + 
						"</head><body>" +
						"<p style=\"font-family: monospace; word-wrap: break-word; white-space: pre-wrap;\">";
						for (int i = 0; i < items.Length; i++) 
						{
							bool isURL = items[i].EndsWith(".url");
							output += "<a class=\"item\" href=\"" +
							((!isURL) ? (path + "\\" + items[i]) : File.ReadAllText(dir + "\\" + items[i]).Replace("[InternetShortcut]\x000d\x000aURL=", "")) +
							"\"" + ((isURL) ? " target=\"_blank\"" : "") + ">" + items[i] +
							((Directory.Exists(dir + "\\" + items[i])) ? "/" : "") +
							"</a>\n";
						}

						output += "</p>" + contextHTML + "</body><script type=\"text/javascript\">" + contextJS + "</script></html>";
						response.Data = Encoding.UTF8.GetBytes(output);
						return response;
					}
				}
				
				return Response.MethodNotAllowed("Only GET method is allowed.");
			}

			public void AddHeader(string header) { Headers += header + "\r\n"; }
			public void Post(HTTPServer server, NetworkStream stream) 
			{
				StreamWriter writer = new StreamWriter(stream);
				Headers = server.Version + " " + Status + "\r\n" + Headers;
				AddHeader("Content-Type: " + Mime + "; charset=utf-8");
				AddHeader("Accept-Ranges: bytes");
				AddHeader("Content-Length: " + Data.Length.ToString());
				writer.WriteLine(Headers);
				writer.Flush();

				try { stream.Write(Data, 0, Data.Length); }
				catch (System.IO.IOException) {}
			}

			public Response() {}
			public Response(string Status, string Mime, byte[] Data) 
			{
				this.Status = Status;
				this.Mime = Mime;
				this.Data = Data;
				this.Headers = "";
			}
		}
	}
}