const LOGIN_API = "/api/Login";
const USER_INFO_API = "/api/User/GetUserInfo";

if (navigator.userAgentData.mobile) {
    console.log('mobile');
    document.getElementById('particles-js').style.display = 'none';
    let box = document.getElementById('login-box');
    box.style.zoom = Math.min(window.innerWidth / box.offsetWidth, window.innerHeight / box.offsetHeight);
}

document.addEventListener("keydown", async function (event) {
    if (event.key === "Enter") {
        await login();
    }
});

async function login() {
    try {
        localStorage.removeItem("token");
        localStorage.removeItem("fullName");
        localStorage.removeItem("title");
        localStorage.removeItem("role");
        let loginData = await _login();
        localStorage.setItem("token", loginData["token"]);
        let userInfo = await _getUserInfo();
        localStorage.setItem("fullName", userInfo["fullName"]);
        localStorage.setItem("role", userInfo["role"]);
        localStorage.setItem("title", userInfo["title"]);
        window.location.href = "../private/web";
    } catch (e) {
        let error = document.getElementById('error-message');
        error.innerHTML = e;
        error.style.display = 'block';
    }
    let box = document.getElementById('login-box');
    box.classList.remove('blur');
    let particles = document.getElementById('particles-js')
    particles.classList.remove('blur');
    let spinner = document.getElementById('spinner-container');
    spinner.style.display = 'none';
}

async function _getUserInfo() {
    return new Promise((resolve, reject) => {
        let apiToken = localStorage.getItem("token");
        if (apiToken === null) {
            reject("API Token is null");
        }
        
        fetch(USER_INFO_API, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "accept": "*/*",
                "Authorization": "Bearer " + apiToken
            }
        }).then(response => {
            if (response.status === 401) {
                reject(`Username or Password are incorrect`);
                return;
            }
            else if (response.status !== 200 || !response.ok) {
                reject(`Server Error: ${response.statusText}`);
                return;
            }
            resolve(response.json());
        });
    });
}


//LOGIN SCRIPT
function _login() {
    let error = document.getElementById('error-message');
    error.style.display = 'none';
    let box = document.getElementById('login-box');
    box.classList.add('blur');
    let particles = document.getElementById('particles-js')
    particles.classList.add('blur');
    let spinner = document.getElementById('spinner-container');
    spinner.style.display = 'flex';
    let usernameString = document.getElementById('user-field').value;
    let passwordString = document.getElementById('pass-field').value;

    let md = forge.md.sha256.create();
    md.update(passwordString);
    let buffer = md.digest().toHex();
    let arrayBuffer = forge.util.hexToBytes(buffer);
    let passwordHash = btoa(arrayBuffer);

    console.log(usernameString, passwordHash);

    return new Promise((resolve, reject) => {
        fetch(LOGIN_API, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "accept": "*/*"
            },
            body: JSON.stringify({
                "username": usernameString,
                "passwordHash": passwordHash
            })
        }).then(response => {
            if (response.status === 401) {
                reject(`Username or Password are incorrect`);
                return;
            }
            else if (response.status !== 200 || !response.ok) {
                reject(`Server Error: ${response.statusText}`);
                return;
            }
            resolve(response.json());
        });
    });
}
