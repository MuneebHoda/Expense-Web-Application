document.addEventListener('DOMContentLoaded', function () {
    const defaultSelectedItem = document.querySelector('.dashboard-item[data-content="charts-content"]');

    if (defaultSelectedItem) {
        defaultSelectedItem.classList.add('selected');
        loadChartsDashboard();
    }


    const dashboardItems = document.querySelectorAll('.dashboard-item');

    dashboardItems.forEach(item => {
        item.addEventListener('click', function () {
            dashboardItems.forEach(i => i.classList.remove('selected'));
            this.classList.add('selected');

            if (this.dataset.content === "charts-content") {
                loadChartsDashboard();
            }
            else if (this.dataset.content === "transaction-content") {
                loadTransactionDashboard();
            }
        });
    });

    function loadChartsDashboard() {
        $.ajax({
            url: '/Admin/LoadChartsPartialView',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;

                initializeCharts();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function loadTransactionDashboard() {
        $.ajax({
            url: '/Admin/LoadTransactionDashboard',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function initializeCharts() {
        var userTotalAmounts = {};
        var statusCounts = {
            Pending: 0,
            Approved: 0,
            Rejected: 0
        };

        var monthlyTotals = Array(12).fill(0);

        expenseFormsData.forEach(function (expense) {
            var date = new Date(parseInt(expense.SubmittedDate.replace(/\/Date\((\d+)\)\//, '$1')));
            var month = date.getMonth();
            console.log(month);
            if (typeof expense.TotalAmount === 'number') {
                monthlyTotals[month] += expense.TotalAmount;
            } else {
                console.error("Invalid TotalAmount for expense:", expense);
            }

            if (!userTotalAmounts[expense.UserId]) {
                userTotalAmounts[expense.UserId] = 0;
            }
            userTotalAmounts[expense.UserId] += expense.Amount;

            if (expense.Status === "Pending") {
                statusCounts.Pending++;
            } else if (expense.Status === "Approved") {
                statusCounts.Approved++;
            } else if (expense.Status === "Rejected") {
                statusCounts.Rejected++;
            }
        });

        var monthLabels = [
            'January', 'February', 'March', 'April',
            'May', 'June', 'July', 'August',
            'September', 'October', 'November', 'December'
        ];

        var ctx1 = document.getElementById('totalAmountChart').getContext('2d');

        if (ctx1) {
            var totalAmountChart = new Chart(ctx1, {
                type: 'bar',
                data: {
                    labels: monthLabels,
                    datasets: [{
                        label: 'Total Amount',
                        data: monthlyTotals,
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.6)', // January - Red
                            'rgba(255, 159, 64, 0.6)', // February - Light Red
                            'rgba(255, 205, 86, 0.6)', // March - Peach
                            'rgba(255, 99, 132, 0.6)', // April - Red
                            'rgba(255, 159, 64, 0.6)', // May - Light Red
                            'rgba(255, 205, 86, 0.6)', // June - Peach
                            'rgba(255, 99, 132, 0.6)', // July - Red
                            'rgba(255, 159, 64, 0.6)', // August - Light Red
                            'rgba(255, 205, 86, 0.6)', // September - Peach
                            'rgba(255, 99, 132, 0.6)', // October - Red
                            'rgba(255, 159, 64, 0.6)', // November - Light Red
                            'rgba(255, 205, 86, 0.6)'  // December - Peach
                        ],
                        borderColor: [
                            'rgba(255, 99, 132, 1)', // January - Red
                            'rgba(255, 159, 64, 1)', // February - Light Red
                            'rgba(255, 205, 86, 1)', // March - Peach
                            'rgba(255, 99, 132, 1)', // April - Red
                            'rgba(255, 159, 64, 1)', // May - Light Red
                            'rgba(255, 205, 86, 1)', // June - Peach
                            'rgba(255, 99, 132, 1)', // July - Red
                            'rgba(255, 159, 64, 1)', // August - Light Red
                            'rgba(255, 205, 86, 1)', // September - Peach
                            'rgba(255, 99, 132, 1)', // October - Red
                            'rgba(255, 159, 64, 1)', // November - Light Red
                            'rgba(255, 205, 86, 1)'  // December - Peach
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Total Amount'
                            }
                        },
                        x: {
                            title: {
                                display: true,
                                text: 'Month'
                            }
                        }
                    }
                }
            });
        }

        var statusLabels = Object.keys(statusCounts);
        var statusData = Object.values(statusCounts);
        var ctx2 = document.getElementById('expenseStatusChart').getContext('2d');

        if (ctx2) {
            var expenseStatusChart = new Chart(ctx2, {
                type: 'pie',
                data: {
                    labels: statusLabels,
                    datasets: [{
                        label: 'Expense Status',
                        data: statusData,
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.6)', // Pending - Red
                            'rgba(255, 159, 64, 0.6)', // Approved - Light Red
                            'rgba(255, 205, 86, 0.6)'  // Rejected - Peach
                        ],
                        borderColor: [
                            'rgba(255, 99, 132, 1)', // Pending - Red
                            'rgba(255, 159, 64, 1)', // Approved - Light Red
                            'rgba(255, 205, 86, 1)'  // Rejected - Peach
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true
                }
            });
        }
    }
});
