const html = document.documentElement;
const canvas = document.getElementById("canvas");
const context = canvas.getContext("2d");
const footer = document.querySelector("footer");

canvas.width = 1920;
canvas.height = 1080;

const frameCount = 473;
const currentFrame = index => (
    `/media/animation/tree_D_four-0${index.toString().padStart(4, '0')}.jpg`
);

function preloadImages() {
    setTimeout(_ => {
        if (!preloadImages.cache) {
            preloadImages.cache = [];
        }
        for (let i = 2; i < frameCount; i++) {
            const cacheImage = new Image();
            cacheImage.src = currentFrame(i);
            preloadImages.cache.push(cacheImage);
        }
    }, 0);
}

const img = new Image();
img.onload = _ => {
    context.drawImage(img, 0, 0);
    preloadImages();
};
img.src = currentFrame(1);

const updateImage = index => {
    img.src = currentFrame(index);
    context.drawImage(img, 0, 0);
}

const global_duration = 15;

let pause = [
    {name: 'Jetson Nano', frame: 102, duration: global_duration},
    {name: 'MassDuino', frame: 132, duration: global_duration},
    {name: 'Motor Driver', frame: 162, duration: global_duration},
    {name: 'Mecanum Wheels', frame: 192, duration: global_duration},
    {name: 'Realsense', frame: 222, duration: global_duration},
    {name: 'DC Motor', frame: 252, duration: global_duration},
    {name: 'Kilian', frame: 312, duration: global_duration},
    {name: 'Manuel M', frame: 332, duration: global_duration},
    {name: 'Manuel U', frame: 352, duration: global_duration},
    {name: 'Marcel T', frame: 372, duration: global_duration},
    {name: 'Maximilian', frame: 392, duration: global_duration},
    {name: 'Marcel W', frame: 412, duration: global_duration},
    {name: 'Group', frame: 432, duration: global_duration},
    {name: 'Powered by', frame: 453, duration: global_duration},
    {name: 'End', frame: 473, duration: 0},
]

let animationFrameCount = frameCount;
pause.forEach(p => {
    animationFrameCount += p.duration;

});

window.addEventListener('scroll', () => {
    const scrollTop = html.scrollTop;
    const maxScrollTop = html.scrollHeight - window.innerHeight;
    const scrollFraction = scrollTop / maxScrollTop;
    let animationFrameIndex = Math.ceil(animationFrameCount * scrollFraction);
    let paused = 0;
    for (let i = 0; i < pause.length; i++) {
        const p = pause[i];
        animationFrameIndex -= paused;
        if (animationFrameIndex >= p.frame) {
            if (animationFrameIndex < p.frame + p.duration) {
                animationFrameIndex = p.frame;
                break;
            } else {
                paused = p.duration;
            }
        } else {
            break;
        }
    }

    const frameIndex = Math.min(
        frameCount - 1,
        animationFrameIndex
    );
    if (frameIndex > frameCount - 5) {
        footer.classList.add('full-opacity');
    } else {
        footer.classList.remove('full-opacity');
    }

    requestAnimationFrame(() => updateImage(frameIndex + 1))
});