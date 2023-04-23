$.getScript("https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js", function () {
    particlesJS('particles-js',
        {
            "particles": {
                "number": {
                    "value": 100,
                    "density": {
                        "enable": true,
                        "value_area": 500
                    }
                },
                "color": {
                    "value": "#00a9ff"
                },
                "shape": {
                    "type": "circle",
                    "stroke": {
                        "width": 0,
                        "color": "#000000"
                    },
                    "polygon": {
                        "nb_sides": 5
                    },
                    "image": {
                        "width": 100,
                        "height": 100
                    }
                },
                "opacity": {
                    "value": 0.5,
                    "random": false,
                    "anim": {
                        "enable": false,
                        "speed": 1,
                        "opacity_min": 0.1,
                        "sync": false
                    }
                },
                "size": {
                    "value": 5,
                    "random": true,
                    "anim": {
                        "enable": false,
                        "speed": 40,
                        "size_min": 0.1,
                        "sync": false
                    }
                },
                "line_linked": {
                    "enable": true,
                    "distance": 150,
                    "color": "#ffffff",
                    "opacity": 0.4,
                    "width": 1
                },
                "move": {
                    "enable": true,
                    "speed": 6,
                    "direction": "none",
                    "random": false,
                    "straight": false,
                    "out_mode": "out",
                    "attract": {
                        "enable": false,
                        "rotateX": 600,
                        "rotateY": 1200
                    }
                }
            },
            "interactivity": {
                "detect_on": "canvas",
                "events": {
                    "onhover": {
                        "enable": true,
                        "mode": "repulse"
                    },
                    "onclick": {
                        "enable": true,
                        "mode": "push"
                    },
                    "resize": true
                },
                "modes": {
                    "grab": {
                        "distance": 400,
                        "line_linked": {
                            "opacity": 1
                        }
                    },
                    "bubble": {
                        "distance": 400,
                        "size": 40,
                        "duration": 2,
                        "opacity": 8,
                        "speed": 3
                    },
                    "repulse": {
                        "distance": 100
                    },
                    "push": {
                        "particles_nb": 4
                    },
                    "remove": {
                        "particles_nb": 2
                    }
                }
            },
            "retina_detect": true,
            "config_demo": {
                "hide_card": false,
                "background_color": "#b61924",
                "background_image": "",
                "background_position": "50% 50%",
                "background_repeat": "no-repeat",
                "background_size": "cover"
            }
        }
    );

});

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
        console.log(e);
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
                reject(`Unauthorized ${response.status} ${response.statusText}`);
                return;
            }
            else if (response.status !== 200 || !response.ok) {
                reject(`Server Error ${response.status} ${response.statusText}`);
                return;
            }
            resolve(response.json());
        });
    });
}


//LOGIN SCRIPT
function _login() {
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
                reject(`Unauthorized ${response.status} ${response.statusText}`);
                return;
            }
            else if (response.status !== 200 || !response.ok) {
                reject(`Server Error ${response.status} ${response.statusText}`);
                return;
            }
            resolve(response.json());
        });
    });
}
