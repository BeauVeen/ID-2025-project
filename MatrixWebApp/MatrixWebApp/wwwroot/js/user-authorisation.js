document.addEventListener('DOMContentLoaded', function () {
    const pageSize = 5;

    function showMoreRows(rowClass, buttonId) {
        const rows = Array.from(document.querySelectorAll(rowClass + '.d-none'));
        for (let i = 0; i < pageSize && i < rows.length; i++) {
            rows[i].classList.remove('d-none');
        }
        if (rows.length <= pageSize) {
            document.getElementById(buttonId).style.display = 'none';
        }
    }

    document.getElementById('load-more-bezorgers')?.addEventListener('click', function (e) {
        e.preventDefault();
        showMoreRows('.bezorger-row', 'load-more-bezorgers');
    });

    document.getElementById('load-more-admins')?.addEventListener('click', function (e) {
        e.preventDefault();
        showMoreRows('.admin-row', 'load-more-admins');
    });

    function updateHeaderVisibility(listId, rowClass) {
        const list = document.getElementById(listId);
        const header = list.querySelector('.list-group-item.fw-bold');
        const anyVisible = Array.from(list.querySelectorAll(rowClass))
            .some(row => row.style.display !== 'none');
        header.style.display = anyVisible ? '' : 'none';
    }

    // Filter Bezorgers
    const searchBezorgers = document.getElementById('search-bezorgers');
    if (searchBezorgers) {
        searchBezorgers.addEventListener('input', function () {
            const query = this.value.trim().toLowerCase();
            document.querySelectorAll('#bezorgers-list .bezorger-row').forEach(row => {
                row.classList.remove('d-none');
                const cols = row.querySelectorAll('.col-3');
                const name = cols[0]?.textContent?.toLowerCase() ?? '';
                const email = cols[1]?.textContent?.toLowerCase() ?? '';
                const match = (name.includes(query) || email.includes(query));
                row.style.setProperty('display', match ? '' : 'none', 'important');
            });
            const loadMoreBtn = document.getElementById('load-more-bezorgers');
            if (loadMoreBtn) loadMoreBtn.style.display = query ? 'none' : '';
            updateHeaderVisibility('bezorgers-list', '.bezorger-row');
        });
    }

    // Filter Admins
    const searchAdmins = document.getElementById('search-admins');
    if (searchAdmins) {
        searchAdmins.addEventListener('input', function () {
            const query = this.value.trim().toLowerCase();
            document.querySelectorAll('#admins-list .admin-row').forEach(row => {
                row.classList.remove('d-none');
                const cols = row.querySelectorAll('.col-3');
                const name = cols[0].textContent.toLowerCase();
                const email = cols[1].textContent.toLowerCase();
                row.style.setProperty('display', (name.includes(query) || email.includes(query)) ? '' : 'none', 'important');
            });
            const loadMoreBtn = document.getElementById('load-more-admins');
            if (loadMoreBtn) loadMoreBtn.style.display = query ? 'none' : '';
            updateHeaderVisibility('admins-list', '.admin-row');
        });
    }
});