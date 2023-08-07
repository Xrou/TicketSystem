function login()
{
    var http = new XMLHttpRequest();

    var login = document.getElementById("auth_login").value;
    var password = document.getElementById("auth_password").value;

    var data = JSON.stringify({"login": login, "password": password});
    http.open('POST', 'api/users/login', true);

    http.setRequestHeader('Content-type', 'application/json');

    http.onreadystatechange = function() {
        if (http.readyState == 4 && http.status == 200) {
            var data = JSON.parse(http.responseText);
            sessionStorage.setItem("access_token", data.access_token);
            sessionStorage.setItem("id", data.id);
            document.location.replace("/");
        }
    }

    http.send(data);
}