var availableGroups = {};
var selection = {}

function loadGroups() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);

            var row = document.getElementById("companies_header");

            for (var i = 0; i < data.length; i++) {
                var th = document.createElement("th");

                th.innerHTML = data[i].name;
                th.style.minWidth = 10;
                row.appendChild(th);

                availableGroups[data[i].id] = data[i].name;
            }

            loadUsers();
        }
    });

    xhr.open("GET", "api/usergroups/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function loadUsers() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);
            var table = document.getElementById("users_table");

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                var td_name = document.createElement("td");
                td_name.innerHTML = data[i].fullName;

                var td_company = document.createElement("td");
                td_company.innerHTML = data[i].companyName;

                tr.appendChild(td_name);
                tr.appendChild(td_company);


                var availableKeys = Object.keys(availableGroups);
                var userKeys = [];

                for (var j = 0; j < data[i].userGroups.length; j++) {
                    userKeys.push(data[i].userGroups[j].id.toString());
                }

                for (var availableKeyIndex = 0; availableKeyIndex < availableKeys.length; availableKeyIndex++) {
                    var checked = false;

                    if (userKeys.includes(availableKeys[availableKeyIndex])) {
                        checked = true;
                    }

                    var td = document.createElement("td");
                    td.innerHTML = `
                    <input type="checkbox" id="${data[i].id}g${availableKeys[availableKeyIndex]}" oninput="saveSelection(this)" ${checked ? "checked" : ""}>
                    `;

                    tr.appendChild(td)
                }

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "api/users/GetUsersGroups");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function saveSelection(box) {
    var userId = Number(box.id.split("g")[0]);
    var groupId = Number(box.id.split("g")[1]);

    if (!(userId in selection)) {
        selection[userId] = {};
    }

    selection[userId][groupId] = box.checked;

    console.log(selection);
}

function saveChanges() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            console.log(this.responseText);
        }
    });

    xhr.open("POST", "api/usergroups/UpdateUserGroups");
    
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));
    
    xhr.send(JSON.stringify(selection));
}

loadGroups();
