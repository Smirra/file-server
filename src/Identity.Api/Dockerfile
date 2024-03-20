FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
ARG TARGETARCH

COPY src/Identity.Api/*.csproj ./Identity.Api/
COPY src/Shared/*.csproj ./Shared/

RUN dotnet restore ./Identity.Api -a $TARGETARCH 

COPY src/Identity.Api ./Identity.Api
COPY src/Shared ./Shared

WORKDIR /src/Identity.Api

RUN dotnet publish -a $TARGETARCH --no-restore -o /app -c Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["./Identity.Api"]