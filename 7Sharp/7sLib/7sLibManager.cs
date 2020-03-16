using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace _7Sharp._7sLib
{
	using static Console;
	internal class _7sLibManager
	{
		const int LIBVERSION = 1;

		internal static void Save(string path)
		{
			try
			{
				//create directory
				string dir = GetTempDir();
				Directory.CreateDirectory(dir);
				//create files
				string sha1Text = WriteAndGetSHA1(dir + ".text", Program.shell.GetCode());
				_ = WriteAndGetSHA1(dir + ".head", "7SLIB\nV"+LIBVERSION+"\n"+sha1Text); 
				//compress
				string output = path + ".7slib";
				ZipFile.CreateFromDirectory(dir, output);
				Directory.Delete(dir, true);
			}
			catch (Exception e)
			{
				Utils.PrintError(e);
			}
		}

		private static string WriteAndGetSHA1(string path, string contents)
		{
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(contents);
			}
			return GetSHA1(path);
		}

		internal static _7sLibrary Load(string path)
		{
			try
			{
				using (ZipArchive archive =  ZipFile.Open(path, ZipArchiveMode.Read))
				{
					if (archive.GetEntry(".head") != null)
					{
						if (archive.GetEntry(".text") != null)
						{
							//check header
							string[] header;
							using (StreamReader sr = new StreamReader(archive.GetEntry(".head").Open()))
							{
								header = sr.ReadToEnd().Split('\n');
							}
							if (header != null && header.Length >= 3)
							{
								if (header.ToList().FindIndex(x => x == null) > -1)
								{
									throw new FormatException("Invalid header!");
								}
								if (header[0] != "7SLIB")
								{
									throw new FormatException($"Invalid header! Expected 7SLIB at line 1! Got {header[0]}");
								}
								int v;
								if (!header[1].StartsWith("V") || !int.TryParse(header[1].Substring(1), out v))
								{
									throw new FormatException($"Invalid header! Expected V{LIBVERSION} or higher at line 2!");
								}
								if (v < LIBVERSION)
								{
									throw new FormatException($"Library version is INVALID! Expected {LIBVERSION} or more, got {v}");
								}
							}
							else
							{
								throw new FormatException("Invalid header!");
							}
							//check code
							string code;
							using (Stream entry = archive.GetEntry(".text").Open())
							{
								using (StreamReader sr = new StreamReader(entry))
								{
									string SHA1 = GetSHA1(entry);
									if (header[2] != SHA1)
									{
										throw new FormatException($"Invalid Code! SHA1 in header ({header[2]}) did NOT match SHA1 of .text! ({SHA1})");
									}
								}
							}
							using (Stream entry = archive.GetEntry(".text").Open())
							{
								using (StreamReader sr = new StreamReader(entry))
								{
									code = sr.ReadToEnd();
								}
							}
							var lib = new _7sLibrary();
							lib.Content = code;
							return lib;
						}
						else
						{
							throw new FormatException("The 7sLib file did not have a .text file!");
						}
					}
					else
					{
						throw new FormatException("The 7sLib file did not have a .head file!");
					}
				}
			}
			catch (Exception e)
			{
				Utils.PrintError(e);
			}
			return null;
		}

		private static string GetSHA1(Stream s) => BitConverter.ToString(SHA1.Create().ComputeHash(s));

		private static string GetSHA1(string path)
		{
			string o;
			using (Stream fs = new FileStream(path, FileMode.Open))
			{
				o = BitConverter.ToString(SHA1.Create().ComputeHash(fs)); //dont return here let using exit to close stream			}
			}
			return o;
		}

		private static string WriteBytesAndGetSHA1(string path, byte[] bytes)
		{
			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				foreach (byte b in bytes)
				{
					fs.WriteByte(b);
				}
			}
			return GetSHA1(path);
		}

		private static string GetTempDir()
		{
			string dir;
			do
			{
				dir = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + Program.DirectorySeperator;
			}
			while (dir == null || Directory.Exists(dir));
			return dir;
		}
	}
}
