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
			state.Functions.Add(new _7sFunction("readFile", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<string, Stream>(ReadFile) }
			}));
			state.Functions.Add(new _7sFunction("writeFile", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<string, Stream>(WriteFile) }
			}));
			state.Functions.Add(new _7sFunction("readByte", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<Stream, byte>(ReadByte) }
			}));
			state.Functions.Add(new _7sFunction("readLine", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<Stream, string>(ReadLine) },
				{ 2, new Func<Stream, string, string>(ReadLine) }
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
			state.Functions.Add(new _7sFunction("close", new Dictionary<int, Delegate>()
			{
				{ 1, new Action<Stream>(Close) }
			}));
		}

		private void Close(Stream s)
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
		
		private static void WriteLine(Stream stream, string line, string encoding)
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
							bytes = Encoding.UTF8.GetBytes(line);
							break;
						case "ascii":
							bytes = Encoding.ASCII.GetBytes(line);
							break;
						default:
							throw new InterpreterException("readLine: encoding must be ascii or utf8!");
					}
					stream.Write(bytes.Concat(new byte[] { (byte)'\n' }).ToArray(), 0, bytes.Length + 1);
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
	}
}
