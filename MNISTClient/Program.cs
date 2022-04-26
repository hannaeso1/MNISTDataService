using System;
using Grpc.Core;
using GrpcGreeter;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MNISTClient
{
    class Program
    {
        const string Target = "127.0.0.1:30052";
        const int ImageArrayDimention = 28;

        static List<Google.Protobuf.ByteString> GetImagesFromServer(int numberOfImages)
        {
            Channel channel = new Channel(Target, ChannelCredentials.Insecure);
            var client = new GrpcGreeter.MNISTDataService.MNISTDataServiceClient(channel);
            var  reply = client.GetImages(new GetImagesRequest { ImagesCount = numberOfImages });
            channel.ShutdownAsync().Wait();
            
            return reply.DigitImages.ToList();
        }
        static void WriteImagesToClient(List<Google.Protobuf.ByteString> digitImages)
        {
            foreach (var digitImage in digitImages)
            {
                for (int i = 0; i < ImageArrayDimention; i++)
                {
                    var data = digitImage
                        .Skip(i * ImageArrayDimention)
                        .Take(ImageArrayDimention)
                        .ToList();

                    var row1 = String.Join("", data
                        .Select(x => x.ToString("000")));

                    Console.Write(row1);
                    Console.Write("\t");

                    var row2 = String.Join("", data
                        .Select(x => x == 0 ? " " : "."));
                    Console.WriteLine(row2);
                }
                Console.WriteLine();
            }
        }
        async static Task StreamingImagesToClient(int numberOfImages)
        {
            Channel channel = new Channel(Target, ChannelCredentials.Insecure);
            var client = new GrpcGreeter.MNISTDataService.MNISTDataServiceClient(channel);
            var reply = client.ImagesList(new GetImagesRequest { ImagesCount = numberOfImages });

            await foreach (var response in reply.ResponseStream.ReadAllAsync())
            {
                WriteImagesToClient(response.DigitImages.ToList());
            }
        }
        async static Task Main(string[] args)
        {
            Console.WriteLine("Enter number of images:");
            int numberOfImages;
            while(!int.TryParse(Console.ReadLine(),out numberOfImages))
            {
                Console.WriteLine("Enter number of images:");
            }

            #region One request followed by one response connection.
            //var imagesList = GetImagesFromServer(numberOfImages);
            //WriteImagesToClient(GetImagesFromServer(numberOfImages)); 
            #endregion

            #region Single-sided streaming from server to client
            await StreamingImagesToClient(numberOfImages);
            #endregion

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
