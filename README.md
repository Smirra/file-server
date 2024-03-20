# File Server
### A full stack file hosting service written in .Net Core and Next.js!
<br/>

# Environment variables
All apps, as well as docker compose, can utilise _.env_-files. They are only intended for local development. In production environments, variables should be set directly in the environment.
_.env_ is intended for values that can't be committed to the repo, like passwords and other secrets. _.env.dev_ is intended for development values that can be commited. It's also noteworthy that docker compose automatically interpolates env variables from the _.env_ file, which is useful when we are building images locally.

> * POSTGRES_PASSWORD
> * FILESERVER_IDENTITY_DBPW
> * FILESERVER_FILES_DBPW
> * FILESERVER_OBFUSCATE_KEY
>   * Can be any alphanumerical utf-8 string of length 32 (32 bytes)
> * FILESERVER_OBFUSCATE_IV
>   * Can be any alphanumerical utf-8 string of length 16 (16 bytes)
> * FILESERVER_SUPERUSER_PW
> * FILESERVER_TESTUSER_PW
> * NEXT_PUBLIC_OBFUSCATE_IV=$FILESERVER_OBFUSCATE_IV
> * NEXT_PUBLIC_OBFUSCATE_KEY=$FILESERVER_OBFUSCATE_KEY
 
 Below are an minimal example of how to set the db connection strings.
> * FILESERVER_ConnectionStrings__FILES="Host=localhost;Port=5432;Database=files;User ID=files;Password=$FILESERVER_FILES_DBPW"
> * FILESERVER_ConnectionStrings__IDENTITY="Host=localhost;Port=5432;Database=identity;User ID=identity;Password=$FILESERVER_IDENTITY_DBPW"
> * FILESERVER_ConnectionStrings__KEYS_FILES="Host=localhost;Port=5432;Database=fskeys;User ID=files;Password=$FILESERVER_FILES_DBPW"
> * FILESERVER_ConnectionStrings__KEYS_IDENTITY="Host=localhost;Port=5432;Database=fskeys;User ID=identity;Password=$FILESERVER_IDENTITY_DBPW"

To run or build the production images locally, add these overriding connection strings to your _.env_ file. 
> * DOCKER_ConnectionStrings__FILES="Host=db;Port=5432;Database=files;User ID=files;Password=$FILESERVER_FILES_DBPW"
> * DOCKER_ConnectionStrings__IDENTITY="Host=db;Port=5432;Database=identity;User ID=identity;Password=$FILESERVER_IDENTITY_DBPW"
> * DOCKER_ConnectionStrings__KEYS_FILES="Host=db;Port=5432;Database=fskeys;User ID=files;Password=$FILESERVER_FILES_DBPW"
> * DOCKER_ConnectionStrings__KEYS_IDENTITY="Host=db;Port=5432;Database=fskeys;User ID=identity;Password=$FILESERVER_IDENTITY_DBPW"

# Dev-certs
Run `dotnet dev-certs https --trust` on pc or mac.

To be able to run the development docker containers (postgres etc) you need to put the dev-cert into `./certs/`, so run <br/>`dotnet dev-certs https --trust -ep ./certs/localhost -p <somepass>`

For linux environments:<br/>
https://github.com/BorisWilhelms/create-dotnet-devcert

# JWT-cert
There are many ways to do this. For example:
```
openssl genrsa -out private.pem 2048
openssl rsa -pubout -in private.pem -out public.pem
```
in `./certs`


# Run the project locally
In local development run the db and reverse proxy before running the other micro-services:<br/>
`docker compose -f docker-compose.dev.yml`

Launch them through vscode or with command-line in their respective folders.
```
dotnet run -p ./src/Files.Api
dotnet run -p ./src/Identity.Api
```

```
cd ./frontend && npm run dev
```

# Docker
To run everything in docker just do<br/>
`docker compose up`

## Build
### OS default image:
`docker compose build`

### Multi arch images:
Use a multi-arch builder (QEMU, remote or something else).<br/>
`docker buildx bake`