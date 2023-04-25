let fullName = document.getElementById("user-name");
let title = document.getElementById("user-title");
let people = document.getElementById("people");
fullName.innerHTML = localStorage.getItem("fullName");
title.innerHTML = localStorage.getItem("title");
let role = localStorage.getItem("role");
if (role === 'administrator') {
    people.style.display = 'block';
}
const titles = await getTitles();

await addNewLinks(titles);

async function getTitles() {
    const response = await fetch("/api/Article/GetTitles");
    return await response.json();
}

async function addNewLinks(titles) {
    const link = document.querySelector('.sub-menu li:nth-child(2) a');
    const newDiary = document.getElementById('newDiaryEntry');
    const newDoc = document.getElementById('newDocEntry');

    if ('diary' in titles) {
        titles['diary'].forEach(title => {
            const newLink = link.cloneNode(true);
            newLink.textContent = `${title}`;
            newLink.setAttribute('href', `/private/web/diary/${title}`);

            newDiary.parentNode.insertBefore(newLink, newDiary);
        });
    }
    
    if ('doc' in titles) {
        titles['doc'].forEach(title => {
            const newLink = link.cloneNode(true);
            newLink.textContent = `${title}`;
            newLink.setAttribute('href', `/private/web/doc/${title}`);

            newDoc.parentNode.insertBefore(newLink, newDoc);
        });
    }

    let role = localStorage.getItem("role");
    if (role !== 'editor' && role !== 'administrator') {
        newDiary.style.display = 'none';
        newDoc.style.display = 'none';
    }
}