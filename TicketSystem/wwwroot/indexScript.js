function open_ticket(click_row) {
    window.open(`ticket/?id=${click_row}`, "_blank");
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

            console.log(data);

            for (var i = 0; i < data.length; i++) {
                var tr = document.createElement("tr");

                tr.id = data[i].Id;

                tr.innerHTML = `
                <td>${data[i].id}</td>
                <td>${data[i].userName}</td>
                <td>${data[i].senderCompany}</td>
                <td>${data[i].topicName}</td>
                <td>${data[i].status}</td>
                <td>${data[i].executorUserName}</td>
                <td>${data[i].text}</td>
                <td>${data[i].date}</td>
                <td>${data[i].deadlineTime}</td>
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
    window.open("./auth");
}

getTickets();