var userTotalAmounts = {};
var statusCounts = {
    Pending: 0,
    Approved: 0,
    Rejected: 0
};

expenseFormsData.forEach(function (expense) {
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

var totalAmountLabels = Object.keys(userTotalAmounts);
var totalAmountData = Object.values(userTotalAmounts);

var ctx1 = document.getElementById('totalAmountChart').getContext('2d');
var totalAmountChart = new Chart(ctx1, {
    type: 'bar',
    data: {
        labels: totalAmountLabels,
        datasets: [{
            label: 'Total Amount',
            data: totalAmountData,
            backgroundColor: 'rgba(54, 162, 235, 0.6)',
            borderColor: 'rgba(54, 162, 235, 1)',
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
                    text: 'User ID'
                }
            }
        }
    }
});

var statusLabels = Object.keys(statusCounts);
var statusData = Object.values(statusCounts);

var ctx2 = document.getElementById('expenseStatusChart').getContext('2d');
var expenseStatusChart = new Chart(ctx2, {
    type: 'pie',
    data: {
        labels: statusLabels,
        datasets: [{
            label: 'Expense Status',
            data: statusData,
            backgroundColor: [
                'rgba(255, 206, 86, 0.6)', 
                'rgba(75, 192, 192, 0.6)',  
                'rgba(255, 99, 132, 0.6)'   
            ],
            borderColor: [
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(255, 99, 132, 1)'
            ],
            borderWidth: 1
        }]
    },
    options: {
        responsive: true
    }
});