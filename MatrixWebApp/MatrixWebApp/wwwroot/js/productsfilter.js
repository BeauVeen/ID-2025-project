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
    const ascButton = document.querySelector('button[onclick="sortProducts(\'price-asc\')"]');
    const descButton = document.querySelector('button[onclick="sortProducts(\'price-desc\')"]');
    const currentActiveButton = sortType === 'price-asc' ? ascButton : descButton;

    // Als de geklikte knop al active is, verwijder dan de active class
    if (currentActiveButton.classList.contains('active')) {
        currentActiveButton.classList.remove('active');
        // Reset de sorteer volgorde naar origineel (optioneel)
        products.sort((a, b) => {
            return parseInt(a.dataset.index) - parseInt(b.dataset.index);
        });
    } else {
        // Verwijder active class van beide knoppen
        ascButton.classList.remove('active');
        descButton.classList.remove('active');

        // Voeg active class toe aan de geklikte knop
        currentActiveButton.classList.add('active');

        // Sorteer de producten
        products.sort((a, b) => {
            const priceA = parseFloat(a.querySelector('.price-text').textContent.replace('€', ''));
            const priceB = parseFloat(b.querySelector('.price-text').textContent.replace('€', ''));

            return sortType === 'price-asc' ? priceA - priceB : priceB - priceA;
        });
    }

    // Voeg de gesorteerde producten weer toe aan de container
    products.forEach(product => container.appendChild(product));
    document.getElementById('filterDropdown')?.classList.remove('show');
}