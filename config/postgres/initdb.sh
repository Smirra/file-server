#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 <<-EOSQL
	CREATE DATABASE files;
	CREATE DATABASE identity;
	CREATE DATABASE fskeys;
	CREATE USER files WITH PASSWORD '$FILESERVER_FILES_DBPW';
   	CREATE USER identity WITH PASSWORD '$FILESERVER_IDENTITY_DBPW';
	GRANT ALL PRIVILEGES ON DATABASE files TO files;
	GRANT ALL PRIVILEGES ON DATABASE identity TO identity;
	GRANT ALL PRIVILEGES ON DATABASE fskeys TO files, identity;
	\c fskeys;
	GRANT ALL ON SCHEMA public TO files, identity;
	\c files;
	GRANT ALL ON SCHEMA public TO files;
	\c identity;
	GRANT ALL ON SCHEMA public TO identity;
EOSQL

psql -v ON_ERROR_STOP=1 --username "files" --dbname "fskeys" <<-EOSQL
	ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO identity;
EOSQL

psql -v ON_ERROR_STOP=1 --username "identity" --dbname "fskeys" <<-EOSQL
	ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO files;
EOSQL