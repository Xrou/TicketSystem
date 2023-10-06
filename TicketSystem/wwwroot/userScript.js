function getInfo() {
    var http = new XMLHttpRequest();
    http.open('GET', `api/users/me`, true);

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            console.log(data);

            document.title = data["fullName"];

            document.getElementById("fullName").value = data["fullName"];
            document.getElementById("company").value = data["companyName"];
            document.getElementById("phoneNumber").value = data["phoneNumber"];
            document.getElementById("email").value = data["email"];
            document.getElementById("telegram").value = data["telegram"];
        }
    }

    http.send();
}

function getAccess() {
    var http = new XMLHttpRequest();
    http.open('GET', `api/users/hasAdminRights`, true);

    http.setRequestHeader('Authorization', 'Bearer ' + authToken);
    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var list = document.getElementById("pages_list");
            var li = document.createElement("li");
            li.className = "pages_list_item";
            li.innerHTML = `
                <li class="pages_list_item"><a class="settingsPageLink" href="userAccessGroups">Права пользователей</a></li>
                <li class="pages_list_item"><a class="settingsPageLink" href="userGroups">Группы пользователей</a></li>
                <li class="pages_list_item"><a class="settingsPageLink" href="groups">Группы</a></li>
                <li class="pages_list_item"><a class="settingsPageLink" href="companies">Компании</a></li>
                <li class="pages_list_item"><a class="settingsPageLink" href="usersDirectory">Пользователи</a></li>
                <li class="pages_list_item"><a class="settingsPageLink" href="thematics">Тематики</a></li>
            `;

            list.appendChild(li);
        }
    }

    http.send();
}

getAccess();

getInfo();