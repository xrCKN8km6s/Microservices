# Prerequisites
* Docker
* NET Core 2.2 SDK
* Node
* Angular `npm install -g @angular/cli`

# Microservices information:
Microservice | Url | Swagger
--- | --- | ---
Identity | http://localhost:3000 | NA
Web | http://localhost:4200 | NA
BFF | http://localhost:5000 | http://localhost:5000/swagger
Users | http://localhost:5100 | http://localhost:5100/swagger
Orders | http://localhost:5200 | http://localhost:5200/swagger

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 

# Initial dev setup
* Open Solution directory
* Run:
  * `docker-compose up -d`
  * `seed_db.bat` (Windows) or `./seed_db.sh` (Linux)

# Back-end development setup
* Open solution and set multiple startup projects to:
  * BFF
  * Orders
  * Users
  * Identity
* Start

# Front-end development setup
* Open Solution directory
* Run `docker-compose -f docker-compose.yml -f docker-compose.local.yml up -d`
* Go to `/Web` and run `ng serve --open`
* Navigate to Web url
* Login using username/password provided above
