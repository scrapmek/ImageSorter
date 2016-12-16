using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageSorter
{
    class ImageHandler
    {
        public Dictionary<string, List<long>> hashDictionary { get; set; }
        private string rootDirectory { get; set; }

        public ImageHandler(string rootBackupDirectory)
        {
            rootDirectory = rootBackupDirectory;
            hashDictionary = new Dictionary<string, List<long>>();
            generateHashDictionary();
        }

        public void UpdateHashDictionary(string newImageFile)
        {
            string imageFileDirectory = Path.GetDirectoryName(newImageFile);
            if (hashDictionary.ContainsKey(imageFileDirectory))
            {
                hashDictionary[imageFileDirectory].Add(generateImageHash(newImageFile));
            }
            else hashDictionary.Add(imageFileDirectory, new List<long> { generateImageHash(newImageFile) });
        }

        private void generateHashDictionary()
        {
            List<string> folders = Directory.EnumerateDirectories(rootDirectory, "", SearchOption.AllDirectories).ToList();

            foreach (string item in folders)
            {
                List<string> imageFiles = getImageFiles(item);

                hashDictionary.Add(item, new List<long>());

                foreach (string imageFile in imageFiles)
                {
                    hashDictionary[item].Add(generateImageHash(imageFile));
                }
            }

        }

        public static List<string> findAllImages(string sourceFolder)
        {
            List<string> folders = Directory.EnumerateDirectories(sourceFolder, "", SearchOption.AllDirectories).ToList();
            folders.Add(sourceFolder);
            List<string> imageFiles = new List<string>();

            foreach (string item in folders)
            {
                imageFiles.AddRange(getImageFiles(item));
            }

            return imageFiles;
        }

        private static List<string> getImageFiles(string path)
        {
            List<string> pendingPathList = Directory.EnumerateFiles(path).ToList();
            List<string> imagePathList = new List<string>();


            foreach (string item in pendingPathList)
            {
                if (checkFileIsImage(item)) imagePathList.Add(item);
            }

            return imagePathList;
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

        public long generateImageHash(string imagePath)
        {
            long result = 17;
            using (Bitmap image = new Bitmap(imagePath))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        Color pixelColour = image.GetPixel(i, j);
                        result = result * 13 + (int)pixelColour.A;
                        result = result * 13 + (int)pixelColour.B;
                        result = result * 13 + (int)pixelColour.G;
                        result = result * 13 + (int)pixelColour.R;
                    }
                }
            }

            return result;
        }

        public string generateFullDestinationDirectory(string imagePath)
        {
            string directory = rootDirectory;
            FileInfo currentFileInfo = new FileInfo(imagePath);
            DateTime creationTime = determineLikelyCreationTime(currentFileInfo);
            

            directory += "/" + creationTime.Year.ToString();
            directory += "/" + convertMonthNumberToName(creationTime.Month);

            return directory;
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

        public string generateDestinationPath(string imagePath)
        {

            string destinationFolder = generateFullDestinationDirectory(imagePath);
            FileInfo info = new FileInfo(imagePath);
            string originalName = Path.GetFileNameWithoutExtension(imagePath);
            string candidateName = originalName;
            int i = 1;

            while (File.Exists(Path.Combine(destinationFolder, candidateName) + info.Extension))
            {
                candidateName = originalName + " (" + i + ")";
                i++;
            }

            return Path.Combine(destinationFolder, candidateName) + info.Extension;
        }
    }
}
