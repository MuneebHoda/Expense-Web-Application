function countPendingExpenses(expenseForms) {
    return expenseForms.filter(expenseForm => expenseForm.Status === 'Pending').length;
}

function countAcceptedExpenses(expenseForms) {
    return expenseForms.filter(expenseForm => expenseForm.Status === 'Approved').length;
}

function countRejectedExpenses(expenseForms) {
    return expenseForms.filter(expenseForm => expenseForm.Status === 'Rejected').length;
}

function updateCardCounts() {
    const pendingCount = countPendingExpenses(expenseFormsData);
    const acceptedCount = countAcceptedExpenses(expenseFormsData);
    const rejectedCount = countRejectedExpenses(expenseFormsData);

    document.querySelector('.pending-card .card-bottom-left-corner p').textContent = pendingCount;
    document.querySelector('.accepted-card .card-bottom-left-corner p').textContent = acceptedCount;
    document.querySelector('.rejected-card .card-bottom-left-corner p').textContent = rejectedCount;
}


document.addEventListener('DOMContentLoaded', updateCardCounts);
