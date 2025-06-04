// Toon/verberg dropdown wanneer op filter-icoon wordt geklikt
document.getElementById('filterIcon')?.addEventListener('click', function (e) {
    e.stopPropagation(); // Voorkom dat het klikevent direct wordt afgehandeld
    const dropdown = document.getElementById('filterDropdown');
    dropdown?.classList.toggle('show');
});

// Sluit dropdown wanneer er buiten wordt geklikt
window.addEventListener('click', function () {
    document.getElementById('filterDropdown')?.classList.remove('show');
});

// Voorkom dat dropdown sluit wanneer er in de dropdown wordt geklikt
document.getElementById('filterDropdown')?.addEventListener('click', function (e) {
    e.stopPropagation();
});

// Sorteerfuncties
function sortProducts(sortType) {
    const container = document.querySelector('.product-background .row');
    const products = Array.from(document.querySelectorAll('.col-lg-3'));

    products.sort((a, b) => {
        const priceA = parseFloat(a.querySelector('.price-text').textContent.replace('€', ''));
        const priceB = parseFloat(b.querySelector('.price-text').textContent.replace('€', ''));

        return sortType === 'price-asc' ? priceA - priceB : priceB - priceA;
    });

    products.forEach(product => container.appendChild(product));
    document.getElementById('filterDropdown')?.classList.remove('show');
}