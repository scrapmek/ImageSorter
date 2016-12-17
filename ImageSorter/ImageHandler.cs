using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageSorter
{
    class ImageHandler : IDisposable
    {
        public string RootDestinationDirectory { get; set; }
        public string InputFile { get; set; }

         

        public ImageHandler(string input, string output)
        {
            InputFile = input;
            RootDestinationDirectory = output;
        }

        public bool Transfer()
        {
            bool result = false;
            if (checkDirectoryExists(InputFile))
            {
                if (!checkIfImageIsDuplicate(InputFile))
                {
                    string fileDestinationPath = createUniqueFilePath(InputFile);
                    File.Copy(InputFile, fileDestinationPath);
                    result = true;
                }
                else result = false;
            }
            else
            {
                string fileDestinationPath = createUniqueFilePath(InputFile);
                Directory.CreateDirectory(generateDestinationDirectory(new FileInfo(InputFile)));
                File.Copy(InputFile, fileDestinationPath);
                result = true;
            }

            return result;


        }

        private bool checkIfImageIsDuplicate(string path)
        {
            bool result = false;
            long originalHash = generateImageHash(path);
            FileInfo info = new FileInfo(path);
            string directory = generateDestinationDirectory(info);
            List<string> files = Directory.EnumerateFiles(directory).ToList();
            List<string> images = new List<string>();
            foreach (string item in files)
            {
                if (checkFileIsImage(item))
                    images.Add(item);
            }

            List<long> hashes = new List<long>();

            foreach (string item in images)
            {
                hashes.Add(generateImageHash(item));
            }

            if (hashes.Contains(originalHash))
                result = true;

            return result;

        }

        public long generateImageHash(string imagePath)
        {
            long result = 17;
            using (Bitmap image = new Bitmap(imagePath))
                using(Bitmap fingerprint = new Bitmap(image, new Size(32,32)))
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

        private string createUniqueFilePath(string item)
        {
            FileInfo info = new FileInfo(item);
            string originalName = Path.GetFileNameWithoutExtension(item);
            string perspectiveName = originalName;
            string directory = generateDestinationDirectory(info);

            int i = 1;

            while (File.Exists(Path.Combine(directory, perspectiveName) + info.Extension))
            {
                perspectiveName = originalName + " (" + i + ")";
                i++;
            }

            return Path.Combine(directory, perspectiveName) + info.Extension;
        }

        private bool checkDirectoryExists(string path)
        {
            bool result = false;
            FileInfo info = new FileInfo(path);

            if (Directory.Exists(generateDestinationDirectory(info)))
                result = true;

            return result;
        }

        private string generateDestinationDirectory(FileInfo file)
        {
            DateTime creationDate = determineLikelyCreationTime(file);
            string year = creationDate.Year.ToString();
            string month = convertMonthNumberToName(creationDate.Month);

            return Path.Combine(RootDestinationDirectory, year, month);
        }

        private DateTime determineLikelyCreationTime(FileInfo fileInfo)
        {
            DateTime time = fileInfo.CreationTime;
            if (time > fileInfo.LastAccessTime)
            {
                time = fileInfo.LastAccessTime;
            }
            if (time > fileInfo.LastWriteTime)
            {
                time = fileInfo.LastWriteTime;
            }

            return time;
        }

        private string convertMonthNumberToName(int monthNumber)
        {
            string name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);

            return name;
        }

        public static List<string> getAllInputImageFiles(string directory)
        {
            List<string> files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList();

            List<string> images = new List<string>();

            foreach (string item in files)
            {
                if (checkFileIsImage(item))
                    images.Add(item);
            }

            return images;
        }

        private static bool checkFileIsImage(string filePath)
        {
            bool result = false;
            string extension = Path.GetExtension(filePath).ToUpper();
            List<string> filetypes = new List<string> {
            ".JPG",
            ".JPEG",
            ".JIF",
            ".JFIF",
            ".PNG",
            ".BMP",
            ".TIFF",
            ".TIF",
            ".GIF"};


            if (filetypes.Contains(extension))
            {
                result = true;
            }

            return result;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
