const API_BASE_URL = "https://localhost:7267/api";

class ApiService {
    constructor() {
        this.token = localStorage.getItem('authToken');
    }

    // Header do uwierzytelnienia dla wszystkich requestów
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json'
        };

        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        return headers;
    }

    // Login i rejestracja
    async login(email, password) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Auth/Login`, {
                email,
                password
            });

            this.token = response.data.token;
            localStorage.setItem('authToken', this.token);
            localStorage.setItem('userRole', response.data.role);
            localStorage.setItem('userEmail', response.data.email);
            if (response.data.clientId) {
                localStorage.setItem('clientId', response.data.clientId);
            }

            return response.data;
        } catch (error) {
            console.error("ApiService.login: Błąd podczas żądania", error); // Bardzo ważne!
            console.error("ApiService.login: Szczegóły błędu Axios:", error.response, error.request, error.message, error.config);
            throw error.response?.data || error.message;
        }
    }

    async register(email, password, name, lastname) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Auth/Register`, {
                email,
                password,
                name,
                lastname
            });

            this.token = response.data.token;
            localStorage.setItem('authToken', this.token);
            localStorage.setItem('userRole', response.data.role);
            localStorage.setItem('userEmail', response.data.email);
            if (response.data.clientId) {
                localStorage.setItem('clientId', response.data.clientId);
            }

            return response.data;
        } catch (error) {
            console.error("ApiService.register: Błąd podczas żądania", error); // Bardzo ważne!
            console.error("ApiService.register: Szczegóły błędu Axios:", error.response, error.request, error.message, error.config);
            throw error.response?.data || error.message;
        }
    }

    logout() {
        this.token = null;
        localStorage.clear();
        window.location.href = '/index.html';
    }

    // Filmy
    async getMovies() {
        try {
            const response = await axios.get(`${API_BASE_URL}/Movies/List`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async getMovie(id) {
        try {
            const response = await axios.get(`${API_BASE_URL}/Movies/ByID?id=${id}`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async addMovie(movie) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Movies/Add`, movie, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async updateMovie(movie) {
        try {
            const response = await axios.put(`${API_BASE_URL}/Movies/Update`, movie, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async deleteMovie(id) {
        try {
            const response = await axios.delete(`${API_BASE_URL}/Movies/Delete/${id}`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async uploadMovieCover(movieId, file) {
        try {
            const formData = new FormData();
            formData.append('file', file);

            const response = await axios.post(`${API_BASE_URL}/Movies/UploadCover/${movieId}`, formData, {
                headers: {
                    'Authorization': `Bearer ${this.token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    getMovieCoverUrl(movieId) {
        return `${API_BASE_URL}/Movies/DownloadCover/${movieId}`;
    }

    // Wypożyczenie
    async rentMovie(movieId) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Movies/Rent/${movieId}`, {}, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async returnMovie(movieId) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Movies/Return/${movieId}`, {}, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async rateMovie(movieId, rating, comment) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Movies/Rate/${movieId}`, {
                rating,
                comment
            }, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async getMyRentals() {
        console.log('getMyRentals w API'); //DEbug
        try {
            const response = await axios.get(`${API_BASE_URL}/Movies/MyRentals`, {
                headers: this.getHeaders()
            });
            console.log('Odpowiedź API MyRentals:', response.data);
            return response.data;
        } catch (error) {
            console.error('Błąd w myrentals:', error);
            throw error.response?.data || error.message;
        }
    }

    // Klienci
    async getClients() {
        try {
            const response = await axios.get(`${API_BASE_URL}/Clients/List`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async getClient(id) {
        try {
            const response = await axios.get(`${API_BASE_URL}/Clients/ByID?id=${id}`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async addClient(client) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Clients/Add`, client, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async updateClient(client) {
        try {
            const response = await axios.patch(`${API_BASE_URL}/Clients/Update`, client, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async deleteClient(id) {
        try {
            const response = await axios.delete(`${API_BASE_URL}/Clients/Delete?id=${id}`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async getMyProfile() {
        try {
            const response = await axios.get(`${API_BASE_URL}/Clients/MyProfile`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    // Użytkownicy
    async getCurrentUser() {
        try {
            const response = await axios.get(`${API_BASE_URL}/Auth/Me`, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    async changePassword(currentPassword, newPassword) {
        try {
            const response = await axios.post(`${API_BASE_URL}/Auth/ChangePassword`, {
                currentPassword,
                newPassword
            }, {
                headers: this.getHeaders()
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error.message;
        }
    }

    // Sprawdzanie czy użytkownik jest zalogowany
    isAuthenticated() {
        return !!this.token;
    }

    // Metoda do pozyskania roli
    getUserRole() {
        return localStorage.getItem('userRole');
    }

    // Metoda do sprawdzania czy user jest adminem
    isAdmin() {
        return this.getUserRole() === 'Administrator';
    }
}

// Tworzenie instancji
const api = new ApiService();

// Fallback gdy user jest niezalogowany
function requireAuth() {
    if (!api.isAuthenticated()) {
        window.location.href = '/index.html';
    }
}

// Dashboard zależnie od roli
function redirectToDashboard() {
    if (api.isAdmin()) {
        window.location.href = '/admin.html';
    } else {
        window.location.href = '/dashboard.html';
    }
}