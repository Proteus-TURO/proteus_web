const path = window.location.pathname;
const pathArray = path.split("/");
console.log(pathArray);
let markdownConverter = new showdown.Converter();
if (pathArray.length > 2 && pathArray[3] !== '') {
    let first = pathArray[3];
    let second = pathArray[4] || null;
    let third = pathArray[5] || null;
    
    
    if (first === 'diary' || first === 'doc') {
        let titles = await getTitles();
        if (second === null) {
            if (first in titles) {
                second = titles[first][0];
                document.location = `/private/web/${first}/${second}`
            }
        }
        if (first in titles && titles[first].includes(second)) {
            await setInformation(first, second);
        } else {
            // TODO Error
        }
    } else if (first === 'new') {
        const titleEditor = document.getElementById('title-editor');
        titleEditor.style.display = 'block';
        const submitButton = document.getElementsByClassName('submit-button')[0];
        submitButton.style.display = 'block';
        startMarkdownEditor(second, '', '');
    } else if (first === 'edit') {
        const submitButton = document.getElementsByClassName('submit-button')[0];
        submitButton.style.display = 'block';
        startMarkdownEditor(second, third, await getContent(second, third));
    } else if (first === 'monitoring') {
        // TODO
    } else if (first === 'people') {
        // TODO
    } else if (first === 'settings') {
        // TODO
    }
} else {
    console.log('Test');
    document.location = '/private/web/monitoring';
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
    let role = localStorage.getItem("role");
    if (role === 'editor' || role === 'administrator') {
        const editorButtons = document.getElementById("editor-button-container");
        editorButtons.style.display = 'flex';
        editorButtons.children[0].setAttribute('onclick', `window.location.href='/private/web/edit/${topic}/${title}';`);
        editorButtons.children[1].setAttribute("onClick", `deleteArticle("${topic}", "${title}")`);
        console.log(editorButtons);
    }

    await setContent(topic, title);
}

async function getContent(topic, title) {
    const response = await fetch(`/api/Article/${encodeURIComponent(topic)}/${encodeURIComponent(title)}`);
    const data = await response.json();
    return data['content'];
}

async function setContent(topic, title) {
    const data = await getContent(topic, title);

    const info = document.getElementById("content");
    info.innerHTML = markdownConverter.makeHtml(data);
    info.style.marginLeft = '4%';
    let spinner = document.getElementById('content-spinner');
    spinner.style.display = 'none';
}