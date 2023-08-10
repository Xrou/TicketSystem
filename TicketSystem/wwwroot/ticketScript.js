var deadline_date;

function showTimeSelector() {
    document.getElementById("action_time_selector").style.display = "block";
}

function hideTimeSelector() {
    document.getElementById("action_time_selector").style.display = "none";
}

function getTicketData() {
    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    document.title = `Заявка ${ticket_id}`

    var http = new XMLHttpRequest();
    http.open('GET', `https://localhost:7177/api/tickets/${ticket_id}`, true);

    var authToken = sessionStorage.getItem("access_token")

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            console.log(data);

            var executorName = "не назначен";

            if(data.executorUserName != "")
                executorName = data.executorUserName;

            document.getElementById("ticket_date").innerHTML = data.date;
            document.getElementById("sender_name").innerHTML = data.userName;
            document.getElementById("sender_company").innerHTML = data.senderCompany;
            document.getElementById("ticket_text").innerHTML = data.text;
            document.getElementById("executor_name").innerHTML = executorName;

            var day = data.deadlineTime.split('.')[0];
            var month = data.deadlineTime.split('.')[1];
            var year = data.deadlineTime.split(' ')[0].split('.')[2];
            var hour = data.deadlineTime.split(' ')[1].split(':')[0];
            var minute = data.deadlineTime.split(' ')[1].split(':')[1];
            var second = data.deadlineTime.split(' ')[1].split(':')[2];

            deadline_date = new Date(year, month - 1, day, hour, minute, second);
        }
    }
    http.send();
}

function getComments() {
    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var url = new URL("https://localhost:7177/api/comments");
    url.searchParams.append('ticketId', ticket_id);
    url.searchParams.append('commentType', "1");

    var http = new XMLHttpRequest();
    http.open('GET', url.toString(), true);

    var authToken = sessionStorage.getItem("access_token")

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            var standardComments = document.getElementById("standard_comments");

            standardComments.replaceChildren();
            for (var i = 0; i < data.length; i++) {
                var li = document.createElement("li");

                li.innerHTML = `
                <span>${data[i].UserName}</span>
                <span>${data[i].Date}</span>
                <p>${data[i].Text}</p>
                `;
                li.className = "comment";
                standardComments.appendChild(li);
            }
        }
    }

    http.send();

    url.searchParams.set('commentType', '2');

    var http2 = new XMLHttpRequest();
    http2.open('GET', url.toString(), true);

    var authToken = sessionStorage.getItem("access_token")

    http2.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http2.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http2.onreadystatechange = function () {
        if (http2.readyState == 4 && http2.status == 200) {
            var data = JSON.parse(http2.responseText);
            var officialComments = document.getElementById("official_comments");

            officialComments.replaceChildren();
            for (var i = 0; i < data.length; i++) {
                var li = document.createElement("li");

                li.innerHTML = `
                <span>${data[i].UserName}</span>
                <span>${data[i].Date}</span>
                <p>${data[i].Text}</p>
                `;
                li.className = "comment";
                officialComments.appendChild(li);
            }
        }
    }

    http2.send();

    url.searchParams.set('commentType', '3');

    var http3 = new XMLHttpRequest();
    http3.open('GET', url.toString(), true);

    var authToken = sessionStorage.getItem("access_token")

    http3.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http3.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http3.onreadystatechange = function () {
        if (http3.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http3.responseText);
            var serviceComments = document.getElementById("service_comments");
            serviceComments.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var li = document.createElement("li");

                li.innerHTML = `
                <span>${data[i].UserName}</span>
                <span>${data[i].Date}</span>
                <p>${data[i].Text}</p>
                `;
                li.className = "comment";
                serviceComments.appendChild(li);
            }
        }
    }

    http3.send();
}

function sendComment() {
    commentText = document.getElementById("write_comment_text").value;
    document.getElementById("write_comment_text").value = "";
    if (commentText == "")
        return;

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');
    var commentType = 3;
    if (document.getElementById("tab-btn-1").checked)
        commentType = 1;
    else if (document.getElementById("tab-btn-2").checked)
        commentType = 2;

    var http = new XMLHttpRequest();
    http.open('POST', 'https://localhost:7177/api/comments', true);

    var authToken = sessionStorage.getItem("access_token");

    var ticketData = JSON.stringify({
        TicketId: ticket_id,
        Text: commentText,
        Type: commentType
    });

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 201) {
            getComments();
        }
    }

    http.send(ticketData);

}

function takeTicket() {
    var http = new XMLHttpRequest();
    http.open('POST', 'https://localhost:7177/api/tickets/assignTicket', true);

    var authToken = sessionStorage.getItem("access_token");
    var myId = sessionStorage.getItem("id");

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var ticketData = JSON.stringify({
        userId: myId,
        ticketId: ticket_id
    });

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            getTicketData();
        }
    }

    http.send(ticketData);
}

function subscribeTicket() {
    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var http = new XMLHttpRequest();
    http.open('GET', `https://localhost:7177/api/tickets/subscribe/${ticket_id}`, true);

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            getTicketData();
        }
    }

    http.send();
}

function assignTicket() {
    alert("после переезда на фио сделать");
}

function updateTimer() {
    var timer = document.getElementById("timer");

    if (deadline_date < new Date()) {
        timer.textContent = `00:00:00`;
        return;
    }

    var remainingTime = deadline_date - new Date();

    var seconds = Math.floor(remainingTime / 1000);
    var minutes = Math.floor(seconds / 60);
    var hours = Math.floor(minutes / 60);
    var days = Math.floor(hours / 24);

    seconds -= minutes * 60;
    minutes -= hours * 60;
    hours -= days * 24;

    if (days > 0)
        timer.textContent = `${days.toString().padStart('2', '0')} ${hours.toString().padStart('2', '0')}:${minutes.toString().padStart('2', '0')}:${seconds.toString().padStart('2', '0')}`;

    else
        timer.textContent = `${hours.toString().padStart('2', '0')}:${minutes.toString().padStart('2', '0')}:${seconds.toString().padStart('2', '0')}`;
}

function setDeadlineDate(days) {
    var now = new Date();
    now.setDate(now.getDate() + days);

    var day = ("0" + (now.getDate())).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);

    var hour = ("0" + (now.getHours())).slice(-2);
    var minute = ("0" + (now.getMinutes())).slice(-2);

    var today = now.getFullYear() + "-" + (month) + "-" + (day) + " " + (hour) + ":" + (minute);

    $('#action_time_selector_calendar').val(today);
}

function getDateAsString(date) {
    var currentDate = date.toLocaleDateString().substring(6, 10) + "-" + date.toLocaleDateString().substring(3, 5) + "-" + date.toLocaleDateString().substring(0, 2);
    return currentDate;
}

function setDeadline() {
    var deadlineTime = document.getElementById('action_time_selector_calendar').value;
    var http = new XMLHttpRequest();
    http.open('POST', 'https://localhost:7177/api/tickets/setDeadline', true);

    var authToken = sessionStorage.getItem("access_token");

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var ticketData = JSON.stringify({
        ticketId: ticket_id,
        deadlineTime: deadlineTime
    });

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 204) {
            getTicketData();
        }
    }

    http.send(ticketData);

    var http2 = new XMLHttpRequest();
    http2.open('POST', 'https://localhost:7177/api/comments', true);

    var commentText = document.getElementById('action_time_selector_text').value;
    deadlineTime = new Date(deadlineTime);
    commentText = `Установлен срок исполнения ${deadlineTime.getHours().toString().padStart(2, "0")}:${deadlineTime.getMinutes().toString().padStart(2, "0")} ${deadlineTime.getDate().toString().padStart(2, "0")}.${(deadlineTime.getMonth() + 1).toString().padStart(2, "0")}.${deadlineTime.getFullYear()} Причина: ${commentText}`;

    var commentData = JSON.stringify({
        ticketId: ticket_id,
        text: commentText,
        type: 1
    });

    http2.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http2.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http2.onreadystatechange = function () {
        if (http2.readyState == 4 && http2.status == 201) {
            getComments();
        }
    }

    http2.send(commentData);
}

setInterval(updateTimer, 1000);

getTicketData();
getComments();

var now = new Date();

var day = ("0" + (now.getDate())).slice(-2);
var month = ("0" + (now.getMonth() + 1)).slice(-2);

var hour = ("0" + (now.getHours())).slice(-2);
var minute = ("0" + (now.getMinutes())).slice(-2);

var today = now.getFullYear() + "-" + (month) + "-" + (day) + " " + (hour) + ":" + (minute);

$('#action_time_selector_calendar').val(today);