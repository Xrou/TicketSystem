var groupId;
var companiesInGroup = {};
var topicsInGroup = {};

var saveChanges = { "topics": {}, "companies": {} };

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

            for (var i = 0; i < data["companies"].length; i++) {
                companiesInGroup[data["companies"][i]["name"]] = data["companies"][i];
            }

            for (var i = 0; i < data["topics"].length; i++) {
                topicsInGroup[data["topics"][i]["name"]] = data["topics"][i];
            }

            loadTopicsAndCompanies();
        }
    });

    xhr.open("GET", "api/userGroups/" + groupId);
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function saveGroup() {
    // WARNING: For POST requests, body is set to null by browsers.
    var data = JSON.stringify({
        "name": document.getElementById("name_input").value,
        "topicsChanges": saveChanges["topics"],
        "companiesChanges": saveChanges["companies"], 
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            location.replace("../groups");
        }
    });

    xhr.open("POST", "api/userGroups/" + groupId);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

function loadTopicsAndCompanies() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status === 200) {
            var data = JSON.parse(this.responseText);

            var companiesTable = document.getElementById("companies_table");

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                var checked = data[i].name in companiesInGroup;

                tr.innerHTML = `
                <td>${data[i].name}</td>
                <td><input type="checkbox" name="" id="c${data[i].id}" onInput="onCheckBoxChecked(this)" ${checked ? "checked" : ""}></td>`;
                companiesTable.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "api/companies/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();

    var xhr2 = new XMLHttpRequest();
    xhr2.withCredentials = true;

    xhr2.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status === 200) {
            var data = JSON.parse(this.responseText);

            var companiesTable = document.getElementById("topics_table");

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                var checked = data[i].name in topicsInGroup;

                tr.innerHTML = `
                <td>${data[i].name}</td>
                <td><input type="checkbox" name="" id="t${data[i].id}" onInput="onCheckBoxChecked(this)" ${checked ? "checked" : ""}></td>`;
                companiesTable.appendChild(tr);
            }
        }
    });

    xhr2.open("GET", "api/topics/");
    xhr2.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr2.send();
}

function onCheckBoxChecked(item) {
    console.log(item.id);

    if (item.id.startsWith("c")) {
        saveChanges["companies"][item.id.slice(1)] = item.checked;
    }
    else if (item.id.startsWith("t")) {
        saveChanges["topics"][item.id.slice(1)] = item.checked;
    }

    console.log(saveChanges)
}

loadGroup();
