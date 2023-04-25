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

let easyMDE;

function startMarkdownEditor(topic, title, content) {
    const element = document.getElementById('content');
    element.classList.add('markdown-editor');
    let textArea = document.createElement('textarea');
    element.appendChild(textArea);
    easyMDE = new EasyMDE({
        element: textArea,
        toolbar: ['bold', 'italic', 'strikethrough', '|', 'heading-1', 'heading-2', 'heading-3', '|', 'quote', 'unordered-list', 'ordered-list', 'table', '|', 'code', 'link',
            {
                name: 'upload-image',
                action: EasyMDE.drawUploadedImage,
                title: 'Upload File',
                className: 'fa fa-file-arrow-up'
            }
            , '|', 'preview', 'side-by-side', 'fullscreen', '|', 'undo', 'redo', 'guide'],
        renderingConfig: {
            codeSyntaxHighlighting: true
        },
        imageAccept: '*',
        uploadImage: true,
        imageUploadEndpoint: '/api/UploadFile',
        imageUploadFunction: function (file, onSuccess, onError) {
            let apiToken = localStorage.getItem("token");
            let fd = new FormData();
            fd.append('file', file);
            fetch('/api/UploadFile', {
                method: "POST",
                headers: {
                    "Authorization": "Bearer " + apiToken
                },
                body: fd
            }).then(response => {
                if (response.status === 201) {
                    onSuccess(response.headers.get('location'));
                } else {
                    onError('Something went wrong');
                }
            });
            /*setTimeout(function () {
                onSuccess("https://picsum.photos/200/300");
            }, 1000);*/
        }
    });
    console.log(content);
    easyMDE.value(content);

    if (title) {
        document.getElementById('titleDB').innerHTML = title;
        document.getElementsByClassName("submit-button")[0].setAttribute("onClick", `sendArticle("${topic}", "${title}", true)`);
        let markdownEditor = document.getElementsByClassName('markdown-editor')[0];
        markdownEditor.style.height = '65%';
        markdownEditor.style.marginTop = '20px';
    }
}

async function sendNewArticle() {
    const title = document.getElementById('title-input').value;
    const path = window.location.pathname;
    const pathArray = path.split("/");
    let topic = pathArray[4];
    await sendArticle(topic, title, false);
}

async function sendArticle(topic, title, isEdit) {
    let spinner = document.getElementById('content-spinner');
    spinner.style.display = 'flex';
    let method = "POST";
    if (isEdit) {
        method = "PUT";
    }
    const content = easyMDE.value();
    let apiToken = localStorage.getItem("token");
    let response = await fetch(
        `/api/Article/${encodeURIComponent(topic)}/${encodeURIComponent(title)}`, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "accept": "*/*",
                "Authorization": "Bearer " + apiToken
            },
            body: JSON.stringify({
                'content': content
            })
        }
    );

    console.log(response);

    if (!response.ok) {
        let errorMessage = document.getElementById('error-message');
        errorMessage.style.display = 'block';
        errorMessage.innerHTML = await response.text();
    } else {
        spinner = document.getElementById('content-spinner');
        spinner.style.display = 'none';
        document.location = `/private/web/${topic}/${title}`
    }
}

async function deleteArticle(topic, title) {
    let spinner = document.getElementById('content-spinner');
    spinner.style.display = 'flex';
    let apiToken = localStorage.getItem("token");
    let response = await fetch(
        `/api/Article/${encodeURIComponent(topic)}/${encodeURIComponent(title)}`, {
            method: "DELETE",
            headers: {
                "accept": "*/*",
                "Authorization": "Bearer " + apiToken
            }
        }
    );

    console.log(response);

    if (!response.ok) {
        let errorMessage = document.getElementById('error-message');
        errorMessage.style.display = 'block';
        errorMessage.innerHTML = await response.text();
    } else {
        spinner = document.getElementById('content-spinner');
        spinner.style.display = 'none';
        document.location = '/private/web/'
    }
}

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
          <input id="title-input" name="title-input">
        </div>
        <div class="text-editor-input text-area-div">
          <label for="content-input">Content:</label>
          <textarea id="content-input" name="content-input"></textarea>
        </div>
        <button type="submit" onClick="createContent()" id="post-button">Submit</button>
      </div>  
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