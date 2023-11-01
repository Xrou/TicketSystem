var deadline_date;
var availableUsers = {};
var selectedUser = 0;
var userRights;

var availableCompanies = {};
var selectedCompany = 0;
var selectedEditCompany = 0;

var availableStatuses = {};
var selectedStatus = {};

var ticketUser;

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

    var ticketType;

    var http = new XMLHttpRequest();
    http.open('GET', `../api/tickets/${ticket_id}`, true);

    var authToken = sessionStorage.getItem("access_token")

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            console.log(data);

            var executorName = "не назначен";

            if (data.executorUserName != "")
                executorName = data.executorUserName;

            var urgency = "низкая";

            if (data.urgency == 1)
                urgency = "средняя";
            else if (data.urgency == 2)
                urgency = "высокая";

            document.getElementById("ticket_date").innerHTML = data.date;
            document.getElementById("sender_name").innerHTML = data.userName;
            document.getElementById("phone_number").innerHTML = data.senderPhone;
            document.getElementById("sender_company").innerHTML = data.senderCompany;
            document.getElementById("ticket_text").innerHTML = data.text;
            document.getElementById("executor_name").innerHTML = executorName;
            document.getElementById("ticket_urgency").innerHTML = urgency;
            document.getElementById("ticket_topic").innerHTML = data.topicName;
            document.getElementById("status").innerHTML = data.status;
            console.log(userRights);
            if (userRights["CanTakeTickets"] == false)
            {
                document.getElementById("take_ticket_button").style.display = "none";
            }
            if (userRights["CanAssignTickets"] == false)
            {
                document.getElementById("assign_ticket_button").style.display = "none";
            }
            if (userRights["CanSubscribe"] == false)
            {
                document.getElementById("subscribe_ticket_button").style.display = "none";
            }
            if (userRights["CanFinishTickets"] == false)
            {
                document.getElementById("finish_ticket_button").style.display = "none";
            }
            if (userRights["CanMoveTickets"] == false)
            {
                document.getElementById("move_ticket_button").style.display = "none";
            }

            file_links = document.getElementById("file_links");

            for (var i = 0; i < data.files.length; i++) {
                a = document.createElement("a");
                a.target = "_blank";
                a.href = "../" + data.files[i];
                a.innerText = data.files[i] + "\n";
                a.download = "../" + data.files[i];
                file_links.appendChild(a);
            }


            ticketUser = data.userId;

            var day = data.deadlineTime.split('.')[0];
            var month = data.deadlineTime.split('.')[1];
            var year = data.deadlineTime.split(' ')[0].split('.')[2];
            var hour = data.deadlineTime.split(' ')[1].split(':')[0];
            var minute = data.deadlineTime.split(' ')[1].split(':')[1];
            var second = data.deadlineTime.split(' ')[1].split(':')[2];

            deadline_date = new Date(year, month - 1, day, hour, minute, second);
            var canRegisterValue;
            var http2 = new XMLHttpRequest();
            http2.open('GET', `../api/users/canRegisterUsers`, false);

            var authToken = sessionStorage.getItem("access_token")

            http2.setRequestHeader('Authorization', 'Bearer ' + authToken);

            http2.onreadystatechange = function () {
                if (http2.readyState == 4 && http2.status == 200) {
                    canRegisterValue = http2.responseText == "true";

                    if (data.type == 2 && canRegisterValue) {
                        document.getElementById("registration").style.display = "block";
                        loadUserDataToConfirm();
                    }
                }
            }

            http2.send();
            checkCanEditUserInfo();
        }
    }

    http.send();
}

function loadAvailableStatuses() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            availableStatuses = {};
            var datalist = document.getElementById("status_suggestions");
            datalist.replaceChildren();

            for (var i = 0; i < data.length; i++) {
                var option = document.createElement("option");
                option.innerHTML = data[i].Name;
                datalist.appendChild(option);

                availableStatuses[data[i].Name] = data[i];
            }
        }
    });

    xhr.open("GET", "/api/statuses");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function onStatusSelect(input) {
    statusName = input.value;

    if (statusName in availableStatuses) {
        selectedStatus = availableStatuses[statusName].Id;
    }
    else {
        selectedStatus = 0;
    }
    console.log(selectedStatus);
    if (selectedStatus == 0)
        return;

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var data = JSON.stringify({
        "ticketId": ticket_id,
        "statusId": selectedStatus.toString()
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.open("POST", "/api/tickets/setStatus");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

function checkCanEditUserInfo() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            if (data["CanEditUsers"] == true) {
                var currentCompany = document.getElementById("sender_company").textContent;
                var currentStatus = document.getElementById("status").textContent;

                document.getElementById("sender_name").contentEditable = true;
                document.getElementById("phone_number").contentEditable = true;

                document.getElementById("sender_company").innerHTML = `
                <input type="text" autocomplete="on" list="company_suggestions" oninput="onCompanySelectEditUser(this)" value="${currentCompany}">`;
                document.getElementById("status").innerHTML = `
                <input type="text" autocomplete="on" list="status_suggestions" oninput="onStatusSelect(this)" value="${currentStatus}">`;

                document.getElementById("ticket_info_save_edits").style.display = "block";

                selectedEditCompany = availableCompanies[currentCompany].id;
            }
        }
    });

    xhr.open("GET", "/api/Users/getRights");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function onCompanySelectEditUser(input) {
    companyName = input.value;

    if (companyName in availableCompanies) {
        selectedEditCompany = availableCompanies[companyName].id;
    }
    else {
        selectedEditCompany = 0;
    }
}

function editUserInfo() {
    var data = JSON.stringify({
        "fullName": document.getElementById("sender_name").innerText,
        "phoneNumber": document.getElementById("phone_number").innerText,
        "companyId": selectedEditCompany,
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            location.reload();
        }
    });

    xhr.open("POST", "/api/Users/" + ticketUser);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}

function getComments() {
    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var url = new URL(window.location.origin + "/api/comments");
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

    if (!userRights["CanSeeServiceComments"]) {
        document.getElementById("service_comments").remove();
        document.getElementById("service_comments_radio").remove();
        document.getElementById("service_comments_label").remove();

        return;
    }

    console.log("can see");
        
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

function getPossibleExecutors() {
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

                suggestions.appendChild(option);
            }
        }
    });

    var authToken = sessionStorage.getItem("access_token");

    xhr.open("POST", "../api/Users/getExecutors");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader('Authorization', 'Bearer ' + authToken);

    xhr.send("{}");
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
    http.open('POST', '../api/comments', true);

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
    http.open('POST', '../api/tickets/assignTicket', true);

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
    http.open('GET', `../api/tickets/subscribe/${ticket_id}`, true);

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            getTicketData();
        }
    }

    http.send();
}

function showAssignButtons() {
    document.getElementById("assign_button").style.display = "none";
    document.getElementById("sender_input").style.display = "block";
    document.getElementById("assign_button_commit").style.display = "block";
    document.getElementById("assign_button_cancel").style.display = "block";

}

function hideAssignButtons() {
    document.getElementById("assign_button").style.display = "block";
    document.getElementById("sender_input").style.display = "none";
    document.getElementById("assign_button_commit").style.display = "none";
    document.getElementById("assign_button_cancel").style.display = "none";
}

function onsenderselect(input) {
    try {
        var userName = input.value.split('[')[0].trim();

        var userCompanyShortName = input.value.split('[')[1];
        userCompanyShortName = userCompanyShortName.substring(0, userCompanyShortName.length - 1);
    }
    catch {
        selectedUser = 0;
        return;
    }

    if (userName + userCompanyShortName in availableUsers) {
        selectedUser = availableUsers[userName + userCompanyShortName].id;
    }
    else {
        selectedUser = 0;
    }
}

function assignTicket() {
    showAssignButtons();
}

function assignTicketCommit() {
    hideAssignButtons();

    if (selectedUser == 0)
        return;

    var http = new XMLHttpRequest();
    http.open('POST', '../api/tickets/assignTicket', true);

    var authToken = sessionStorage.getItem("access_token");
    var myId = sessionStorage.getItem("id");

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var ticketData = JSON.stringify({
        userId: String(selectedUser),
        ticketId: String(ticket_id)
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

function assignTicketCancel() {
    hideAssignButtons();
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
    http.open('POST', '../api/tickets/setDeadline', true);

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
    http2.open('POST', '../api/comments', true);

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

function verifyRegistration() {
    var http = new XMLHttpRequest();
    http.open('POST', '../api/users/confirmRegistration', true);

    var authToken = sessionStorage.getItem("access_token");

    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = urlParams.get('id');

    var fullName = document.getElementById("verify_fullName").value;
    var phone = document.getElementById("verify_phone").value;
    var email = document.getElementById("verify_email").value;

    var ticketData = JSON.stringify({
        ticketId: ticket_id,
        fullName: fullName,
        phone: phone,
        email: email,
        companyId: selectedCompany
    });

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            location.replace("#close");
            location.reload();
        }
    }

    http.send(ticketData);
}

function loadCompaniesVerify() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);

            var suggestions = document.getElementById("company_suggestions");
            suggestions.replaceChildren();
            for (var i = 0; i < data.length; i++) {
                var option = document.createElement("option");
                option.innerHTML = `${data[i].name}`;
                suggestions.appendChild(option);
                availableCompanies[data[i].name] = data[i];
            }
        }
    });

    xhr.open("GET", "../api/companies/");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function onCompanySelect(input) {
    companyName = input.value;

    if (companyName in availableCompanies) {
        selectedCompany = availableCompanies[companyName].id;
    }
    else {
        selectedCompany = 0;
    }
}

function loadUserDataToConfirm() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);

            document.getElementById("verify_fullName").value = data.fullName;
            document.getElementById("verify_phone").value = data.phoneNumber;
            document.getElementById("verify_email").value = data.email;
        }
    });

    xhr.open("GET", "../api/users/" + ticketUser);
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

function closeTicket() {
    var urlParams = new URLSearchParams(window.location.search);
    var ticket_id = Number(urlParams.get('id'));

    var finishStatus = 0;
    if (document.getElementById("changes").checked)
        finishStatus = 1;

    var finishComment = document.getElementById("finish_comment").value;

    var data = JSON.stringify({
        "ticketId": ticket_id,
        "finishStatus": finishStatus,
        "commentText": finishComment
    });

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            getTicketData();
            getComments();
            location.replace("#close");
        }
    });

    xhr.open("POST", "../api/Tickets/closeTicket");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send(data);
}


function CheckRights() {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            var data = JSON.parse(this.responseText);
            userRights = data;
        }
    });

    xhr.open("GET", "/api/Users/getRights");
    xhr.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("access_token"));

    xhr.send();
}

CheckRights();

setInterval(updateTimer, 1000);

getTicketData();
loadCompaniesVerify();
loadAvailableStatuses();
getComments();
getPossibleExecutors();


var now = new Date();

var day = ("0" + (now.getDate())).slice(-2);
var month = ("0" + (now.getMonth() + 1)).slice(-2);

var hour = ("0" + (now.getHours())).slice(-2);
var minute = ("0" + (now.getMinutes())).slice(-2);

var today = now.getFullYear() + "-" + (month) + "-" + (day) + " " + (hour) + ":" + (minute);

$('#action_time_selector_calendar').val(today);