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

console.log(navigator);

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
        await _login();
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

    const LOGIN_API = `https://${window.location.host}/api/Login`;
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
            if (response.status >= 500) {
                window.alert(`Server Error ${response.statusText}`)
                reject(`Server Error ${response.status} ${response.statusText}`);
            }
            // TODO: Unauthorized
            return response.json();
        }) .then(data => {
            localStorage.setItem("key", data["token"]);
            window.location.href = "../private/personal.html";
        }).catch((error) => {
            console.log(`Fehler ${error}`);
            reject(error);
        });
    });
}
