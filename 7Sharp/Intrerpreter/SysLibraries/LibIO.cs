using _7Sharp.Manual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _7Sharp.Intrerpreter.SysLibraries
{
	internal class LibIO : SysLibrary
	{
		public override string GetName() => "io.sys";
		
		public override void Import(ref InterpreterState state)
		{
			// Opening / closing streams
			state.Functions.Add(new _7sFunction("readFile", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<string, Stream>(ReadFile) }
			}));
			state.Functions.Add(new _7sFunction("writeFile", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<string, Stream>(WriteFile) }
			}));
			state.Functions.Add(new _7sFunction("close", new Dictionary<int, Delegate>()
			{
				{ 1, new Action<Stream>(Close) }
			}));
			// Reading
			state.Functions.Add(new _7sFunction("readByte", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<Stream, byte>(ReadByte) }
			}));
			state.Functions.Add(new _7sFunction("readLine", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<Stream, string>(ReadLine) },
				{ 2, new Func<Stream, string, string>(ReadLine) }
			}));
			state.Functions.Add(new _7sFunction("readText", new Dictionary<int, Delegate>()
			{
				{ 2, new Func<Stream, int, string>(ReadText) },
				{ 3, new Func<Stream, int, string, string>(ReadText) },
			}));
			// Writing
			state.Functions.Add(new _7sFunction("writeText", new Dictionary<int, Delegate>()
			{
				{ 2, new Action<Stream, string>(WriteText) },
				{ 3, new Action<Stream, string, string>(WriteText) }
			}));
			state.Functions.Add(new _7sFunction("writeByte", new Dictionary<int, Delegate>()
			{
				{ 2, new Action<Stream, int>(WriteByte) }
			}));
			state.Functions.Add(new _7sFunction("writeLine", new Dictionary<int, Delegate>()
			{
				{ 2, new Action<Stream, string>(WriteLine) },
				{ 3, new Action<Stream, string, string>(WriteLine) }
			}));
		}

		private static void WriteText(Stream s, string text) => WriteText(s, text, "utf8");

		[ManualDocs("writeText", "{\"title\":\"writeText(s, txt, enc = \\\"utf8\\\")\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"writeText(<stream>, <text>);\nOR\nwriteText(<stream>, <text>, <encoding>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Write \"},{\"text\":\"txt\",\"color\":\"Green\"},{\"text\":\" to stream \"},{\"text\":\"s\",\"color\":\"Green\"},{\"text\":\" using encoding \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\". Defaults to \"},{\"text\":\"utf8\",\"color\":\"Cyan\"}]}]}")]
		private static void WriteText(Stream stream, string text, string encoding)
		{
			if (stream.CanWrite)
			{
				try
				{
					byte[] bytes;
					switch (encoding.ToLower())
					{
						case "utf8":
						case "utf-8":
							bytes = Encoding.UTF8.GetBytes(text);
							break;
						case "ascii":
							bytes = Encoding.ASCII.GetBytes(text);
							break;
						default:
							throw new InterpreterException("readLine: encoding must be ascii or utf8!");
					}
					stream.Write(bytes.ToArray(), 0, bytes.Length);
				}
				catch (ObjectDisposedException)
				{
					throw new InterpreterException("Cannot write to a stream that is closed!");
				}
			}
			else
			{
				throw new InterpreterException("Stream is not writable!");
			}
		}

		[ManualDocs("readText", "{\"title\":\"readText(s, count, enc = \\\"utf8\\\")\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"readText(<stream>, <text>);\nOR\nreadText(<stream>, <text>, <encoding>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Read \"},{\"text\":\"count\",\"color\":\"Green\"},{\"text\":\" characters from stream \"},{\"text\":\"s\",\"color\":\"Green\"},{\"text\":\" using encoding \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\". \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\" defaults to \"},{\"text\":\"utf8\",\"color\":\"Cyan\"}]}]}")]
		private static string ReadText(Stream s, int count) => ReadText(s, count, "utf8");

		private static string ReadText(Stream s, int count, string encoding)
		{
			if (s.CanRead)
			{
				try
				{
					byte[] bytes = new byte[count];
					s.Read(bytes, 0, bytes.Length);
					switch (encoding.ToLower())
					{
						case "utf8":
						case "utf-8":
							return Encoding.UTF8.GetString(bytes);
						case "ascii":
							return Encoding.ASCII.GetString(bytes);
						default:
							throw new InterpreterException("readText: encoding must be ascii or utf8!");
					}
				}
				catch (ObjectDisposedException)
				{
					throw new InterpreterException("Attempted to read from a closed stream!");
				}
				catch (Exception e)
				{
					throw new InterpreterException("Unkown error while reading from stream", e);
				}
			}
			else
			{
				throw new InterpreterException("Stream is not readable!");
			}
		}

		[ManualDocs("close", "{\"title\":\"close(s)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"close(<stream>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Close stream \"},{\"text\":\"s\",\"color\":\"Green\"},{\"text\":\". \"},{\"text\":\"Not doing this could cause a memory leak!\",\"color\":\"Red\"}]}]}")]
		private static void Close(Stream s)
		{
			try
			{
				s.Close();
			}
			catch
			{
				throw new InterpreterException("Error closing stream! (Was it already closed?)");
			}
		}

		[ManualDocs("readFile", "{\"title\":\"readFile(path)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"readFile(<path>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Get a read stream to file at \"},{\"text\":\"path\",\"color\":\"Green\"},{\"text\":\". See the following for more info: \"},{\"text\":\"readText readLine readByte\",\"color\":\"Yellow\"}]}]}")]
		private static Stream ReadFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new InterpreterException($"Could not find file \"{path}\"");
			}
			try
			{
				return new FileStream(path, FileMode.Open, FileAccess.Read);
			}
			catch
			{
				throw new InterpreterException($"Error opening file \"{path}\", is it protected or is it in use?");
			}
		}

		[ManualDocs("writeFile", "{\"title\":\"writeFile(path)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"writeFile(<path>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Get a write stream to file at \"},{\"text\":\"path\",\"color\":\"Green\"},{\"text\":\". See the following for more info: \"},{\"text\":\"writeText writeLine writeByte\",\"color\":\"Yellow\"}]}]}")]
		private static Stream WriteFile(string path)
		{
			try
			{
				return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
			}
			catch
			{
				throw new InterpreterException($"Error opening file \"{path}\", is it protected or is it in use?");
			}
		}

		[ManualDocs("readByte", "{\"title\":\"readByte(s)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"readByte(<stream>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Read a byte from stream \"},{\"text\":\"s\",\"color\":\"Green\"}]}]}")]
		private static byte ReadByte(Stream stream)
		{
			if (stream.CanRead)
			{
				try
				{
					int v = stream.ReadByte();
					if (v == -1)
					{
						throw new InterpreterException("Attempted to read a byte at the end of a stream!");
					}
					return (byte)v;
				}
				catch (ObjectDisposedException)
				{
					throw new InterpreterException("Attempted to read from a closed stream!");
				}
				catch (Exception e)
				{
					throw new InterpreterException("Unkown error while reading from stream", e);
				}
			}
			else
			{
				throw new InterpreterException("Stream is not readable!");
			}
		}

		[ManualDocs("writeByte", "{\"title\":\"writeByte(s, v)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"writeByte(<stream>, <value>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Write byte \"},{\"text\":\"v\",\"color\":\"Green\"},{\"text\":\" to stream \"},{\"text\":\"s\",\"color\":\"Green\"}]}]}")]
		private static void WriteByte(Stream stream, int v)
		{
			if (stream.CanWrite)
			{
				if (v < byte.MinValue || v > byte.MaxValue)
				{
					throw new InterpreterException("writeByte: byte was out of range!");
				}
				stream.WriteByte((byte)v);
			}
			else
			{
				throw new InterpreterException("Stream is not writable");
			}
		}

		[ManualDocs("readLine", "{\"title\":\"readLine(s, enc = \\\"utf8\\\")\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"readLine(<stream>);\nOR\nreadLine(<stream>, <encoding>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Read \"},{\"text\":\"count\",\"color\":\"Green\"},{\"text\":\" one line of text from stream \"},{\"text\":\"s\",\"color\":\"Green\"},{\"text\":\" using encoding \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\". \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\" defaults to \"},{\"text\":\"utf8\",\"color\":\"Cyan\"}]}]}")]
		private static string ReadLine(Stream stream) => ReadLine(stream, "utf8");

		private static string ReadLine(Stream stream, string encoding)
		{
			if (stream.CanRead)
			{
				try
				{
					List<byte> bytes = new List<byte>();
					bool read = true;
					while (read)
					{
						int v = stream.ReadByte();
						if (v == -1 || v == '\n')
						{
							read = false;
						}
						else
						{
							bytes.Add((byte)v);
						}
					}
					string s = null;
					switch (encoding.ToLower())
					{
						case "utf8":
						case "utf-8":
							s = Encoding.UTF8.GetString(bytes.ToArray());
							break;
						case "ascii":
							s = Encoding.ASCII.GetString(bytes.ToArray());
							break;
						default:
							throw new InterpreterException("readLine: encoding must be ascii or utf8!");
					}
					if (s != null)
					{
						if (s.EndsWith("\r"))
						{
							s = s.Remove(s.Length - 1, 1);
						}
					}
					return s ?? string.Empty;
				}
				catch (ObjectDisposedException)
				{
					throw new InterpreterException("Attempted to read from a closed stream!");
				}
			}
			else
			{
				throw new InterpreterException("Stream is not readable!");
			}
		}

		private static void WriteLine(Stream stream, string line) => WriteLine(stream, line, "utf8");
		
		[ManualDocs("writeLine", "{\"title\":\"writeText(s, txt, enc = \\\"utf8\\\")\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"writeText(<stream>, <text>);\nOR\nwriteText(<stream>, <text>, <encoding>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Write \"},{\"text\":\"txt\",\"color\":\"Green\"},{\"text\":\" to stream \"},{\"text\":\"s\",\"color\":\"Green\"},{\"text\":\" using encoding \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\", followed by a new line. \"},{\"text\":\"enc\",\"color\":\"Green\"},{\"text\":\" defaults to \"},{\"text\":\"utf8\",\"color\":\"Cyan\"}]}]}")]
		private static void WriteLine(Stream stream, string line, string encoding) => WriteText(stream, line + "\n", encoding);
	}
}
