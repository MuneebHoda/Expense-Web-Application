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
            else if (this.dataset.content === "approved-content") {
                loadAcceptedExpenses()
            }
        });
    });

    function loadPendingExpenses() {
        $.ajax({
            url: '/Accountant/LoadAccountantPendingExpenses',
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
            url: '/Accountant/LoadAccountantAcceptedExpenses',
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
            document.getElementById('payExpenseBtn').style.display = 'flex';
        } else {
            document.getElementById('payExpenseBtn').style.display = 'none';
        }
    }

    attachEventListeners();
});
