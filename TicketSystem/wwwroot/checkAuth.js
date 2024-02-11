var authToken = sessionStorage.getItem("access_token")

if(authToken == undefined)
window.open("auth");