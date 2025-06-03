document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchSerial');
    const productList = document.getElementById('productList');
    const products = window.products;

    function renderProducts(products) {
        if (products.length === 0) {
            productList.innerHTML = '<p>Geen producten gevonden</p>';
            return;
        }

        productList.innerHTML = products.map(product => `
            <div class="d-flex border rounded p-3 align-items-center mb-3">
                <div style="width: 100px; height: 100px; overflow: hidden;" class="me-3">
                    ${product.picture?.length
                        ? `<img src="data:image/png;base64,${product.picture}" alt="${product.name}" class="img-fluid" style="max-height: 100%; object-fit: contain;" />`
                        : `<div class="bg-secondary text-white d-flex justify-content-center align-items-center" style="height: 100px; width: 100px;">Geen foto</div>`
                    }
                </div>

                <div class="flex-grow-1 d-flex flex-column justify-content-between ms-3">
                    <h5 class="mb-1">${product.name}</h5>
                    <small class="text-muted">Serienummer: ${product.productId}</small>
                    <p class="mb-0">${product.description}</p>
                </div>

                <div class="d-flex flex-column justify-content-center align-items-center mx-3" style="min-width: 120px;">
                    <div><strong>Voorraad:</strong> ${product.stock}</div>
                </div>

                <div class="d-flex flex-column justify-content-center align-items-end" style="min-width: 120px;">
                    <div><strong>${product.price.toLocaleString('nl-NL', { style: 'currency', currency: 'EUR' })}</strong></div>
                    <div class="text-muted"><small>Incl. btw: ${(product.price * 1.21).toLocaleString('nl-NL', { style: 'currency', currency: 'EUR' })}</small></div>
                </div>
            </div>
        `).join('');
    }

    function debounce(func, delay) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), delay);
        }
    }

    searchInput.addEventListener('input', debounce(() => {
        const query = searchInput.value.trim().toLowerCase();

        if (!query) {
            renderProducts(products);
            return;
        }

        const filtered = products.filter(p => p.productId.toString().includes(query));
        renderProducts(filtered);
    }, 300));

    renderProducts(products);
});