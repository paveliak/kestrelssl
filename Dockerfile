FROM ubuntu AS build-env

WORKDIR /app

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y dotnet-sdk-7.0
COPY *.csproj *.cs *.json ./
RUN dotnet publish -c Release -o out

FROM ubuntu
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
COPY --from=build-env --chown=${USER}:${GROUP} /app/out .

USER ${USER}
ENTRYPOINT ["./kestrelssl"]