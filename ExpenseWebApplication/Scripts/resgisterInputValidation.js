document.getElementById('email').addEventListener('input', function () {
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    let email = document.getElementById('email').value;
    let emailError = document.getElementById('emailError');

    if (!emailPattern.test(email)) {
        emailError.textContent = 'Invalid email format';
    } else {
        emailError.textContent = '';
    }
});

document.getElementById('password').addEventListener('input', function () {
    let password = document.getElementById('password').value;
    let passwordLengthMessage = document.getElementById('passwordLengthMessage');

    if (password.length >= 8) {
        passwordLengthMessage.style.display = 'none';
    } else {
        passwordLengthMessage.style.display = 'block';
    }
});

document.getElementById('confirmPassword').addEventListener('input', function () {
    let password = document.getElementById('password').value;
    let confirmPassword = this.value;
    let passwordError = document.getElementById('passwordError');

    if (password !== confirmPassword) {
        passwordError.textContent = 'Passwords do not match';
    } else {
        passwordError.textContent = '';
    }
});

document.getElementById('username').addEventListener('input', function () {
    let username = this.value;
    let usernameError = document.getElementById('usernameError');

    if (username.length < 3) {
        usernameError.textContent = 'Username must be at least 3 characters long';
    } else {
        usernameError.textContent = '';
    }
});