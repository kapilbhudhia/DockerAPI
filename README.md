# DockerAPI
This application serves as a small demonstration for accessing the Docker REST API using the **Docker.DotNet** nuget package

### Pre-requisite
The application creates new containers for the docker image : **vad1mo/hello-world-rest**, so make sure that this image is already pulled to your local docker repository.

use the following command to pull the Image:

```docker pull vad1mo/hello-world-rest:latest```

### Building/Running the application
The application is a very minimalistic Balzor app and is supposed to be built as a docker image. The best way would be to open the solution file in Visual Studio 19.0 and run the application 
in either Debug or Release mode selecting Docker as the runtime configuration.

### Running the application standalone (not using Visual studio)
- You must first build the application using docker build command - please refer to the docker documentation for more help regarding this.
- An alternate way would be to run the application in Release mode once using Visual Studio, this should create an image **containercreationusingdockerapi:latest** 
in the local docker repository. You can use the following image subsequently to run the docker container from the command line
- Use the following command from a wsl linux terminal (git-bash would also do) to start the container
```docker run -dt -p 5000:80 -v "/var/run/docker.sock:/var/run/docker.sock" -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:80" -P --name containercreationusingdockerapi_manual containercreationusingdockerapi:latest```

- The application makes REST Api calls to the docker daemon using the unix socket so make sure that in the above command the daemon socket is volume mounted for the application to connect to 

### Report Issues/ Request Enhancements
Please contact kapilbhudhia@gmail.com for raising any issues or for adding more features.
