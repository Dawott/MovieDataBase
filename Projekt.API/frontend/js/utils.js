function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Błąd',
        text: message
    });
}

function showSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: 'Sukces',
        text: message,
        timer: 2000,
        showConfirmButton: false
    });
}

function showLoading() {
    Swal.fire({
        title: 'Ładowanie...',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
}

function hideLoading() {
    Swal.close();
}

// Formatowanie daty
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('pl-PL', options);
}

// Odbij funkcję
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Walidacja formularza - email
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Helper dla lokalnego przechowania
function saveToLocalStorage(key, value) {
    try {
        localStorage.setItem(key, JSON.stringify(value));
    } catch (e) {
        console.error('Nie udało się zachować lokalnie', e);
    }
}

function getFromLocalStorage(key) {
    try {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    } catch (e) {
        console.error('Nie udało się załadować', e);
        return null;
    }
}

// Sprawdzanie sesji
function checkSession() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = '/index.html';
        return false;
    }

    // Sprawdzanie czy token się skończył
    const expiresAt = localStorage.getItem('tokenExpiresAt');
    if (expiresAt && new Date(expiresAt) < new Date()) {
        localStorage.clear();
        window.location.href = '/index.html';
        return false;
    }

    return true;
}

// Fallback obrazka
function handleImageError(img) {
    img.onerror = null;
    img.src = '/api/placeholder/300/400';
}

// Export funkcji
window.utils = {
    showError,
    showSuccess,
    showLoading,
    hideLoading,
    formatDate,
    debounce,
    validateEmail,
    saveToLocalStorage,
    getFromLocalStorage,
    checkSession,
    handleImageError
};