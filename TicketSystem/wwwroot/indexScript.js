function open_ticket(click_row) {
    window.location.replace(`ticket/?id=${click_row}`);
}

function getTickets() {
    var http = new XMLHttpRequest();
    http.open('GET', 'api/tickets', true);

    var authToken = sessionStorage.getItem("access_token")

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);

            var table = document.getElementById("tickets_table");
            table.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.id = data[i].Id;

                tr.innerHTML = `
                <td>${data[i].Id}</td>
                <td>${data[i].UserName}</td>
                <td>${data[i].Text}</td>
                <td>${data[i].Date}</td>
                <td>${data[i].DeadlineTime}</td>
                <td>${data[i].ExecutorUserName}</td>
                `;
                tr.onclick = function () {
                    open_ticket(this.id);
                }

                table.appendChild(tr);
            }
        }
    }

    http.send();
}

function logout() {
    sessionStorage.removeItem("access_token");
    sessionStorage.removeItem("id");
    document.location.replace("./auth");
}

getTickets();