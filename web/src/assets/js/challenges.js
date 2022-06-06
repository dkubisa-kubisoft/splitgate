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