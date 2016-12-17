using System.IO;

namespace ImageSorter
{
    class ImageInfo
    {
        public FileInfo Info { get; set; }
        public long ImageHash { get; set; }

        public ImageInfo(FileInfo info, long hash)
        {
            this.Info = info;
            this.ImageHash = hash;
        }
    }
}
