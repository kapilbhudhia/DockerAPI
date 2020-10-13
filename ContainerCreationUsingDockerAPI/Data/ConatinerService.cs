using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace ContainerCreationUsingDockerAPI.Data
{
    public class ContainerService
    {
        private Random randomPortGenerator = new Random();
        private DockerClient _client = null;
        private string mongoDbimage = "klingelnberg.azurecr.io/gearengine/mongo:4.0";
        public ContainerService()
        {
            _client = new DockerClientConfiguration().CreateClient();
        }
        public async Task<List<Container>> GetContainersAsync()
        {
            try
            {
                string imageName = "vad1mo/hello-world-rest:latest";
                List<Container> containers = new List<Container>();
                IList<ContainerListResponse> containersResponse = await _client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        //Limit = 10,
                        Filters = new Dictionary<string, IDictionary<string, bool>>()
                        {
                            {
                                "ancestor", new Dictionary<string, bool>()
                                {
                                    {mongoDbimage, true}
                                }
                            }
                        }
                    });

                foreach (var container in containersResponse)
                {
                    containers.Add(new Container(
                        id: container.ID,
                        name: container.Names[0],
                        state: container.State,
                        port: container.Ports[0].PublicPort.ToString()));
                }

                return containers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task StopContainerAsync(string ID)
        {
            try
            {
                var stopResponse = await _client.Containers.StopContainerAsync(ID,
                    new ContainerStopParameters()
                    {
                        WaitBeforeKillSeconds = 5
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Could not stop the running container");
            }

        }

        public async Task CreateNewAsync()
        {
            await TestMongoDbBackup();

            try
            {
                string randomPort = randomPortGenerator.Next(50000, 60000).ToString();
                var containerResponse = await _client.Containers.CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = "vad1mo/hello-world-rest:latest",
                    HostConfig = new HostConfig()
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>()
                        {
                            { "5050/tcp",  new List<PortBinding>()
                            {
                                new PortBinding(){HostPort = randomPort}
                            }}
                        }
                    }

                }, CancellationToken.None);


                bool started = await _client.Containers.StartContainerAsync(containerResponse.ID, null);

                //var inspectionResponse = await _client.Containers.InspectContainerAsync(containerResponse.ID);
                //containers.Add(
                //    new Container()
                //    {
                //        Name = inspectionResponse.Name,
                //        State = inspectionResponse.State,
                //        ID = inspectionResponse.ID,
                //        Port = inspectionResponse.HostConfig.PortBindings.Values.ElementAt(0)[0].HostPort
                //    });

                //containerCount += 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task TestMongoDbBackup()
        {
            try
            {
                IList<ContainerListResponse> containersResponse = await _client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        //Limit = 10,
                        Filters = new Dictionary<string, IDictionary<string, bool>>()
                        {
                            {
                                "ancestor", new Dictionary<string, bool>()
                                {
                                    {
                                        mongoDbimage, true
                                    }
                                }
                            }
                        }
                    });


                // "/bin/sh" is usually a symlink to a shell
                // -c string If  the  -c  option  is  present, then commands are read from string.
                var execResponse = await _client.Exec.ExecCreateContainerAsync(
                    containersResponse.First().ID,//containerResponse.ID,
                    new ContainerExecCreateParameters()
                    {
                        Cmd = new[]
                        {
                            "/bin/sh", "-c",  " mkdir -p /dump/yyyyMMdd_HHmmss123456",

                        }
                    });

                await _client.Exec.StartContainerExecAsync(execResponse.ID);

                var execResponse2 = await _client.Exec.ExecCreateContainerAsync(
                    containersResponse.First().ID,//containerResponse.ID,
                    new ContainerExecCreateParameters()
                    {
                        Cmd = new[]
                        {
                            "/bin/sh", "-c",  " mongodump  --forceTableScan --db CorrectionLoop --out dump/yyyyMMdd_HHmmss123456"
                        }
                    });

                await _client.Exec.StartContainerExecAsync(execResponse2.ID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}