# Splitgate Challenges Site
[Splitgate](https://www.splitgate.com/) is a fast-paced, cross-platform PVP Portal Shooter. This independent project is a basic website that lists and tracks the progress of the current daily, weekly and seasonal challenges offered in Splitgate. **This site does not in any way integrate with your actual challenge progress.** Think of it as a checklist: It allows you to view and track which challenges you have completed as you play the game. It is intended to be used on your phone or in a browser while you're in-game, allowing you to keep track of which challenges you'd like to work on without requiring you to drop from matchmaking to check your progress. You can also use it as a quick and convenient way to check on the current day's challenges from any web-enabled device without needing to lauch the game.

Challenge progress is currently saved and displayed by IP address, so if you switch to a different internet connection or your IP address changes, your challenge progress will be lost. Steam login is currently being explored as a more reliable alternative to IP-based user management.

This project contains website itself and all of the underlying and supporting infrastructure. The authors of this project are not affiliated with Splitgate or 1047 Games in any official capacity; we're just fans of the game looking to get the most out of our Splitgate experience.

### Website
The website renders the current daily, weekly and seasonal Splitgate challenges and enables users to mark challenges as completed as they progress through the challenges. Challenge progress is saved by IP address and automatically resets as new challenges replace existing ones. It is written using the [Angular](https://angular.io/) framework.

### API
The API exposes the basic mechanisms to create, update and retrieve the challenges used by the site. It is written in C# and hosted in a function app via [Microsoft Azure Functions](https://azure.microsoft.com/en-us/services/functions/?&ef_id=Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB:G:s&OCID=AID2200277_SEM_Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB:G:s&gclid=Cj0KCQjwhLKUBhDiARIsAMaTLnEVv3p7zKlhPNc6nHOX3khInmyY1SUY6IuBXW-QYZ-N-Vkha1RtkzEaAnJxEALw_wcB#overview). 

### Storage
All challenge and challenge completion data is stored in [Microsoft Azure Table Storage](https://azure.microsoft.com/en-us/services/storage/tables/?&ef_id=Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB:G:s&OCID=AID2200277_SEM_Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB:G:s&gclid=Cj0KCQjwhLKUBhDiARIsAMaTLnFMryPUE7Ck9UdGuWc5KF3KUkP5nLA0XeykNhgSsaVx2c-5pBjBFn8aAj7fEALw_wcB) tables. 

### Scraper
The scraper is a Python script that runs the game and obtains the challenges directly from the UI via Optical Character Recognition. Once obtained, the challenges are sent to storage via the API.
