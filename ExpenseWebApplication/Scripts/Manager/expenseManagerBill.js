let currencySymbol = "$";
let expenses = [];
let currentFormID = null;
let isUpdating = false;
// Functions
function formatDate(dateString) {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

function updateExpenseViewTable() {
    const billBody = document.getElementById('billBody');
    billBody.innerHTML = '';

    let total = 0;

    expenses.forEach((expense, index) => {
        const newRow = billBody.insertRow();
        newRow.innerHTML = `
          <td>${index + 1}</td>
          <td>${expense.currency}</td>
          <td>${expense.description}</td>
          <td>${expense.expenseDate}</td>
          <td>${expense.amount.toFixed(2)}</td>
          <td><button class="delete-expense-btn" disabled>Delete</button></td>
        `;

        total += expense.amount;
    });

    document.getElementById('totalAmount').innerText = total.toFixed(2);
}

function updateExpenseStatus(formId, action, comments = null) {
    const data = JSON.stringify({ formId: formId, action: action, comments: comments });

    const formData = {
        data: data,
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    }
    $.ajax({
        url: '/Manager/UpdateExpenseStatus',
        type: 'POST',
        data: formData,
        contentType: 'application/x-www-form-urlencoded',
        success: function (response) {
            alert(response.message);
            closeExpenseModal();
            location.reload();
            isUpdating = false;
        },
        error: function (xhr) {
            alert('Error: ' + xhr.responseText);
        }
    });
}

function closeExpenseModal() {
    document.getElementById('expenseTableForm').style.display = 'none';
    document.getElementById('addExpenseModal').style.display = 'none';
}

// Event Listeners
function attachEventListeners() {
    document.addEventListener('click', function (event) {
        if (event.target && event.target.classList.contains('expense-table-row-view-btn')) {
            const button = event.target;
            const formId = button.getAttribute('data-form-id'); 
            const expenseForm = expenseFormsData.find(form => form.FormID == formId);
            currentFormID = expenseForm.FormID;

            document.getElementById('addExpenseModal').style.display = 'flex';
            document.getElementById('expenseTableForm').style.display = 'flex';

            document.querySelector('.expense-table-currency-select').style.display = 'none';
            document.getElementById('addManagerComments').style.display = 'none';
            document.getElementById('addManagerCommentsForm').style.display = 'none';

            const relevantExpenses = expenseForm.Expenses;

            expenses = relevantExpenses.map((expense, index) => {
                const timestamp = parseInt(expense.Date.match(/\/Date\((\d+)\)\//)[1], 10);
                const formattedDate = formatDate(new Date(timestamp));

                return {
                    id: index + 1,
                    currency: expense.Currency,
                    description: expense.Description,
                    expenseDate: formattedDate,
                    amount: expense.Amount
                };
            });

            updateExpenseViewTable();

        }
        else if (event.target && (event.target.classList.contains('close') || event.target.id === 'addExpenseModal')) {
            closeExpenseModal();
        }
        else if (event.target && event.target.id === 'approveExpenseBtn') {
            if (isUpdating)
                return;
            isUpdating = true;
            updateExpenseStatus(currentFormID, 'approve');
        }
        else if (event.target && event.target.id === 'rejectExpenseBtn') {
            document.getElementById('addManagerComments').style.display = 'flex';
            document.getElementById('addManagerCommentsForm').style.display = 'flex';
        }
        else if (event.target && event.target.id === 'submitManagerComments') {
            var comments = $('#itemComments').val();
            updateExpenseStatus(currentFormID, 'reject', comments);
        }
    });
}

document.addEventListener('DOMContentLoaded', attachEventListeners);