function searchProducts() {
    const searchTerm = document.getElementById('navbarSearch').value.toLowerCase();
    window.location.href = '/Products?query=' + encodeURIComponent(searchTerm);
}

document.addEventListener('DOMContentLoaded', function () {
    const searchBox = document.getElementById('navbarSearch');

    // Alleen doorgaan als het element bestaat
    if (!searchBox) return;

    // Vul zoekveld als query aanwezig is
    const urlParams = new URLSearchParams(window.location.search);
    const query = urlParams.get('query');

    if (query && window.location.pathname.toLowerCase().includes('products')) {
        searchBox.value = query;

        // Kleine vertraging om zeker te zijn dat alle producten geladen zijn
        setTimeout(() => {
            document.querySelectorAll('.col-lg-3').forEach(col => {
                const title = col.querySelector('.card-title').textContent.toLowerCase();
                col.style.display = title.includes(query.toLowerCase()) ? '' : 'none';
            });
        }, 50);
    }

    // Enter-to-search functionaliteit
    searchBox.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            searchProducts();
        }
    });
});