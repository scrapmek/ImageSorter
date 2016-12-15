using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    static class ImageFingerprint
    {
        public static bool CheckForIdenticalImageFingerprint(Image image1, Image image2 , int precision)
        {
            bool result = true;

            using (Bitmap a = generateImageFingerprint(image1, precision))
            using (Bitmap b = generateImageFingerprint(image2, precision))
            {

                for (int x = 0; x < a.Width; x++)
                {
                    for (int y = 0; y < a.Height; y++)
                    {
                        Color aPixelColour = a.GetPixel(x, y);
                        Color bPixelColour = b.GetPixel(x, y);
                        if (aPixelColour != bPixelColour)
                        {
                            result = false;
                            break;
                        }
                    }
                    if (!result)
                    {
                        break;
                    }
                }
            }
            
            return result;


        }

        

        private static Bitmap generateImageFingerprint(Image image, int precision)
        {
            int fingerprintWidth = precision;
            int fingerprintHeight = precision;

            Bitmap fingerprint = new Bitmap(image, fingerprintWidth, fingerprintHeight);

            return fingerprint;
            
        }

    }
}
