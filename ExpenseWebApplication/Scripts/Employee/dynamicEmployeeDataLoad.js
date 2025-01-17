document.addEventListener('DOMContentLoaded', function () {
    const defaultSelectedItem = document.querySelector('.dashboard-item[data-content="dashboard-content"]');

    if (defaultSelectedItem) {
        defaultSelectedItem.classList.add('selected');
        loadDashboardExpenses();
    }


    const dashboardItems = document.querySelectorAll('.dashboard-item');

    dashboardItems.forEach(item => {
        item.addEventListener('click', function () {
            dashboardItems.forEach(i => i.classList.remove('selected'));
            this.classList.add('selected');

            if (this.dataset.content === "dashboard-content") {
                loadDashboardExpenses();
            }
            else if (this.dataset.content === "pending-content") {
                loadPendingExpenses();
            }
            else if (this.dataset.content === "rejected-content") {
                loadRejectedExpenses();
            }
            else if (this.dataset.content === "approved-content") {
                loadAcceptedExpenses()
            }
            else if (this.dataset.content === "progress-content") {
                loadApprovalProgressExpenses()
            }
        });
    });

    function loadDashboardExpenses() {
        $.ajax({
            url: '/Employee/LoadEmployeeDashboardExpenses',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer'); 
                dashboardExpensesContainer.innerHTML = html;
                initializeFilter();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function loadPendingExpenses() {
        $.ajax({
            url: '/Employee/LoadEmployeePendingExpenses',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;
                initializeFilter();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function loadRejectedExpenses() {
        $.ajax({
            url: '/Employee/LoadEmployeeRejectedExpenses',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;
                initializeFilter();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function loadAcceptedExpenses() {
        $.ajax({
            url: '/Employee/LoadEmployeeAcceptedExpenses',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;
                initializeFilter();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }

    function loadApprovalProgressExpenses() {
        $.ajax({
            url: '/Employee/LoadApprovalProgressExpenses',
            type: 'GET',
            success: function (html) {

                const dashboardExpensesContainer = document.getElementById('expenseContainer');
                dashboardExpensesContainer.innerHTML = html;
                initializeFilter();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching expenses:', textStatus, errorThrown);
            }
        });
    }
    function initializeFilter() {
        if (typeof initializeFilteringLogic === 'function') {
            initializeFilteringLogic();
        }
    }

    attachEventListeners();
});
