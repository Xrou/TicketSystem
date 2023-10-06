var groupId;

function loadGroup() {
    var urlParams = new URLSearchParams(window.location.search);
    groupId = urlParams.get('id');

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);

            document.getElementById("id_input").value = data["id"];
            document.getElementById("name_input").value = data["name"];
        }
    });

    xhr.open("GET", "api/userGroups/" + groupId);
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function saveGroup() {
    // WARNING: For POST requests, body is set to null by browsers.
    var data = JSON.stringify({
        "name": document.getElementById("name_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4  && this.status == 200) {
            location.replace("../groups");
        }
    });

    xhr.open("POST", "api/userGroups/" + groupId);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

loadGroup();