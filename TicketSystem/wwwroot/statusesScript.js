function loadStatuses() {
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
                    <td>${data[i].Id}</td>
                    <td>${data[i].Name}</td>
                    <td><button id="${data[i].Id}" onclick="editStatus(this)">Редактировать</button></td>
                    <td><button id="${data[i].Id}" onclick="deleteStatus(this)">Удалить</button></td>
                </tr>
                `;

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "/api/statuses/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function createStatus() {
    var name = document.getElementById("name_input").value;

    if (name == "")
        return;

    var data = JSON.stringify({
        "name": document.getElementById("name_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            loadStatuses();
        }
    });

    xhr.open("POST", "/api/statuses");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

loadStatuses();
