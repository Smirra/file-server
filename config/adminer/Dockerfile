FROM adminer:fastcgi

USER root
RUN apt-get update && apt-get install -y libfcgi-bin
COPY config/adminer/php-fpm-healthcheck /usr/local/bin/php-fpm-healthcheck
RUN chmod +x /usr/local/bin/php-fpm-healthcheck