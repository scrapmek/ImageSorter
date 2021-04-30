using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageSorter
{
	static class Helpers
	{
		public static IEnumerable<string> ImageExtensions =>
			new string[]
			{
				".JPG",
				".JPEG",
				".JIF",
				".JFIF",
				".PNG",
				".BMP",
				".TIFF",
				".TIF",
				".GIF"
			};



		public static bool IsImage(this FileInfo file) => ImageExtensions.Any(x => x == file.Extension);

		public static IEnumerable<FileInfo> GetAllInputImageFiles(string directory) => Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(x => new FileInfo(x)).Where(x => x.IsImage());

		public static IEnumerable<FileInfo> GetAllImageFiles(IEnumerable<FileInfo> files) => files.Where(x => x.IsImage());

		public static long GenerateImageHash(FileInfo imageInfo)
		{
			long result = 17;
			int fingerprintSize = 32;

			using (Bitmap image = new Bitmap(imageInfo.FullName))
			using (Bitmap fingerprint = new Bitmap(image, new Size(fingerprintSize, fingerprintSize)))
			{

				for (int i = 0; i < fingerprint.Width; i++)
				{
					for (int j = 0; j < fingerprint.Height; j++)
					{
						Color pixelColour = fingerprint.GetPixel(i, j);
						result = result * 13 + (int)pixelColour.A;
						result = result * 13 + (int)pixelColour.B;
						result = result * 13 + (int)pixelColour.G;
						result = result * 13 + (int)pixelColour.R;
					}
				}
			}
			return result;
		}

		public static DateTime DetermineLikelyCreationTime(FileInfo fileInfo)
		{
			List<DateTime> times =
				new List<DateTime>();

			if (fileInfo.CreationTime > new DateTime(2000, 1, 1))
				times.Add(fileInfo.CreationTime);

			if (fileInfo.LastWriteTime > new DateTime(2000, 1, 1))
				times.Add(fileInfo.LastWriteTime);

			if (fileInfo.LastAccessTime > new DateTime(2000, 1, 1))
				times.Add(fileInfo.LastAccessTime);

			return times.OrderBy(x => x)?.FirstOrDefault() ?? new DateTime();
		}

		public static string GetDestinationDirectory(FileInfo file, string rootDirectory)
		{
			DateTime creationDate = Helpers.DetermineLikelyCreationTime(file);
			string year = creationDate.Year.ToString();
			string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationDate.Month);
			month = month.First().ToString().ToUpper() + month.Substring(1);

			return Path.Combine(rootDirectory, year, month);
		}

		public static void preserveCreationTime(FileInfo originalFile, FileInfo newfile)
		{
			newfile.IsReadOnly = false;
			newfile.CreationTime = originalFile.CreationTime;
		}
	}
}
