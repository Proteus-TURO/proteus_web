window.addEventListener('load', async function () {
    let fullName = document.getElementById("user-name");
    let title = document.getElementById("user-title");
    fullName.innerHTML = localStorage.getItem("fullName");
    title.innerHTML = localStorage.getItem("title");
    const link = document.querySelector('.sub-menu li:first-child a');
    const titles = await getTitles();
    console.log(titles);

    await addNewLinks(titles);
});

async function getTitles() {
    const response = await fetch("/api/Article/GetTitles");

    return await response.json();
}

async function addNewLinks(titles) {
    const link = document.querySelector('.sub-menu li:nth-child(2) a');
    const newDiary = document.getElementById('newDiaryEntry');
    const newDoc = document.getElementById('newDocEntry');

    titles['diary'].forEach(title => {
        const newLink = link.cloneNode(true);
        newLink.textContent = `${title}`;
        newLink.setAttribute('href', '#');
        newLink.setAttribute('onclick', `setInformation("diary", "${title}")`);

        newDiary.parentNode.insertBefore(newLink, newDiary);
    });

    titles['doc'].forEach(title => {
        const newLink = link.cloneNode(true);
        newLink.textContent = `${title}`;
        newLink.setAttribute('href', '#');
        newLink.setAttribute('onclick', `setInformation("doc", "${title}")`);

        newDoc.parentNode.insertBefore(newLink, newDoc);
    });
}

async function setInformation(topic, title) {
    let spinner = document.getElementById('content-spinner');
    spinner.style.display = 'flex';
    let element = document.getElementById("text-editor-form");
    if (element) {
        element.remove();
    }

    const tit = document.getElementById("titleDB");
    tit.textContent = title;
    tit.style.marginLeft = 4 + '%';
    const editorButtons = document.getElementById("editor-button-container");
    editorButtons.style.display = 'flex';

    await setContent(topic, title);
}

let markdownConverter = new showdown.Converter();

async function setContent(topic, title) {
    const response = await fetch(`/api/Article/${encodeURIComponent(topic)}/${encodeURIComponent(title)}`);
    const data = await response.json();

    const info = document.getElementById("content");
    let markdown = data['content'];
    info.innerHTML = markdownConverter.makeHtml(markdown);
    info.style.marginLeft = 4 + '%';
    let spinner = document.getElementById('content-spinner');
    spinner.style.display = 'none';
}