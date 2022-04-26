using System;
using Grpc.Core;
using System.Threading.Tasks;
using GrpcGreeter;
using System.IO;
using System.Collections.Generic;

namespace MNISTServer
{
    public class GreeterImpl : GrpcGreeter.MNISTDataService.MNISTDataServiceBase
    {
        private const int ChunkImagesCount = 5;
        public override async Task ImagesList(GetImagesRequest request, IServerStreamWriter<GetImagesReply> responseStream, ServerCallContext context)
        {
            int totalImagesSend = 0;
            while (totalImagesSend < request.ImagesCount)
            {
                GetImagesReply reply = new GetImagesReply();
                int currentChunk = request.ImagesCount - totalImagesSend >= ChunkImagesCount? ChunkImagesCount: request.ImagesCount - totalImagesSend;
                reply.DigitImages.AddRange(DigitImage.GetRandomDigitImages(currentChunk));
                await responseStream.WriteAsync(reply);
                totalImagesSend += currentChunk;
            }
        }

        public override Task<GetImagesReply> GetImages(GetImagesRequest request, ServerCallContext context)
        {

            GetImagesReply reply = new GetImagesReply();
            reply.DigitImages.AddRange(DigitImage.GetRandomDigitImages(request.ImagesCount));
            return Task.FromResult(reply);
        }
        class Program
        {
            const int Port = 30052;
            const string Server = "localhost";
            public static void Main(string[] args)
            {
                Server server = new Server
                {
                    Services = { GrpcGreeter.MNISTDataService.BindService(new GreeterImpl()) },
                    Ports = { new ServerPort(Server, Port, ServerCredentials.Insecure) }
                };
                server.Start();

                Console.WriteLine("MNIST server listening on port " + Port);
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();

                server.ShutdownAsync().Wait();
            }
        }
    }
}
