using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
					switch (encoding.ToLower())
					{
						case "utf8":
						case "utf-8":
							return Encoding.UTF8.GetString(bytes.ToArray());
						case "ascii":
							return Encoding.ASCII.GetString(bytes.ToArray());
						default:
							throw new InterpreterException("readLine: encoding must be ascii or utf8!");
					}
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

		private static void WriteLine(Stream stream, string line, string encoding) => WriteText(stream, line + "\n", encoding);
	}
}
