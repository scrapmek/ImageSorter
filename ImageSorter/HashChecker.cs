using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageSorter
{
    class HashChecker
    {
        public List<ImageInfo> ImageInfoList { get; set; }
        private string directory { get; set; }

        public HashChecker(string rootDirectory)
        {
            directory = rootDirectory;
            ImageInfoList = new List<ImageInfo>();

            List<string> files = Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories).ToList();
            List<string> images = Helpers.GetAllImageFiles(files);

            foreach (string item in images)
            {
                FileInfo info = new FileInfo(item);
                long hash = Helpers.GenerateImageHash(info);
                ImageInfoList.Add(new ImageInfo(info, hash));
            }
            
        }

        public void AddNewImageInfo(FileInfo info)
        {
            long hash = Helpers.GenerateImageHash(info);
            ImageInfoList.Add(new ImageInfo(info, hash));

        }

        public bool CheckIfDuplicate(FileInfo file)
        {
            bool result = false;

            long perspectiveHash = Helpers.GenerateImageHash(file); 
            string perspectiveDestinationFolder = Helpers.GetDestinationDirectory(file, directory);
            List<ImageInfo> existingImagesInSameFolder = new List<ImageInfo>();

            foreach (ImageInfo item in ImageInfoList)
            {
                if (item.Info.DirectoryName == perspectiveDestinationFolder)
                    existingImagesInSameFolder.Add(item);
            }

            foreach (ImageInfo item in existingImagesInSameFolder)
            {
                if (perspectiveHash == item.ImageHash)
                {
                    result = true;
                    break;
                }

            }

            return result;
        }
    }
}
