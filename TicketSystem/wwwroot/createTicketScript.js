var availableUsers = {};
var selectedUser = sessionStorage.getItem("id");

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
    var xhr = new XMLHttpRequest();

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);

            var suggestions = document.getElementById("suggestions");
            suggestions.replaceChildren();
            availableUsers = {};

            for (var i = 0; i < data.length; i++) {
                var option = document.createElement("option");

                option.innerText = data[i].fullName + ` [${data[i].companyShortName}]`;
                availableUsers[data[i].fullName + data[i].companyShortName] = data[i];

                if (data[i].id == sessionStorage.getItem("id")) {
                    document.getElementById("sender_input").value = data[i].fullName + ` [${data[i].companyShortName}]`;
                    document.getElementById("user_data_company").value = data[i].companyName;
                    document.getElementById("user_data_phone").value = data[i].phoneNumber;
                    document.getElementById("user_data_email").value = data[i].email;
                
                    selectedUser = data[i].id;
                }

                suggestions.appendChild(option);
            }
        }
    });

    var authToken = sessionStorage.getItem("access_token");

    xhr.open("POST", "api/Users/getExecutors");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader('Authorization', 'Bearer ' + authToken);

    xhr.send("{}");
}

function onsenderselect(input) {
    try {
        var userName = input.value.split('[')[0].trim();

        var userCompanyShortName = input.value.split('[')[1];
        userCompanyShortName = userCompanyShortName.substring(0, userCompanyShortName.length - 1);
    }
    catch {
        selectedUser = sessionStorage.getItem("id");
        return;
    }

    if (userName + userCompanyShortName in availableUsers) {
        selectedUser = availableUsers[userName + userCompanyShortName].id;
    }
    else {
        selectedUser = sessionStorage.getItem("id");
    }

    document.getElementById("user_data_company").value = availableUsers[userName + userCompanyShortName].companyName;
    document.getElementById("user_data_phone").value = availableUsers[userName + userCompanyShortName].phoneNumber;
    document.getElementById("user_data_email").value = availableUsers[userName + userCompanyShortName].email;
}

loadUsers();