var companyId;


function loadCompany() {
    var urlParams = new URLSearchParams(window.location.search);
    companyId = urlParams.get('id');

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            document.getElementById("id_input").value = data.id;
            document.getElementById("fullname_input").value = data.name;
            document.getElementById("shortname_input").value = data.shortName;
        }
    });

    xhr.open("GET", "../api/companies/" + companyId);
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function saveCompany() {
    var data = JSON.stringify({
        "name": document.getElementById("fullname_input").value,
        "shortName": document.getElementById("shortname_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            location.replace("../companies");
        }
    });

    xhr.open("POST", "../api/companies/" + companyId);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU3VwZXJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMjAiLCJleHAiOjE2OTQxMTgzMTUsImlzcyI6IklUTFNlcnZlciIsImF1ZCI6IklUTFVzZXIifQ.AenMeyE3OjJu0gLetZbt5wdT1ct4L7s4U3bbbDrtVfewoHCc50bLcRPKg02i4xGlAw6efdeVM7wF_kVl5ThXSQ");

    xhr.send(data);
}


loadCompany();