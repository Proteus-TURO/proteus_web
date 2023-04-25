window.addEventListener('load', async function () {
    const link = document.querySelector('.sub-menu li:first-child a');
    const titles = await getTitles();
    console.log(titles);

    addNewLinks(titles);
});

window.onpopstate = function(event) {
    if (event.state && event.state.opened) {
        console.log(event.state.opened);
    }
};

let fullName = document.getElementById("user-name");
let title = document.getElementById("user-title");
fullName.innerHTML = localStorage.getItem("fullName");
title.innerHTML = localStorage.getItem("title");

let arrow = document.querySelectorAll(".arrow");
for (let i = 0; i < arrow.length; i++) {
    arrow[i].addEventListener("click", (e) => {
        let arrowParent = e.target.parentElement.parentElement;//selecting main parent of arrow
        arrowParent.classList.toggle("showMenu");
    });
}
let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");
console.log(sidebarBtn);
sidebarBtn.addEventListener("click", () => {
    sidebar.classList.toggle("close");
});

//document.getElementById('markdown').innerHTML = marked.parse('# Marked in the browser\n\nRendered by **marked**.');


//Diary Functional Code
function startEditor() {
    if (document.getElementById('editButton')) {
        document.getElementById('editButton').remove();
    }
    // Erstelle HTML-Code für die Textfelder
    if (!document.getElementById("text-editor-form")) {
        const formHTML = `
    <form id="text-editor-form">
      <div class="overDiv">
        <div class="text-editor-input">
          <label for="title-input">Titel:</label>
          <input id="title-input" name="title-input"></input>
        </div>
        <div class="text-editor-input">
          <label for="content-input">Content:</label>
          <textarea id="content-input" name="content-input"></textarea>
        </div>
      </div>  
      <button type="submit" onClick="createContent()" id="post-button">Erstellen</button>
    </form>
  `;

        // Füge das Textfeld zum Dokument hinzu
        const homeContent = document.querySelector('.home-content');
        homeContent.insertAdjacentHTML('afterend', formHTML);

        // Ändere das Grid-Layout der home-content Klasse
        homeContent.style.display = 'grid';
        homeContent.style.gridTemplateColumns = '1fr';
        homeContent.style.gridTemplateRows = 'repeat(4, auto)';
        homeContent.style.gap = '1rem';
    }

    // Fange das Submit-Event ab
    const form = document.getElementById('text-editor-form');
    form.addEventListener('submit', (event) => {
        event.preventDefault();
        // Hole die eingegebenen Daten
        const title = document.getElementById('title-input').value;
        const content = document.getElementById('content-input').value;
        // Sende die Daten an die Datenbank (ersetze diesen Teil mit deiner eigenen Code-Implementierung)
        sendDataToDatabase(title, content);
        // Entferne das Textfeld und den Edit-Button
        form.remove();
    });

    // Platziere die Textfelder im Grid-Container
    const titleInput = document.getElementById('title-input');
    titleInput.style.gridRow = '1';
    const contentInput = document.getElementById('content-input');
    contentInput.style.gridRow = '2';
    const postButton = document.getElementById('post-button');
    postButton.style.gridRow = '3';

    //Ausblenden von Titel und Content
    console.log("Test");
    document.getElementById("titleDB").textContent = "";
    document.getElementById("contentInfo").textContent = "";
}

// Füge einen Event Listener zum Edit Button hinzu
/////////const editButton = document.querySelector('#editButton');
////////editButton.addEventListener('click', startEditor);

// Funktion zum Senden der Daten an die Datenbank
function sendDataToDatabase(title, content) {
    // Ersetze diesen Teil mit deiner eigenen Code-Implementierung, um die Daten an die Datenbank zu senden
    console.log(`Titel: ${title}`);
    console.log(`Content: ${content}`);
}


//DATABASE
// finden Sie das Element "bx bx-log-out"
let logOutButton = document.getElementById('logoutbtn');
console.log(logOutButton);
// Hinzufügen eines Klickereignis-Listeners auf das "bx bx-log-out" -Symbol
logOutButton.addEventListener('click', logOut);

// Funktion, die aufgerufen wird, wenn auf das "bx bx-log-out" -Symbol geklickt wird
async function logOut() {
    localStorage.removeItem("token");
    localStorage.removeItem("fullName");
    localStorage.removeItem("title");
    localStorage.removeItem("role");
    console.log("Vorgang Ausloggen!");
    // Hier können Sie den Code schreiben, der den Benutzer ausloggt und ihn zur Anmeldeseite weiterleitet
    const LOGOUT_API = `/api/Logout`;
    try {
        await fetch(LOGOUT_API, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "accept": "*/*"
            }
        });
        window.location.href = "/login/"
    } catch (e) {
        console.log(error)
    }
}

async function getTitles() {
    const response = await fetch("/api/Article/GetTitles");
    const data = await response.json();

    const titles = data.diary ? data.diary.join(', ') : '';

    return titles;
}

function createContent() {
    const topic = "diary";
    const title = document.getElementById("title-input").value;
    const content = document.getElementById("content-input").value;

    fetch("/api/Article/New", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            "topic": topic,
            "title": title,
            "content": content
        })
    })
        .then(response => response.json())
        .then(data => {
            console.log("Erfolgreich! " + data);
            location.reload();
        })
        .catch(error => {
            console.error("Fehler! " + error);
            location.reload();
        });
}

function deleteElement() {

}

async function addNewLinks(titles) {
    const link = document.querySelector('.sub-menu li:nth-child(2) a');
    const newLinkContainer = document.getElementById('newLink');

    titles.split(', ').forEach(title => {
        const newLink = link.cloneNode(true);
        newLink.textContent = `${title}`;
        newLink.setAttribute('href', '#');
        newLink.setAttribute('onclick', `setInformation("${title}", " ${newLink}")`);

        newLinkContainer.parentNode.insertBefore(newLink, newLinkContainer);
    });
}

function setInformation(title, item) {
    let element = document.getElementById("text-editor-form");
    if (element) {
        element.remove();
    } else {
        console.log("Gibt es nicht! " + element);
    }

    const tit = document.getElementById("titleDB");
    tit.textContent = title;
    tit.style.marginLeft = 4 + '%';

    setContent(title);
}

async function setContent(title) {
    const response = await fetch(`/api/Article/GetContent?topic=diary&title=${encodeURIComponent(title)}`);
    const data = await response.text();

    const info = document.getElementById("contentInfo");
    info.textContent = data;
    info.style.marginLeft = 4 + '%';

    const btn = document.getElementById("delbtn");
    btn.style.visibility = "visible";
}