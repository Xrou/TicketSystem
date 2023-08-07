function getInfo(){
    var http = new XMLHttpRequest();
    http.open('GET', `https://localhost:7177/api/users/me`, true);

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

getInfo();