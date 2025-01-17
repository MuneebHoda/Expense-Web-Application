document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('addExpenseModal');
    const closeModal = document.getElementsByClassName('close')[0];

    function openAddExpenseModal() {
        expenses = [];
        updateExpenseTable();

        document.getElementById('expenseTableForm').style.display = 'flex';
        document.getElementById('addExpenseItemForm').style.display = 'none';
        modal.style.display = 'flex';
    }

    document.addEventListener('click', function (event) {
        if (event.target && event.target.id === 'addExpenseBtn') {
            openAddExpenseModal();
        } else if (event.target === closeModal) {
            closeAddExpenseModal();
        } else if (event.target === modal) {
            closeAddExpenseModal();
        }
    });

    function closeAddExpenseModal() {
        document.getElementById('expenseTableForm').style.display = 'none';
        document.getElementById('addExpenseItemForm').style.display = 'none';
        modal.style.display = 'none';
        location.reload();
    }
});
