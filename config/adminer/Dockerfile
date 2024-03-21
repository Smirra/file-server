FROM adminer:fastcgi

USER root
RUN apt-get update && apt-get install -y libfcgi-bin
COPY --chmod=+x config/adminer/php-fpm-healthcheck /usr/local/bin/php-fpm-healthcheck