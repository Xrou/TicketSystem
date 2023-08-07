function createTicket() {
    var http = new XMLHttpRequest();
    http.open('POST', 'https://localhost:7177/api/tickets', true);

    var authToken = sessionStorage.getItem("access_token");

    var ticketText = document.getElementById("text_input").value;

    var ticketData = JSON.stringify({
        text: ticketText
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