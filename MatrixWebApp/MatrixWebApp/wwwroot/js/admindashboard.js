document.addEventListener('DOMContentLoaded', function () {
    if (typeof dashboardData === 'undefined') return;

    // Customers Line Chart
    var ctxCustomers = document.getElementById('customersLineChart').getContext('2d');
    new Chart(ctxCustomers, {
        type: 'line',
        data: {
            labels: dashboardData.customerMonths,
            datasets: [{
                label: 'Nieuwe klanten per maand',
                data: dashboardData.customerCounts,
                borderColor: '#007bff',
                backgroundColor: 'rgba(0,123,255,0.1)',
                fill: true,
                tension: 0.3
            }]
        },
        options: { scales: { y: { beginAtZero: true } } }
    });

    // Orders Line Chart
    var ctxOrders = document.getElementById('salesLineChart').getContext('2d');
    new Chart(ctxOrders, {
        type: 'line',
        data: {
            labels: dashboardData.ordersMonths,
            datasets: [{
                label: 'Bestellingen afgerond per maand',
                data: dashboardData.ordersCounts,
                borderColor: '#28a745',
                backgroundColor: 'rgba(40,167,69,0.1)',
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                    // No max property here!
                }
            }
        }
    });
});