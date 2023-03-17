const loginButton = document.getElementById("logout-btn")

function logout() {
    const LOGIN_API = "https://localhost:12346/api/logout";
    return new Promise((resolve, reject) => {
        fetch(LOGIN_API, {
            method: "POST"
        }).then(response => {
            return response;
        }).then(data => {
            if (data.status) {
                location.href = data.url;
                resolve();
            }
        }).catch(error => {
            reject(error);
        });
    });
}


loginButton.addEventListener("click", async () => {
    try {
        await logout();
        console.log("Successfully logged out");
    } catch (e) {
        console.error(e);
    }
})

document.getElementById('markdown').innerHTML = marked.parse('# Marked in the browser\n\nRendered by **marked**.');