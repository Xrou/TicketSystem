var authToken = sessionStorage.getItem("access_token")

if(authToken == undefined)
    document.location.replace("auth");