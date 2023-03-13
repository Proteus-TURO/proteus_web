const loginButton = document.getElementById("login-btn")

function login(username, password) {
    const LOGIN_API = "https://localhost:12346/api/login";
    return new Promise((resolve, reject) => {
        fetch(LOGIN_API, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        }).then(response => {
            return response.json();
        }).then(data => {
            resolve(data["token"]);
        }).catch((error) => {
            reject(error);
        });
    });
}

loginButton.addEventListener("click", async () => {
    let username = "admin"
    let password = "JosuMagMajo#2022"

    try {
        let token = await login(username, password);
        alert(`You successfully logged into the private space. Here is your Toke: ${token}`);
    } catch (e) {
        console.error(e);
    }
})