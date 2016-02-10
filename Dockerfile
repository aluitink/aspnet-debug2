FROM microsoft/aspnet:1.0.0-rc1-finial

COPY aspnet-debug.Shared/project.json /opt/aspnet-debug/aspnet-debug.Shared/
COPY aspnet-debug.Server/project.json /opt/aspnet-debug/aspnet-debug.Server/

WORKDIR /opt/aspnet-debug/aspnet-debug.Shared
RUN ["dnu", "restore"]

WORKDIR /opt/aspnet-debug/aspnet-debug.Server
RUN ["dnu", "restore"]

COPY aspnet-debug.Shared /opt/aspnet-debug/aspnet-debug.Shared
COPY aspnet-debug.Server /opt/aspnet-debug/aspnet-debug.Server

EXPOSE 13001
ENTRYPOINT ["dnx", "-p", "project.json", "aspnet_debug.Server"]