function clickChallenge(obj) {
    
    let classes = obj.classList;

    for (let i = 0; i < classes.length; i++) {
        if (classes[i] == "completed") {
            obj.classList.remove("completed");
            return;
        }
    }

    obj.classList.add("completed");
}

function getExpiryTime(challengeEndDateString) {
    let now = new Date();
    let end = new Date(challengeEndDateString);

    let millisecondsLeft = end - now;

    return msToHrsMin(millisecondsLeft);
}

function msToHrsMin( ms ) {
    // 1- Convert to seconds:
    let seconds = ms / 1000;
    // 2- Extract hours:
    let hours = parseInt( seconds / 3600 ); // 3,600 seconds in 1 hour
    seconds = seconds % 3600; // seconds remaining after extracting hours
    // 3- Extract minutes:
    let minutes = parseInt( seconds / 60 ); // 60 seconds in 1 minute
    // 4- Keep only seconds not extracted to minutes:
    seconds = seconds % 60;

    if (seconds >= 30) {
        minutes = minutes + 1;
    }

    return hours + " hrs, " + minutes + " min";
}