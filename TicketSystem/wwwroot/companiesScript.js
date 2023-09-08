function getCompanies() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            var table = document.getElementById("groups_table");
            table.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.innerHTML = `
                    <td>${data[i].id}</td>
                    <td>${data[i].name}</td>
                    <td>${data[i].shortName}</td>`;

                tr.id = data[i].id

                tr.onclick = function () {
                    console.log("clck");
                    location.replace("editCompany/?id=" + this.id);
                };

                table.appendChild(tr);
            }
        }
    });

    xhr.open("GET", "/api/companies");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function create_company() {
    var data = JSON.stringify({
        "name": document.getElementById("name_input").value,
        "shortName": document.getElementById("short_name_input").value
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            getCompanies();
        }
    });

    xhr.open("POST", "/api/companies");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

getCompanies();
