var selectedUser = undefined;
var availableUsers = {};

function createTicket() {
    urgency = 0;

    if (document.getElementById("medium").checked)
        urgency = 1;
    else if (document.getElementById("high").checked)
        urgency = 2;

    var http = new XMLHttpRequest();
    http.open('POST', 'api/tickets', true);

    var authToken = sessionStorage.getItem("access_token");

    var ticketText = document.getElementById("text_input").value;

    var ticketData = JSON.stringify({
        userId: selectedUser, 
        text: ticketText,
        urgency: urgency
    });

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 201) {
            document.location.replace(`ticket/?id=${http.responseText}`);
        }
    }

    http.send(ticketData);
}

function loadUsers() {
    var data = JSON.stringify({
        "name": document.getElementById("name_filter").value,
        "company": document.getElementById("company_filter").value,
        "id": document.getElementById("id_filter").value
    });

    var xhr = new XMLHttpRequest();

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);

            var table = document.getElementById("users_table");
            table.replaceChildren();

            availableUsers = []

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.id = data[i].id;

                tr.innerHTML = `
                <td>${data[i].fullName}</td>
                <td>${data[i].companyName}</td>
                `;

                availableUsers[data[i].id] = { "name": data[i].fullName, "company": data[i].companyName };

                tr.onclick = function () {
                    var s = document.getElementById("modal_selected_user");
                    s.innerText = availableUsers[this.id].name + " " + availableUsers[this.id].company;
                    var b = document.getElementById("select_user_button");
                    b.innerText = availableUsers[this.id].name;

                    selectedUser = this.id;
                }

                table.appendChild(tr);
            }
        }
    });

    var authToken = sessionStorage.getItem("access_token");

    xhr.open("POST", "api/Users/getFilteredUsers");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader('Authorization', 'Bearer ' + authToken);

    xhr.send(data);
}

loadUsers();