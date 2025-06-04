document.addEventListener('DOMContentLoaded', function () {
    const pageSize = 5;

    document.getElementById('load-more-bezorgers')?.addEventListener('click', function () {
        const rows = document.querySelectorAll('.bezorger-row.d-none');
        for (let i = 0; i < pageSize && i < rows.length; i++) {
            rows[i].classList.remove('d-none');
        }
        if (document.querySelectorAll('.bezorger-row.d-none').length === 0) {
            this.style.display = 'none';
        }
    });

    document.getElementById('load-more-admins')?.addEventListener('click', function () {
        const rows = document.querySelectorAll('.admin-row.d-none');
        for (let i = 0; i < pageSize && i < rows.length; i++) {
            rows[i].classList.remove('d-none');
        }
        if (document.querySelectorAll('.admin-row.d-none').length === 0) {
            this.style.display = 'none';
        }
    });
});