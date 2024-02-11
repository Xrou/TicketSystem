function register() {
    var login = document.getElementById("login_input").value;
    var password = document.getElementById("password_input").value;
    var code = document.getElementById("code_input").value;

    var fullName = document.getElementById("fullname_input").value;
    var phone = document.getElementById("phone_input").value;
    var email = document.getElementById("email_input").value;
    var company = document.getElementById("company_input").value;

    var http = new XMLHttpRequest();
    http.open('POST', 'api/users/register', true);

    var ticketData = JSON.stringify({
        login: login,
        password: password,
        fullName: fullName,
        phoneNumber: phone,
        company: company,
        email: email,
        verificationCode: code
    });

    http.setRequestHeader('Content-type', 'application/json; charset=utf-8');

    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 201) {
            window.open("auth");
        }
    }

    http.send(ticketData);
}
