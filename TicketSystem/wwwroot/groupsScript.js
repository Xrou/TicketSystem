function loadGroups() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            console.log(this.responseText);

            var data = JSON.parse(this.responseText);
            var table = document.getElementById("groups_table");
            table.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.innerHTML = `
                <tr>
                    <td>${data[i].id}</td>
                    <td>${data[i].name}</td>
                    <td><button id="${data[i].id}" onclick="editGroup(this)">Редактировать</button></td>
                    <td><button id="${data[i].id}" onclick="deleteGroup(this)">Удалить</button></td>
                </tr>
                `;

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "/api/userGroups/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function editGroup(button) {
    location.replace(`editGroup?id=${button.id}`);
}


function deleteGroup(button) {
    console.log(button.id)
}

function createGroup() {
    if (document.getElementById("name_input").value == "")
        return;

    var data = JSON.stringify({
        "name": document.getElementById("name_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            loadGroups();
        }
    });

    xhr.open("POST", "api/userGroups/");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}


loadGroups();
