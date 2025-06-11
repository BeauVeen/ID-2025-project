function showNotification(message, type) {
    if (!message || message.startsWith('@')) return;

    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'fadeOut 0.5s ease-out';
        setTimeout(() => {
            notification.remove();
        }, 500);
    }, 3000);
}

document.addEventListener('DOMContentLoaded', function () {
    // Check all notification types
    [
        { id: 'notification-data', type: 'success' },
        { id: 'notification-warning', type: 'warning' },
        { id: 'notification-error', type: 'error' }
    ].forEach(notification => {
        const element = document.getElementById(notification.id);
        if (element && element.dataset.message) {
            showNotification(element.dataset.message, notification.type);
        }
    });
});