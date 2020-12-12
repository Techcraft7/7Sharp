using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace _7Sharp._7sLib
{
	internal class _7sLibManager
	{
		const int LIBVERSION = 1;

		internal static void Save(string path)
		{
			try
			{
				using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Update))
				{
					ZipArchiveEntry dotText = zip.CreateEntry(".text");
					ZipArchiveEntry dotHead = zip.CreateEntry(".head");
					using (Stream text = dotText.Open())
					{
						string sha1Text = WriteAndGetSHA1(text, Program.shell.GetCode());
						using (Stream head = dotHead.Open())
						{
							byte[] buf = Encoding.UTF8.GetBytes("7SLIB\nV" + LIBVERSION + "\n" + sha1Text);
							head.Write(buf, 0, buf.Length);
						}
					}
				}
			}
			catch (Exception e)
			{
				Utils.PrintError(e);
			}
		}

		private static string WriteAndGetSHA1(Stream s, string contents)
		{
			byte[] buf = Encoding.UTF8.GetBytes(contents);
			s.Write(buf, 0, buf.Length);
			return GetSHA1(contents);
		}

		internal static _7sLibrary Load(string path)
		{
			try
			{
				using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
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
								if (!header[1].StartsWith("V") || !int.TryParse(header[1].Substring(1), out int v))
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
							_7sLibrary lib = new _7sLibrary
							{
								Content = code
							};
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
	}
}
