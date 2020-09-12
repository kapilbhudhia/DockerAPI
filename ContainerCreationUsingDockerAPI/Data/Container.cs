using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContainerCreationUsingDockerAPI.Data
{
    public class Container
    {
        public string Name { get; }
        public string State { get; }
        public string ID { get; }
        public string Port { get; }

        public Container(
            string name,
            string state,
            string id,
            string port)
        {
            this.Name = name;
            this.ID = id;
            this.State = state;
            this.Port = port;
        }

        public string Link => @"http://localhost:" + Port + @"/";
    }
}
