document.addEventListener('DOMContentLoaded', function () {
    const defaultSelectedItem = document.querySelector('.dashboard-item[data-content="pending-content"]');

    if (defaultSelectedItem) {
        defaultSelectedItem.classList.add('selected');
        loadPendingExpenses();
        toggleButtonsBasedOnTab('pending-content');
    }


    const dashboardItems = document.querySelectorAll('.dashboard-item');

    dashboardItems.forEach(item => {
        item.addEventListener('click', function () {
            dashboardItems.forEach(i => i.classList.remove('selected'));
            this.classList.add('selected');

            const activeTab = this.dataset.content; 
            toggleButtonsBasedOnTab(activeTab);

            if (this.dataset.content === "pending-content") {
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

    function loadPendingExpenses() {
        $.ajax({
            url: '/Manager/LoadManagerPendingExpenses',
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

    function loadRejectedExpenses() {
        $.ajax({
            url: '/Manager/LoadManagerRejectedExpenses',
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

    function loadAcceptedExpenses() {
        $.ajax({
            url: '/Manager/LoadManagerAcceptedExpenses',
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

    function loadApprovalProgressExpenses() {
        $.ajax({
            url: '/Manager/LoadApprovalProgressExpenses',
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

    function toggleButtonsBasedOnTab(activeTab) {
        if (activeTab === 'pending-content') {
            document.getElementById('approveExpenseBtn').style.display = 'flex';
            document.getElementById('rejectExpenseBtn').style.display = 'flex';
        } else {
            document.getElementById('approveExpenseBtn').style.display = 'none';
            document.getElementById('rejectExpenseBtn').style.display = 'none';
        }
    }

    attachEventListeners();
});
