FROM ubuntu:22.04

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y apache2 && \
    rm -rf /var/lib/apt/lists/*

RUN echo "Hello from Apache on Ubuntu!" > /var/www/html/index.html

EXPOSE 80
CMD ["apachectl","-D","FOREGROUND"]
