let currencySymbol = "$";
let expenses = [];
let trackedChanges = [];
let globalFormID;

// Functions
function formatDate(dateString) {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

function updateExpenseTable() {
    const billBody = document.getElementById('billBody');
    billBody.innerHTML = '';

    document.getElementById('editExpenseColumn').style.display = 'table-cell';

    let total = 0;


    expenses.forEach((expense, index) => {
        const newRow = billBody.insertRow();
        newRow.setAttribute('data-expense-id', expense.expenseID);
        newRow.innerHTML = `
          <td>${index + 1}</td>
          <td>${expense.currency}</td>
          <td>${expense.description}</td>
          <td>${expense.expenseDate}</td>
          <td>${expense.amount.toFixed(2)}</td>
          <td><button class="edit-expense-btn"><i class="fa-solid fa-pencil-alt"></i></button></td>
          <td><button class="delete-expense-btn"><i class="fa-solid fa-trash-can"></i></button></td>
        `;

        total += expense.amount;
        newRow.querySelector('.delete-expense-btn').onclick = function () {
            const deletedExpense = expenses[index];
            trackedChanges.push({ ExpenseFormID: globalFormID, action: 'deleted', data: deletedExpense });
            console.log(trackedChanges);
            expenses.splice(index, 1);
            updateExpenseTable();
        };
    });

    document.getElementById('totalAmount').innerText = total.toFixed(2);
}

function updateExpenseViewTable() {
    const billBody = document.getElementById('billBody');
    billBody.innerHTML = '';

    document.getElementById('editExpenseColumn').style.display = 'none';

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

function attachEventListeners() {
    document.getElementById('currencySelect').onchange = function () {
        currencySymbol = this.value;
        expenses.forEach(expense => {
            expense.currency = currencySymbol;
        });
        updateExpenseTable();
    };

    document.getElementById('openAddItemFormBtn').onclick = function () {
        document.getElementById('addExpenseForm').reset();
        document.getElementById('editExpenseBtn').style.display = 'none';
        document.getElementById('addItemBtn').style.display = 'flex';
        document.getElementById('addExpenseItemForm').style.display = 'flex';
        document.getElementById('expenseTableForm').style.display = 'flex';
    };

    document.getElementById('addItemBtn').onclick = function () {
        const expenseDate = document.getElementById('itemDate').value;
        const description = document.getElementById('itemDescription').value;
        const amount = parseFloat(document.getElementById('itemAmount').value);

        document.getElementById('itemDescription').classList.remove('error');
        document.getElementById('itemAmount').classList.remove('error');
        document.getElementById('itemDate').classList.remove('error');

        document.getElementById('editExpenseBtn').style.display = 'none';

        if (!description || !amount || !expenseDate) {
            if (!description) document.getElementById('itemDescription').classList.add('error');
            if (!amount) document.getElementById('itemAmount').classList.add('error');
            if (!expenseDate) document.getElementById('itemDate').classList.add('error');
            showToast("Please fill out all fields before adding an item.", "error");
            return;
        }

        if (amount <= 0 || amount > 5000) {
            document.getElementById('itemAmount').classList.add('error');
            showToast("Amount should be greater than 0 and less than or equal to 5000.", "error");
            return;
        }

        const newExpense = {
            id: expenses.length + 1,
            currency: currencySymbol,
            description: description,
            expenseDate: formatDate(expenseDate),
            amount: amount
        };

        expenses.push(newExpense);

        updateExpenseTable();
        document.getElementById('addExpenseForm').reset();
        document.getElementById('addExpenseItemForm').style.display = 'none';
        trackedChanges.push({ ExpenseFormID: globalFormID, action: 'added', data: newExpense });
        showToast("Item added successfully!", "success");
    };

    document.addEventListener('click', function (event) {
        if (event.target && event.target.closest('.edit-expense-btn')) {
            const row = event.target.closest('tr');
            const rowId = row.getAttribute('data-form-id');
            const expense = expenses.find(e => e.expenseID == rowId);

            if (expense) {
                document.getElementById('addExpenseItemForm').style.display = 'flex';
                document.getElementById('itemDate').value = expense.expenseDate;
                document.getElementById('itemDescription').value = expense.description;
                document.getElementById('itemAmount').value = expense.amount;

                document.getElementById('addItemBtn').style.display = 'none';
                document.getElementById('editExpenseBtn').style.display = 'flex';

                document.getElementById('addExpenseForm').setAttribute('data-expense-id', rowId);
            }
        }
    });

    document.addEventListener('click', function (event) {
        if (event.target && (event.target.id === 'editExpenseBtn')) {
            const rowId = document.getElementById('addExpenseForm').getAttribute('data-form-id');
            const updatedDate = document.getElementById('itemDate').value;
            const updatedDescription = document.getElementById('itemDescription').value;
            const updatedAmount = parseFloat(document.getElementById('itemAmount').value);


            const updatedExpense = {
                expenseID: rowId,
                currency: currencySymbol,
                description: updatedDescription,
                expenseDate: updatedDate,
                amount: updatedAmount
            };

            const rowIndex = expenses.findIndex(expense => expense.expenseID == rowId);
            if (rowIndex !== -1) {
                expenses[rowIndex] = updatedExpense;
                trackedChanges.push({ ExpenseFormID: globalFormID, action: 'updated', data: updatedExpense });
                console.log(trackedChanges);
            }

            updateExpenseTable();

            document.getElementById('addExpenseItemForm').style.display = 'none';
            document.getElementById('addExpenseForm').reset();
            document.getElementById('addItemBtn').style.display = 'flex';
            document.getElementById('editExpenseBtn').style.display = 'none';
        }
    });


    document.querySelector('.close').addEventListener('click', function () {
        document.getElementById('addExpenseModal').style.display = 'none';
        document.getElementById('addExpenseForm').reset();
        document.getElementById('addItemBtn').style.display = 'flex';
        document.getElementById('updateItemBtn').style.display = 'none';
    });

    document.getElementById('submitExpenseBtn').onclick = function (e) {
        e.preventDefault();

        if (expenses.length === 0) {
            alert("Please add at least one expense item before submitting.");
            return;
        }

        const formData = {
            Expenses: JSON.stringify(expenses),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            url: '/ExpenseForms/SubmitExpense',
            type: 'POST',
            data: formData,
            contentType: 'application/x-www-form-urlencoded',
            success: function (response) {
                if (response.success) {
                    showToast(response.message, "success");
                    expenses = [];
                    updateExpenseTable();
                    window.location.href = '/Employee/Dashboard';
                } else {
                    showToast(response.message, "error");
                }
            },
            error: function (xhr, status, error) {
                showToast("An error occurred while submitting the form. Please try again.", 'error');
                window.location.href = '/Employee/Dashboard';
            }
        });
    };

    document.getElementById('resubmitExpenseBtn').onclick = function (e) {
        e.preventDefault();

        if (trackedChanges.length === 0) {
            showToast("No changes have been tracked to submit.",'error');
            return;
        }

        const formData = {
            TrackedChanges: JSON.stringify(trackedChanges),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };


        $.ajax({
            url: '/Employee/EditExpensesAndExpenseForm',
            type: 'POST',
            data: formData,
            contentType: 'application/x-www-form-urlencoded',
            success: function (response) {
                if (response.success) {
                    showToast(response.message, "success");
                    trackedChanges = [];
                    window.location.href = '/Employee/Dashboard';
                } else {
                    showToast(response.message, "error");
                }
            },
            error: function (xhr, status, error) {
                showToast("An error occurred while submitting the form. Please try again.", 'error');
                window.location.href = '/Employee/Dashboard';
            }
        });
    };

    document.addEventListener('click', function (event) {
        if (event.target && event.target.id === 'specificExpenseViewer') {
            const button = event.target;
            const formId = button.getAttribute('data-form-id');
            const expenseForm = expenseFormsData.find(form => form.FormID == formId);

            document.getElementById('addExpenseModal').style.display = 'flex';
            document.getElementById('expenseTableForm').style.display = 'flex';

            document.querySelector('.expense-table-currency-select').style.display = 'none';
            document.getElementById('openAddItemFormBtn').style.display = 'none';
            document.getElementById('addExpenseItemForm').style.display = 'none';
            document.getElementById('submitExpenseBtn').style.display = 'none';
            document.getElementById('cancelExpenseBtn').style.display = 'none';

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
    });

    document.addEventListener('click', function (event) {
        if (event.target && event.target.id === 'specificEditExpenseViewer') {
            const button = event.target;
            const formId = button.getAttribute('data-form-id');
            globalFormID = formId;
            const expenseForm = expenseFormsData.find(form => form.FormID == formId);

            document.getElementById('addExpenseModal').style.display = 'flex';
            document.getElementById('expenseTableForm').style.display = 'flex';

            document.querySelector('.expense-table-currency-select').style.display = 'flex';
            document.getElementById('openAddItemFormBtn').style.display = 'flex';
            document.getElementById('addExpenseItemForm').style.display = 'none';
            document.getElementById('submitExpenseBtn').style.display = 'none';
            document.getElementById('cancelExpenseBtn').style.display = 'flex';
            document.getElementById('resubmitExpenseBtn').style.display = 'flex';

            if (expenseForm.ManagerComments) {
                document.getElementById('managerComments').innerHTML = `<p>${expenseForm.ManagerComments}</p>`;
            } else {
                document.getElementById('managerComments').innerHTML = '<p>No comments available</p>';
            }

            const relevantExpenses = expenseForm.Expenses;

            expenses = relevantExpenses.map((expense, index) => {
                const timestamp = parseInt(expense.Date.match(/\/Date\((\d+)\)\//)[1], 10);
                const formattedDate = formatDate(new Date(timestamp));

                return {
                    id: index + 1,
                    currency: expense.Currency,
                    expenseID: expense.ExpenseID,
                    description: expense.Description,
                    expenseDate: formattedDate,
                    amount: expense.Amount
                };
            });

            updateExpenseTable();
        }
    });

    document.getElementById('closeAddExpenseForm').onclick = function () {
        document.getElementById('addExpenseItemForm').style.display = 'none'; 
        document.getElementById('addExpenseForm').reset();
    };


    document.getElementById('cancelExpenseBtn').onclick = function () {
        expenses = [];
        updateExpenseTable();
        document.getElementById('expenseTableForm').style.display = 'none';
        document.getElementById('addExpenseItemForm').style.display = 'none';
        document.getElementById('addExpenseModal').style.display = 'none';
        location.reload();
    };
}
document.addEventListener('DOMContentLoaded', function () {
    attachEventListeners(); 
});