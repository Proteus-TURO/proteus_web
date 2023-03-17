
window.addEventListener('load', async function() {
    const link = document.querySelector('.sub-menu li:first-child a');
    const titles = await getTitles();
    console.log(titles);

    addNewLinks(titles);
});


let arrow = document.querySelectorAll(".arrow");
for (var i = 0; i < arrow.length; i++) {
  arrow[i].addEventListener("click", (e)=>{
 let arrowParent = e.target.parentElement.parentElement;//selecting main parent of arrow
 arrowParent.classList.toggle("showMenu");
  });
}
let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");
console.log(sidebarBtn);
sidebarBtn.addEventListener("click", ()=>{
  sidebar.classList.toggle("close");
});

//document.getElementById('markdown').innerHTML = marked.parse('# Marked in the browser\n\nRendered by **marked**.');


//Diary Functional Code
function startEditor() {

    if(document.getElementById('editButton')) {
        document.getElementById('editButton').remove();
    }
  // Erstelle HTML-Code für die Textfelder
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
      <button type="submit" id="post-button">Erstellen</button>
    </form>
  `;
  
  // Füge das Textfeld zum Dokument hinzu
  const homeContent = document.querySelector('.home-content');
  homeContent.insertAdjacentHTML('afterend', formHTML);
  
  // Fange das Submit-Event ab
  const form = document.querySelector('#text-editor-form');
  form.addEventListener('submit', (event) => {
    event.preventDefault();
    // Hole die eingegebenen Daten
    const title = document.querySelector('#title-input').value;
    const content = document.querySelector('#content-input').value;
    // Sende die Daten an die Datenbank (ersetze diesen Teil mit deiner eigenen Code-Implementierung)
    sendDataToDatabase(title, content);
    // Entferne das Textfeld und den Edit-Button
    form.remove();
    const editButton = document.querySelector('#editButton');
    editButton.remove();
  });

  // Ändere das Grid-Layout der home-content Klasse
  homeContent.style.display = 'grid';
  homeContent.style.gridTemplateColumns = '1fr';
  homeContent.style.gridTemplateRows = 'repeat(4, auto)';
  homeContent.style.gap = '1rem';

  // Platziere die Textfelder im Grid-Container
  const titleInput = document.querySelector('#title-input');
  titleInput.style.gridRow = '1';
  const contentInput = document.querySelector('#content-input');
  contentInput.style.gridRow = '2';
  const postButton = document.querySelector('#post-button');
  postButton.style.gridRow = '3';
  
  //Ändere den Titel zu "New Post", ausblenden Content
    document.getElementById("titleDB").textContent = "New Post";
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
function logOut() {
  console.log("Vorgang Ausloggen!");
  // Hier können Sie den Code schreiben, der den Benutzer ausloggt und ihn zur Anmeldeseite weiterleitet
  const LOGOUT_API = `https://${window.location.host}/api/Logout`;
  return new Promise((resolve, reject) => {
    fetch(LOGOUT_API, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "accept": "*/*"
        }
    });
  }).then(window.location.href = `https://${window.location.host}/index2.html`)
  .catch(error => console.log(error));
}
async function getTitles() {
    const response = await fetch(`https://${window.location.host}/api/Article/GetTitles`);
    const data = await response.json();

    const titles = data.Diary ? data.Diary.join(', ') : '';

    return titles;
}

async function addNewLinks(titles) {
    const link = document.querySelector('.sub-menu li:nth-child(2) a');

    titles.split(', ').forEach(title => {
        const newLink = link.cloneNode(true);
        newLink.textContent = `${title}`;
        newLink.setAttribute('href', '#');
        newLink.setAttribute('onclick', `setInformation("${title}")`);

        link.parentNode.insertBefore(newLink, link.nextSibling);
    });
}

function setInformation(title) {
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
async function setContent(title){
    const response = await fetch(`https://${window.location.host}/api/Article/GetContent?topic=Diary&title=${encodeURIComponent(title)}`);
    const data = await response.text();
    
    const info = document.getElementById("contentInfo");
    info.textContent = data;
    info.style.marginLeft = 4 + '%';
}