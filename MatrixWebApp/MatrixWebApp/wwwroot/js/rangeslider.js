document.addEventListener('DOMContentLoaded', function() {
    const rangeInputs = document.querySelectorAll('.range-input input');
    const priceInputs = document.querySelectorAll('.price-input input');
    const progress = document.querySelector('.slider .progress');
    const priceGap = 10;
    
    // Haal maxPrice op uit het data-attribuut
    const maxPriceValue = parseFloat(document.querySelector('.range-input').dataset.maxPrice);

    if (rangeInputs.length === 2 && priceInputs.length === 2 && progress) {
        let minPrice = 0;
        let currentMaxPrice = maxPriceValue;

        // Zet de max waarden voor de range inputs
        rangeInputs[0].max = maxPriceValue;
        rangeInputs[1].max = maxPriceValue;
        rangeInputs[1].value = maxPriceValue;

        // Update de progress bar en prijzen
        function updateSlider() {
            priceInputs[0].value = minPrice;
            priceInputs[1].value = currentMaxPrice;
            progress.style.left = (minPrice / maxPriceValue) * 100 + "%";
            progress.style.right = 100 - (currentMaxPrice / maxPriceValue) * 100 + "%";
            filterProductsByPrice(minPrice, currentMaxPrice);
        }

        // Event listeners voor range sliders
        rangeInputs.forEach(input => {
            input.addEventListener('input', function(e) {
                let currentMin = parseInt(rangeInputs[0].value);
                let currentMax = parseInt(rangeInputs[1].value);

                if (currentMax - currentMin < priceGap) {
                    if (e.target === rangeInputs[0]) {
                        rangeInputs[0].value = currentMax - priceGap;
                    } else {
                        rangeInputs[1].value = currentMin + priceGap;
                    }
                } else {
                    minPrice = currentMin;
                    currentMaxPrice = currentMax;
                    updateSlider();
                }
            });
        });

        // Event listeners voor prijs inputs
        priceInputs.forEach(input => {
            input.addEventListener('input', function(e) {
                let value = parseInt(e.target.value) || 0;
                
                if (e.target === priceInputs[0]) { // Min prijs
                    value = Math.min(value, currentMaxPrice - priceGap);
                    minPrice = value;
                    rangeInputs[0].value = value;
                } else { // Max prijs
                    value = Math.max(value, minPrice + priceGap);
                    currentMaxPrice = value;
                    rangeInputs[1].value = value;
                }
                
                updateSlider();
            });
        });

        // Debounced input voor direct typen
        priceInputs.forEach(input => {
            input.addEventListener('keyup', debounce(function(e) {
                let value = parseInt(e.target.value) || 0;
                
                if (e.target === priceInputs[0]) {
                    value = Math.min(value, currentMaxPrice - priceGap);
                    minPrice = value;
                    rangeInputs[0].value = value;
                } else {
                    value = Math.max(value, minPrice + priceGap);
                    currentMaxPrice = value;
                    rangeInputs[1].value = value;
                }
                
                updateSlider();
            }, 500));
        });

        // Filter producten op prijs
        function filterProductsByPrice(min, max) {
            document.querySelectorAll('.product-card').forEach(card => {
                const priceText = card.querySelector('.price-text').textContent;
                const price = parseFloat(priceText.replace(/[^\d.,]/g, '').replace(',', '.'));
                
                if (price >= min && price <= max) {
                    card.closest('.col-lg-3').style.display = 'block';
                } else {
                    card.closest('.col-lg-3').style.display = 'none';
                }
            });
        }

        // Debounce functie
        function debounce(func, wait) {
            let timeout;
            return function() {
                const context = this, args = arguments;
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(context, args), wait);
            };
        }

        // Initialiseer de slider waarden
        updateSlider();
    }
});