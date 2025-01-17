document.addEventListener('DOMContentLoaded', function () {
    const dashboardItems = document.querySelectorAll('.dashboard-item');

    function clearSelected() {
        dashboardItems.forEach(item => item.classList.remove('selected'));
    }

    dashboardItems.forEach(item => {
        item.addEventListener('click', function () {
            clearSelected();
            this.classList.add('selected');
        });
    });
});
