
const canvas = document.getElementById('matrix-rain');
const ctx = canvas.getContext('2d');

// Pas canvas grootte aan
function resizeCanvas() {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    columns = Math.floor(canvas.width / font_size);
    drops = Array(columns).fill(1);
}

// Karakters - uitgebreid met meer "matrix" tekens
const chars = "01";
const font_size = 15;
let columns;
let drops = [];

resizeCanvas(); // Initialiseer grootte

// Tekenen
function draw() {
    // Semi-transparante overlay voor vage trail
    ctx.fillStyle = 'rgba(0, 0, 0, 0.05)';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Matrix groene kleur
    ctx.fillStyle = '#00ff41';
    ctx.font = font_size + 'px monospace';

    for (let i = 0; i < drops.length; i++) {
        const text = chars[Math.floor(Math.random() * chars.length)];
        ctx.fillText(text, i * font_size, drops[i] * font_size);

        if (drops[i] * font_size > canvas.height && Math.random() > 0.975) {
            drops[i] = 0;
        }

        drops[i]++;
    }
}

// Window resize handler
window.addEventListener('resize', resizeCanvas);

// Start animatie
setInterval(draw, 33);