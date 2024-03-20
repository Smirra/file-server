FROM adminer:fastcgi

USER root
RUN apt-get update && apt-get install -y wget && apt-get install -y libfcgi-bin

RUN wget -O /usr/local/bin/php-fpm-healthcheck \
https://raw.githubusercontent.com/renatomefi/php-fpm-healthcheck/master/php-fpm-healthcheck \
&& chmod +x /usr/local/bin/php-fpm-healthcheck