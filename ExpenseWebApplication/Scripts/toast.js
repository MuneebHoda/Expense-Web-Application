function showToast(message, type) {
    var toast = document.getElementById("toast");
    var toastMessage = document.getElementById("toastMessage");

    toastMessage.innerText = message;

    toast.className = `toast ${type}`;
    if (type === 'success') {
        toast.style.backgroundColor = '#28a745';
    } else if (type === 'error') {
        toast.style.backgroundColor = '#dc3545';
    }

    toast.style.display = 'flex'; 
    toast.classList.add("show"); 

    setTimeout(function () {
        toast.classList.remove("show");
        toast.style.display = 'none';
    }, 5000);
}
