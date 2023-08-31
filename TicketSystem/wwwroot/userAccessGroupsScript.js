var tableChanges = {};

function getUsers() {
    tableChanges = {};

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            var table = document.getElementById("users_table");

            table.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.innerHTML = `
                    <td>${data[i].fullName}</td>
                    <td><input type="radio" onchange="rightsChanged(this)" name="${data[i].id}" id="${i}u" ${data[i].accessGroupId == 1 ? "checked" : ""}></td>
                    <td><input type="radio" onchange="rightsChanged(this)" name="${data[i].id}" id="${i}sv" ${data[i].accessGroupId == 2 ? "checked" : ""}></td>
                    <td><input type="radio" onchange="rightsChanged(this)" name="${data[i].id}" id="${i}l1" ${data[i].accessGroupId == 3 ? "checked" : ""}></td>
                    <td><input type="radio" onchange="rightsChanged(this)" name="${data[i].id}" id="${i}l2" ${data[i].accessGroupId == 4 ? "checked" : ""}></td>
                    <td><input type="radio" onchange="rightsChanged(this)" name="${data[i].id}" id="${i}sa" ${data[i].accessGroupId == 5 ? "checked" : ""}></td>
                `;

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "api/settings/GetUsersAccessGroup");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function rightsChanged(button) {
    var id = button.name;
    var accessGroupId = 1;

    if (button.id.endsWith("sv"))
        accessGroupId = 2;
    else if (button.id.endsWith("l1"))
        accessGroupId = 3;
    else if (button.id.endsWith("l2"))
        accessGroupId = 4
    else if (button.id.endsWith("sa"))
        accessGroupId = 5

    tableChanges[id] = accessGroupId;
    console.log(tableChanges);
}

function saveChanges() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            getUsers();
        }
    });

    xhr.open("POST", "api/settings/UpdateUsersAccessGroups");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));
    xhr.send(JSON.stringify(tableChanges));
}

getUsers();