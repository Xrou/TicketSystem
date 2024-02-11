function login() {
    var http = new XMLHttpRequest();

    var login = document.getElementById("auth_login").value;
    var password = document.getElementById("auth_password").value;

    var data = JSON.stringify({ "login": login, "password": password });
    http.open('POST', 'api/users/login', true);

    http.setRequestHeader('Content-type', 'application/json');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            sessionStorage.setItem("access_token", data.access_token);
            sessionStorage.setItem("id", data.id);
            window.open("/");
        }
    }

    http.send(data);
}

function authTelegram(user) {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;

    xhr.addEventListener("readystatechange", function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            sessionStorage.setItem("access_token", data.access_token);
            sessionStorage.setItem("id", data.id);
            window.open("/");
        }
        else if (xhr.readyState == 4 && (xhr.status == 404 || xhr.status == 403)) {
            alert("Нет пользователя с таким telegram аккаунтом. Пожалуйста, зарегистрируйтесь");
        }
    });

    xhr.open("POST", "/api/Users/loginTelegram");
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.send(JSON.stringify(user));
}