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
			
		}

		private static StreamReader ReadFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new InterpreterException($"Could not find file \"{path}\"");
			}
			try
			{
				return new StreamReader(path);
			}
			catch
			{
				throw new InterpreterException($"Error opening file \"{path}\", is it protected or is it in use?");
			}
		}

		private static BinaryReader ReadBinary(string path)
		{
			if (!File.Exists(path))
			{
				throw new InterpreterException($"Could not find file \"{path}\"");
			}
			try
			{
				return new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
			}
			catch
			{
				throw new InterpreterException($"Error opening file \"{path}\", is it protected or is it in use?");
			}
		}

		private static BinaryWriter WriteBinary(string path)
		{
			if (!File.Exists(path))
			{
				throw new InterpreterException($"Could not find file \"{path}\"");
			}
			try
			{
				return new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write));
			}
			catch
			{
				throw new InterpreterException($"Error opening file \"{path}\", is it protected or is it in use?");
			}
		}

		private static StreamWriter WriteFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new InterpreterException($"Could not find file \"{path}\"");
			}
			try
			{
				return new StreamWriter(path);
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

		private static void WriteByte(Stream stream, byte v)
		{
			if (stream.CanWrite)
			{
				stream.WriteByte(v);
			}
			else
			{
				throw new InterpreterException("Stream is not writeable");
			}
		}


		private static string ReadLine(Stream stream) => ReadLine(stream, "utf8");

		private static string ReadLine(Stream stream, string encodeing)
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
					switch (encodeing.ToLower())
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
	}
}
