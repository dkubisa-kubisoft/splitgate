# Splitgate Challenges Site
The Splitgate Challenges site is a basic website that lists off the current daily, weekly and seasonal challenges for [Splitgate](https://www.splitgate.com/), a fast-paced, cross-platform PVP Portal Shooter. This project contains website itself and all of the underlying and supporting infrastructure.

### Website
The website is a basic website written in [Angular](https://angular.io/). It renders the current daily, weekly and seasonal Splitgate challenges that it gets from the API.

### API
The API exposes the basic mechanisms to create, update and retrieve the challenges used by the site. It is written in C# and hosted in a function app via [Microsoft Azure Functions](https://azure.microsoft.com/en-us/services/functions/?&ef_id=Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB:G:s&OCID=AID2200277_SEM_Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB:G:s&gclid=Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB#overview). 

### Storage
All challenge and challenge completion data is stored here via [Microsoft Azure Table Storage](https://azure.microsoft.com/en-us/services/storage/tables/?&ef_id=Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB:G:s&OCID=AID2200277_SEM_Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB:G:s&gclid=Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB) tables. 

### Scraper
The scraper is a Python script that runs Splitgate and obtains the challenges directly from the UI via Optical Character Recognition. Once obtained, the challenges are sent to storage via the API.
