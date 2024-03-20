FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
ARG TARGETARCH

COPY src/Files.Api/*.csproj ./Files.Api/
COPY src/Shared/*.csproj ./Shared/

RUN dotnet restore ./Files.Api -a $TARGETARCH

COPY src/Files.Api ./Files.Api
COPY src/Shared ./Shared

WORKDIR /src/Files.Api

RUN dotnet publish -a $TARGETARCH --no-restore -o /app -c Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["./Files.Api"]