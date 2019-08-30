using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace GMAssetCompiler
{
	public class Trace
	{
		public static FileStream _storageFileStream;

		public static StreamWriter _streamWriter;

		public static string Filename
		{
			get;
			set;
		}

		static Trace()
		{
			string executablePath = Application.ExecutablePath;
			string directoryName = Path.GetDirectoryName(executablePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			string[] files = Directory.GetFiles(directoryName, "*.gz");
			TimeSpan t = new TimeSpan(30, 0, 0, 0, 0);
			string[] array = files;
			foreach (string text in array)
			{
				FileInfo fileInfo = new FileInfo(text);
				if (TimeSpan.Compare(DateTime.Now.Subtract(fileInfo.CreationTime), t) > 0)
				{
					File.Delete(text);
				}
			}
			Filename = Path.Combine(directoryName, "TraceGMA.log");
			if (File.Exists(Filename))
			{
				FileInfo fileInfo2 = new FileInfo(Filename);
				if (fileInfo2.Length > 204800)
				{
					string path = string.Format("trace-{0}.gz", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));
					using (FileStream fileStream2 = fileInfo2.OpenRead())
					{
						using (FileStream fileStream = new FileStream(Path.Combine(directoryName, path), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
						{
							using (GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Compress))
							{
								byte[] array2 = new byte[2048];
								bool flag = false;
								while (!flag)
								{
									fileStream2.Position = 0L;
									fileStream.Position = 0L;
									try
									{
										int num2;
										for (long num = 0L; num < fileInfo2.Length; num += num2)
										{
											num2 = fileStream2.Read(array2, 0, array2.Length);
											gZipStream.Write(array2, 0, num2);
										}
									}
									finally
									{
										gZipStream.Close();
										fileStream.Close();
										fileStream2.Close();
										flag = true;
									}
								}
							}
						}
					}
					File.Delete(Filename);
				}
			}
		}

		~Trace()
		{
			_storageFileStream.Close();
		}

		public static void Write(string message)
		{
			using (_storageFileStream = new FileStream(Filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
			{
				using (_streamWriter = new StreamWriter(_storageFileStream))
				{
					_streamWriter.AutoFlush = true;
					_streamWriter.WriteLine(message);
					_streamWriter.Flush();
					bool isAttached = Debugger.IsAttached;
				}
			}
		}

		public static void Write(string message, params object[] _arguments)
		{
			using (_storageFileStream = new FileStream(Filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
			{
				using (_streamWriter = new StreamWriter(_storageFileStream))
				{
					_streamWriter.AutoFlush = true;
					_streamWriter.WriteLine(message, _arguments);
					_streamWriter.Flush();
					bool isAttached = Debugger.IsAttached;
				}
			}
		}
	}
}
