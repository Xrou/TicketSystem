var availableUsers = {};
var selectedUser = sessionStorage.getItem("id");

var availableTopics = {};
var selectedTopic;

var canSelectTopics = false;

function createTicket() {
    function postTicket() {
        urgency = 0;

        if (document.getElementById("medium").checked)
            urgency = 1;
        else if (document.getElementById("high").checked)
            urgency = 2;

        var http = new XMLHttpRequest();
        http.open('POST', 'api/tickets', true);

        var ticketText = document.getElementById("text_input").value;

        if (canSelectTopics == false)
            selectedTopic = 1;

        var ticketData = JSON.stringify({
            userId: selectedUser,
            text: ticketText,
            urgency: urgency,
            topicId: selectedTopic,
            files: files,
        });

        http.setRequestHeader('Authorization', 'Bearer ' + sessionStorage.getItem("access_token"));
        http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

        http.onreadystatechange = function () {
            if (http.readyState == 4 && http.status == 201) {
                document.location.replace(`ticket/?id=${http.responseText}`);
            }
        }

        http.send(ticketData);
    }

    var file_selector = document.getElementById("file_selector");

    var files = []

    for (var i = 0; i < file_selector.files.length; i++) {
        var fileRequset = new XMLHttpRequest();
        fileRequset.open("POST", "./api/Files", false);

        fileRequset.setRequestHeader('Authorization', 'Bearer ' + sessionStorage.getItem("access_token"));

        var formData = new FormData(); formData.append("uploadedFiles", file_selector.files[i]);
        
        fileRequset.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 201) {
                var createdFile = JSON.parse(this.responseText)[0];
                files.push(createdFile);
                console.log(files);
            }
        }

        fileRequset.send(formData);
    }

    postTicket();
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

function loadTopics() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            var suggestions = document.getElementById("topic_suggestions");
            suggestions.replaceChildren();
            availableTopics = {};

            for (var i = 0; i < data.length; i++) {
                var option = document.createElement("option");

                option.innerText = data[i].name;
                availableTopics[data[i].name] = data[i];

                suggestions.appendChild(option);
            }

        }
    });

    xhr.open("GET", "/api/Topics/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function ontopicselect(input) {
    var topicName = input.value;

    if (topicName in availableTopics) {
        selectedTopic = availableTopics[topicName].id;
    }
    else {
        selectedTopic = availableTopics[0];
    }

    console.log(selectedTopic);
}

function CheckRights() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            if (data["CanSelectTopic"] == true) {
                document.getElementById("topic_select_block").innerHTML = `
                <span class="input_legend">Тематика</span>
                <datalist id="topic_suggestions">
                </datalist>
                <input type="text" id="topic_input" autocomplete="on" list="topic_suggestions"
                    oninput="ontopicselect(this)">
                `;
                CheckRights = true;
                loadTopics();
            }

            if (data["CanEditTickets"] == true) {
                document.getElementById("sender_select_block").innerHTML = `
                <span class="input_legend user_data_item_header">Заявитель:</span>
                <datalist id="suggestions">
                </datalist>
                <input type="text" id="sender_input" autocomplete="on" list="suggestions"
                    oninput="onsenderselect(this)">`;
                loadUsers();
            }
        }
    });

    xhr.open("GET", "/api/Users/getRights");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

CheckRights();