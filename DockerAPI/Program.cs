using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await CallDockerAPI();
        }

        static async Task CallDockerAPI()
        {
            DockerClient client = new DockerClientConfiguration()
                .CreateClient();


            try
            {
                IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        Limit = 10,
                    });

                foreach (var container in containers)
                {
                    Console.WriteLine(container.Names[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
