

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY AOTDemo04_docker.csproj .
RUN dotnet restore --runtime linux-musl-x64 AOTDemo04_docker.csproj

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev


COPY . .
RUN dotnet publish -c Release -r linux-musl-x64 -o out AOTDemo04_docker.csproj

#debian,alpine,scratch,mcr.microsoft.com/dotnet/runtime-deps:8.0 
#FROM debian  AS runtime
FROM  mcr.microsoft.com/dotnet/runtime-deps:8.0   AS runtime
WORKDIR /app
COPY --from=build /app/out/* /app/
EXPOSE 5000

ENTRYPOINT ["/app/AOTDemo04_docker"]