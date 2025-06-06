document.addEventListener('DOMContentLoaded', function () {
    if (typeof dashboardData === 'undefined') return;

    // Chart.js setup
    function renderCharts(months) {
        // Get the last N months for customers
        const customerLabels = dashboardData.customerMonths.slice(-months);
        const customerData = dashboardData.customerCounts.slice(-months);

        // Get the last N months for orders
        const orderLabels = dashboardData.ordersMonths.slice(-months);
        const orderData = dashboardData.ordersCounts.slice(-months);

        // Destroy existing charts if they exist
        if (window.customersChart) window.customersChart.destroy();
        if (window.ordersChart) window.ordersChart.destroy();

        // Customers chart
        window.customersChart = new Chart(document.getElementById('customersLineChart'), {
            type: 'line',
            data: {
                labels: customerLabels,
                datasets: [{
                    label: 'Nieuwe klanten',
                    data: customerData,
                    borderColor: 'rgba(54, 162, 235, 1)',
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    fill: true,
                    tension: 0.3
                }]
            },
            options: { responsive: true }
        });

        // Orders chart
        window.ordersChart = new Chart(document.getElementById('salesLineChart'), {
            type: 'line',
            data: {
                labels: orderLabels,
                datasets: [{
                    label: 'Bestellingen',
                    data: orderData,
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    fill: true,
                    tension: 0.3
                }]
            },
            options: { responsive: true }
        });
    }

    function renderCombinedChart(months) {
        const labels = dashboardData.customerMonths.slice(-months);
        const customerData = dashboardData.customerCounts.slice(-months);
        const orderData = dashboardData.ordersCounts.slice(-months);

        if (window.combinedChart) window.combinedChart.destroy();

        window.combinedChart = new Chart(document.getElementById('combinedLineChart'), {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Nieuwe klanten',
                        data: customerData,
                        borderColor: 'rgba(54, 162, 235, 1)',
                        backgroundColor: 'rgba(54, 162, 235, 0.2)',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'Bestellingen',
                        data: orderData,
                        borderColor: 'rgba(255, 99, 132, 1)',
                        backgroundColor: 'rgba(255, 99, 132, 0.2)',
                        fill: false,
                        tension: 0.3
                    }
                ]
            },
            options: { responsive: true }
        });
    }

    // Initial render (default 12 months)
    let currentMonths = 12;
    renderCharts(currentMonths);

    // Dropdown event
    document.getElementById('monthsSelect').addEventListener('change', function () {
        currentMonths = parseInt(this.value, 10);
        if (document.getElementById('combinedChartContainer').style.display === 'none') {
            renderCharts(currentMonths);
        } else {
            renderCombinedChart(currentMonths);
        }
    });

    document.getElementById('compareBtn').addEventListener('click', function () {
        // Hide split charts, show combined
        document.querySelectorAll('.col-md-6.mb-4').forEach(el => el.style.display = 'none');
        document.getElementById('combinedChartContainer').style.display = '';
        this.classList.add('d-none');
        document.getElementById('splitBtn').classList.remove('d-none');
        renderCombinedChart(currentMonths);
    });

    document.getElementById('splitBtn').addEventListener('click', function () {
        // Show split charts, hide combined
        document.querySelectorAll('.col-md-6.mb-4').forEach(el => el.style.display = '');
        document.getElementById('combinedChartContainer').style.display = 'none';
        this.classList.add('d-none');
        document.getElementById('compareBtn').classList.remove('d-none');
        renderCharts(currentMonths);
    });

    const pageSize = 5;
    const loadMoreBtn = document.getElementById('load-more-low-stock');
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', function (e) {
            e.preventDefault();
            const rows = Array.from(document.querySelectorAll('.low-stock-row.d-none'));
            for (let i = 0; i < pageSize && i < rows.length; i++) {
                rows[i].classList.remove('d-none');
            }
            if (rows.length <= pageSize) {
                loadMoreBtn.style.display = 'none';
            }
        });
    }
});