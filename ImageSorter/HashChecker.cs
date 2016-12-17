using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    class HashChecker
    {
        public List<ImageInfo> ImageInfoList { get; set; }

        public HashChecker()
        {
            ImageInfoList = new List<ImageInfo>();
        }

        public void AddNewImageInfo(string path, long hash)
        {
            ImageInfoList.Add(new ImageInfo(path, hash));
        }

        public bool CheckIfDuplicate(string path)
        {
            FileInfo info = new FileInfo(path);
            return true;
        }
    }
}
