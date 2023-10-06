function loadThematics() {
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
                    <td><button id="${data[i].id}" onclick="editThematic(this)">Редактировать</button></td>
                    <td><button id="${data[i].id}" onclick="deleteThematic(this)">Удалить</button></td>
                </tr>
                `;

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "/api/topics/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function editThematic(button) {

}


function deleteThematic(button) {
    console.log(button.id)
}

function createThematic() {
    if (document.getElementById("name_input").value == "")
        return;

    var data = JSON.stringify({
        "name": document.getElementById("name_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            loadThematics();
        }
    });

    xhr.open("POST", "api/topics/");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}


loadThematics();
