using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Drawing;
using ImageSorter;
using System.Linq;

public class FileHandler
{
    
    
    public static void TransferImage(string imageFilePath, string destinationDirectory)
    {
        string destinationFolder = generateFullDestinationDirectory(imageFilePath, destinationDirectory);
        string fullDestinationPath = generateDestinationPath(destinationFolder, imageFilePath);

        File.Copy(imageFilePath, fullDestinationPath);
    }

    //private static void preserveDateModifiedData(string originalImageFilePath, string copiedImageFilePath)
    //{
    //    DateTime modifiedTime = determineLikelyCreationTime(new FileInfo(originalImageFilePath));

    //    FileInfo newFileInfo = new FileInfo(copiedImageFilePath);
    //    newFileInfo.IsReadOnly = false;
    //    newFileInfo.LastWriteTime = modifiedTime;
    //}

    public static bool DetermineDuplicate(string imageFilePath, string destinationFolder)
    {
        bool result = false;

        string fullDestinationDirectory = generateFullDestinationDirectory(imageFilePath, destinationFolder);
        Directory.CreateDirectory(fullDestinationDirectory);

        string[] fileList = Directory.GetFiles(fullDestinationDirectory);

        FileInfo info = new FileInfo(imageFilePath);
        string path = Path.Combine(generateFullDestinationDirectory(imageFilePath, destinationFolder), info.Name);

        if (File.Exists(path))
        {
            if (determineIfDuplicateImages(imageFilePath, path))
            {
                result = true;
            }
        }

        if (result == false)
        {
            for (int i = 0; i < fileList.Length; i++)
            {
                if (checkFileIsImage(fileList[i]))
                {
                    if (determineIfDuplicateImages(imageFilePath, fileList[i]))
                    {
                        result = true;
                        break;
                    }
                }
            }
        }

        return result;
    }

    
    public static bool CheckForDuplicate(string path)
    {
        bool result = false;
        List<string> images = Directory.GetFiles(path).ToList();
        return result;

        }

    
    public static List<string> PopulateImageList(string sourceFolderPath)
    {
        List<string> pendingPathList = findAllFiles(sourceFolderPath);
        List<string> imagePathList = new List<string>();


        foreach (string item in pendingPathList)
        {
            if (checkFileIsImage(item)) imagePathList.Add(item);
        }

        return imagePathList;
    }

    private static List<string> findAllFiles(string sourceFolderPath)
    {
        List<string> filePathList = new List<string>();

        foreach (string file in Directory.EnumerateFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories))
        {
            filePathList.Add(file);
        }

        return filePathList;
    }

    private static List<string> convertStringArrayToList(string[] v)
    {
        List<string> list = new List<string>();

        for (int i = 0; i < v.Length; i++)
        {
            list.Add(v[i]);
        }

        return list;
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

    

    

    

    private static bool checkForFilenameClash(string destinationFolder, string filename)
    {
        string[] file = Directory.GetFiles(destinationFolder, filename);

        if (file == null) return false;
        else return true;
    }

    

    enum MonthName
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    enum FileExtension
    {
        JPG,
        JPEG,
        JIF,
        JFIF,
        PNG,
        BMP,
        TIFF,
        TIF,
        GIF,
    }
}
