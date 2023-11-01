newRights = {}

function getGroups() {
    table_columns.replaceChildren();
    groups_table.replaceChildren();

    newRights = {};

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var json = JSON.parse(this.responseText);

            var table_columns = document.getElementById("table_columns");
            var groups_table = document.getElementById("groups_table");

            for (var i = 0; i < json.length; i++) {
                var keys = Object.keys(json[0]);

                if (i == 0) {
                    for (var j = 0; j < keys.length; j++) {
                        var th = document.createElement("th");
                        th.innerText = keys[j];
                        table_columns.appendChild(th);
                    }
                }

                var tr = document.createElement("tr");

                for (var k = 0; k < keys.length; k++) {
                    var td = document.createElement("td");

                    if (typeof (json[i][keys[k]]) == 'boolean') {
                        td.innerHTML = `
                        <input type="checkbox" id="${json[i].id}|${keys[k]}" oninput="saveSelection(this)" ${json[i][keys[k]] ? "checked" : ""}>
                        `;
                    }
                    else {
                        td.innerText = json[i][keys[k]];
                    }

                    tr.appendChild(td);
                }

                groups_table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "/api/AccessGroups/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function saveSelection(checkbox) {
    var groupId = checkbox.id.split("|")[0];
    var parameterName = checkbox.id.split("|")[1];

    if (newRights[groupId] == undefined)
        newRights[groupId] = {}

    newRights[groupId][parameterName] = checkbox.checked;

    console.log(newRights)
}

function saveChanges() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            console.log(this.responseText);
        }
    });

    xhr.open("POST", "/api/AccessGroups/UpdateAccessGroups");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(JSON.stringify(newRights));
}

getGroups();