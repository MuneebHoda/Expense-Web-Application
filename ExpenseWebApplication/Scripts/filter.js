function initializeFilteringLogic() {
    const filterOptions = document.getElementById('filterOptions');
    const expenseTableBody = document.querySelector('.expense-table-body'); 

    filterOptions.addEventListener('change', function () {
        const selectedFilter = this.value;

        const rowsArray = Array.from(expenseTableBody.querySelectorAll('.expense-table-row'));

        if (selectedFilter === 'highestToLowest') {
            rowsArray.sort((a, b) => {
                const amountA = parseFloat(a.cells[5].innerText);
                const amountB = parseFloat(b.cells[5].innerText);
                return amountB - amountA;
            });
        } else if (selectedFilter === 'mostRecent') {
            rowsArray.sort((a, b) => {
                const dateA = new Date(a.cells[1].innerText);
                const dateB = new Date(b.cells[1].innerText);
                return dateB - dateA;
            });
        }

        expenseTableBody.innerHTML = '';
        rowsArray.forEach(row => expenseTableBody.appendChild(row));
    });
}
