var availableCompanies = {};
var selectedCompany = 0;

var companyId;

function loadUser() {
    var urlParams = new URLSearchParams(window.location.search);
    companyId = urlParams.get('id');

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);
            console.log(data);

            document.getElementById("id_input").value = data.id;
            document.getElementById("fullname_input").value = data.fullName;
            document.getElementById("login_input").value = data.name;
            document.getElementById("company_input").value = data.companyName;
            document.getElementById("phone_input").value = data.phoneNumber;
            document.getElementById("email_input").value = data.email;
            document.getElementById("telegram_input").value = Number(data.telegram);
        }
    });

    xhr.open("GET", "../api/Users/" + companyId);
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function loadCompanies() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            var suggestions = document.getElementById("suggestions");
            suggestions.replaceChildren();
            availableCompanies = {};

            for (var i = 0; i < data.length; i++) {
                var option = document.createElement("option");

                option.innerHTML = data[i].name;
                availableCompanies[data[i].name] = data[i].id;

                suggestions.appendChild(option);
            }
        }
    });

    xhr.open("GET", "../api/Companies");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}


function onCompanySelect(input) {
    var companyName = input.value;

    if (companyName in availableCompanies) {
        selectedCompany = availableCompanies[companyName];
    }
    else {
        selectedCompany = 0;
    }
    console.log(selectedCompany);
}

function saveUser() {
    var data = {
        "name": document.getElementById("login_input").value,
        "fullName": document.getElementById("fullname_input").value,
        "phoneNumber": document.getElementById("phone_input").value,
        "email": document.getElementById("email_input").value,
        "telegram": Number(document.getElementById("telegram_input").value)
    };

    if (selectedCompany != 0)
        data["companyId"] = selectedCompany;

    data = JSON.stringify(data);

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            location.replace("../usersDirectory");
        }
    });

    xhr.open("POST", "http://localhost/api/Users/" + companyId);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

loadUser();
loadCompanies();