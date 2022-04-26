using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MNISTServer
{
   public static class DigitImage
    {
        const string MNISTDataSetFilePath = @".\train-images-idx3-ubyte\train-images.idx3-ubyte";
        const int ImageArrayDimention = 28;

        public static List<Google.Protobuf.ByteString> GetRandomDigitImages(int imagesCount)
        {
            List<Google.Protobuf.ByteString> imagesList = new List<Google.Protobuf.ByteString>();
            Random rnd = new Random();
            for (int i = 0; i < imagesCount; i++)
            {
                int index = rnd.Next(59000);

                using (FileStream fsImages = new FileStream(MNISTDataSetFilePath, FileMode.Open))
                {
                    using (BinaryReader brImages = new BinaryReader(fsImages))
                    {
                        //discard
                        int magic1 = brImages.ReadInt32();
                        int numImages = brImages.ReadInt32();
                        int numRows = brImages.ReadInt32();
                        int numCols = brImages.ReadInt32();

                        byte[] prevImages = brImages.ReadBytes(ImageArrayDimention * ImageArrayDimention * index);
                        byte[] b = brImages.ReadBytes(ImageArrayDimention * ImageArrayDimention);
                        //imagesList.Add(Google.Protobuf.ByteString.CopyFrom(b));
                        imagesList.Add(Google.Protobuf.UnsafeByteOperations.UnsafeWrap(b));
                    }
                }
            }

            return imagesList;
        }
    }
}
