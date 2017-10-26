FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app/Core3
EXPOSE 52831/tcp
ENV ASPNETCORE_URLS https://*:52831
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
ENTRYPOINT ["dotnet", "run"]