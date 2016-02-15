FROM microsoft/aspnet:1.0.0-rc1-final

RUN apt-get update -qq && apt-get install -qqy \
    apt-transport-https \
    ca-certificates \
    curl \
    lxc \
    iptables \
	supervisor
    
# Install Docker from Docker Inc. repositories.
RUN curl -sSL https://get.docker.com/ | sh

# Install the magic wrapper.
ADD ./wrapdocker /usr/local/bin/wrapdocker
RUN chmod +x /usr/local/bin/wrapdocker


COPY aspnet-debug.Shared/project.json /opt/aspnet-debug/aspnet-debug.Shared/
COPY aspnet-debug.Server/project.json /opt/aspnet-debug/aspnet-debug.Server/

WORKDIR /opt/aspnet-debug/aspnet-debug.Shared
RUN ["dnu", "restore"]

WORKDIR /opt/aspnet-debug/aspnet-debug.Server
RUN ["dnu", "restore"]

COPY aspnet-debug.Shared /opt/aspnet-debug/aspnet-debug.Shared
COPY aspnet-debug.Server /opt/aspnet-debug/aspnet-debug.Server

# Supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 13001
VOLUME /var/lib/docker
CMD ["/usr/bin/supervisord"]