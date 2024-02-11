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

            console.log(json);

            var table_columns = document.getElementById("table_columns");
            var groups_table = document.getElementById("groups_table");

            var th = document.createElement("th");
            table_columns.append(th);

            for (var k = 0; k < json.length; k++) {
                var th = document.createElement("th");
                th.innerText = json[k].name;
                table_columns.append(th);
            }

            var fields = Object.keys(json[0]);

            for (var i = 2; i < fields.length; i++) {
                var tr = document.createElement("tr");

                var td = document.createElement("td");
                td.innerText = fields[i];

                tr.append(td);

                for (var k = 0; k < json.length; k++) {
                    var vals = Object.values(json[k]);
                    var td = document.createElement("td");
                    td.innerHTML = `<input type="checkbox" id="${json[k].id}|${fields[i]}" oninput="saveSelection(this)" ${json[k][fields[i]] ? "checked" : ""}>`;
                    console.log(`${k} ${fields[i]}`);
                    tr.append(td);
                }

                groups_table.append(tr);
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