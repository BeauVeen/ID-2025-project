document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const loginMessage = document.getElementById('loginMessage');

    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value.trim();

        try {
            const response = await fetch('/api/User/authenticate', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });

            if (!response.ok) {
                throw new Error('Inloggen mislukt: verkeerde email of wachtwoord');
            }

            const data = await response.json();

            // Verwacht een token of user info terug (check je API)
            // Hier even voorbeeld met token:
            if (data.token) {
                localStorage.setItem('jwtToken', data.token);
                loginMessage.textContent = 'Inloggen succesvol, je wordt doorgestuurd...';
                loginMessage.style.color = 'green';

                // Redirect naar Account pagina na 1.5s
                setTimeout(() => {
                    window.location.href = '/Account';
                }, 1500);
            } else {
                // Fallback, als er geen token is:
                loginMessage.textContent = 'Inloggen mislukt: geen token ontvangen';
                loginMessage.style.color = 'red';
            }
        } catch (error) {
            loginMessage.textContent = error.message;
            loginMessage.style.color = 'red';
        }
    });
});
