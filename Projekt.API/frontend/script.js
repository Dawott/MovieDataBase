const API_BASE = "https://localhost:5341/api";
let authToken = null;
let currentUser = null;
let currentMovieId = null;

//Autoryzacja
document.addEventListener('DOMContentLoaded', () => {
    checkAuth();
    setupEventListeners();
});

//Sprawdzenie czy user jest zalogowany
function checkAuth() {
    const token = localStorage.getItem('authToken');
    if (token) {
        authToken = token;
        loadUserProfile();
    } else {
        showAuthSection();
    }
}

//------Funkcje zwi¹zane z uwierzytelnieniem-----
function setupEventListeners() {
    document.getElementById('loginForm').addEventListener('submit', handleLogin);
    document.getElementById('registerForm').addEventListener('submit', handleRegister);
}

function showAuthSection() {
    document.getElementById('authSection').style.display = 'block';
    document.getElementById('mainApp').style.display = 'none';
}

function showMainApp() {
    document.getElementById('authSection').style.display = 'none';
    document.getElementById('mainApp').style.display = 'block';

    if (currentUser.role === 'Administrator') {
        document.getElementById('adminSection').style.display = 'block';
        document.getElementById('clientSection').style.display = 'none';
        loadAdminDashboard();
    } else {
        document.getElementById('adminSection').style.display = 'none';
        document.getElementById('clientSection').style.display = 'block';
        loadClientDashboard();
    }
}

function showLogin() {
    document.getElementById('loginForm').style.display = 'block';
    document.getElementById('registerForm').style.display = 'none';
    document.querySelectorAll('.tab-btn')[0].classList.add('active');
    document.querySelectorAll('.tab-btn')[1].classList.remove('active');
}

function showRegister() {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('registerForm').style.display = 'block';
    document.querySelectorAll('.tab-btn')[0].classList.remove('active');
    document.querySelectorAll('.tab-btn')[1].classList.add('active');
}

//Login
async function handleLogin(e) {
    e.preventDefault();
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (response.ok) {
            const data = await response.json();
            authToken = data.token;
            localStorage.setItem('authToken', authToken);
            loadUserProfile();
        } else {
            const error = await response.text();
            document.getElementById('loginError').textContent = error || 'B³¹d logowania';
        }
    } catch (error) {
        document.getElementById('loginError').textContent = 'B³¹d po³¹czenia';
    }
}

//Rejestrowanie
async function handleRegister(e) {
    e.preventDefault();
    const data = {
        email: document.getElementById('registerEmail').value,
        password: document.getElementById('registerPassword').value,
        name: document.getElementById('registerName').value,
        lastname: document.getElementById('registerLastname').value
    };

    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();
            authToken = result.token;
            localStorage.setItem('authToken', authToken);
            loadUserProfile();
        } else {
            const error = await response.text();
            document.getElementById('registerError').textContent = error || 'B³¹d rejestracji';
        }
    } catch (error) {
        document.getElementById('registerError').textContent = 'B³¹d po³¹czenia';
    }
}

async function loadUserProfile() {
    try {
        const response = await fetch(`${API_URL}/auth/me`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        if (response.ok) {
            currentUser = await response.json();
            document.getElementById('userEmail').textContent = currentUser.email;
            document.getElementById('userRole').textContent = currentUser.role;
            showMainApp();
        } else {
            logout();
        }
    } catch (error) {
        console.error('Error loading profile:', error);
        logout();
    }
}

function logout() {
    authToken = null;
    currentUser = null;
    localStorage.removeItem('authToken');
    showAuthSection();
}

// £adowanie widoku admina
function loadAdminDashboard() {
    loadAllMovies();
    loadAllClients();
}

function showMovieManagement() {
    document.getElementById('movieManagement').style.display = 'block';
    document.getElementById('clientList').style.display = 'none';
}

function showClientList() {
    document.getElementById('movieManagement').style.display = 'none';
    document.getElementById('clientList').style.display = 'block';
}

// CLIENT CRUD - poprzednie funkcje
const clientForm = document.getElementById("addClientForm");
const clientList = document.getElementById("clientList");

// Load Clients
/*
async function loadClients() {
    clientList.innerHTML = "";
    try {
        const response = await fetch(`${API_BASE}/Clients/List`);
        const clients = await response.json();
        clients.forEach(client => {
            const li = document.createElement("li");
            li.innerHTML = `
                ${client.name} ${client.lastname} 
                <button onclick="deleteClient(${client.id})">Delete</button>
                <button onclick="editClient(${client.id}, '${client.name}', '${client.lastname}')">Edytuj</button>
            `;
            clientList.appendChild(li);
        });
    } catch (error) {
        console.error("Failed to load clients", error);
    }
}*/
async function loadAllClients() {
    try {
        const response = await fetch(`${API_URL}/clients/list`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const clients = await response.json();

        const clientList = document.getElementById('clientListContent');
        clientList.innerHTML = `
            <table class="client-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Imiê i nazwisko</th>
                        <th>Email</th>
                        <th>Wypo¿yczone</th>
                    </tr>
                </thead>
                <tbody>
                    ${clients.map(client => `
                        <tr>
                            <td>${client.id}</td>
                            <td>${client.name} ${client.lastname}</td>
                            <td>${client.user?.email || 'N/A'}</td>
                            <td>${client.rentals?.length || 0}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        console.error('B³¹d ³adowania klientów:', error);
    }
}

//£adowanie dashboardu dla klienta
function loadClientDashboard() {
    showAvailableMovies();
}

function showAvailableMovies() {
    document.getElementById('availableMovies').style.display = 'block';
    document.getElementById('myRentals').style.display = 'none';
    document.getElementById('myProfile').style.display = 'none';
    loadMoviesForClient();
}
function showMyRentals() {
    document.getElementById('availableMovies').style.display = 'none';
    document.getElementById('myRentals').style.display = 'block';
    document.getElementById('myProfile').style.display = 'none';
    loadMyRentals();
}

function showMyProfile() {
    document.getElementById('availableMovies').style.display = 'none';
    document.getElementById('myRentals').style.display = 'none';
    document.getElementById('myProfile').style.display = 'block';
    loadMyProfile();
}

//£adowanie profilu filmu dla klienta
async function loadMoviesForClient() {
    try {
        const response = await fetch(`${API_URL}/movies/list`);
        const movies = await response.json();

        const movieGrid = document.getElementById('movieGrid');
        movieGrid.innerHTML = '';

        movies.forEach(movie => {
            const movieCard = document.createElement('div');
            movieCard.className = 'movie-card';
            movieCard.innerHTML = `
                <img src="${movie.coverImagePath ?
                `${API_URL}/Movies/UploadCover/${movie.id}` :
                    '/api/placeholder/300/200'}" alt="${movie.name}">
                <div class="movie-info">
                    <div class="movie-title">${movie.name}</div>
                    <div class="movie-genre">${movie.type}</div>
                    <div class="movie-rating">${movie.rating || 'Nieocenione'}</div>
                    <div class="movie-actions">
                        <button class="rent-btn" onclick="rentMovie(${movie.id})">Rent</button>
                    </div>
                </div>
            `;
            movieGrid.appendChild(movieCard);
        });
    } catch (error) {
        console.error('Error loading movies:', error);
    }
}

async function loadMyRentals() {
    try {
        const response = await fetch(`${API_URL}/Movies/MyRentals`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const rentals = await response.json();

        const rentalList = document.getElementById('rentalList');
        rentalList.innerHTML = '';

        rentals.forEach(rental => {
            const rentalItem = document.createElement('div');
            rentalItem.className = `rental-item ${rental.isActive ? 'rental-active' : 'rental-returned'}`;
            rentalItem.innerHTML = `
                <div>
                    <strong>${rental.movieName}</strong> - ${rental.movieType}
                    <div>Rented: ${new Date(rental.rentalDate).toLocaleDateString()}</div>
                    ${rental.returnDate ? `<div>Returned: ${new Date(rental.returnDate).toLocaleDateString()}</div>` : ''}
                </div>
                <div class="rental-actions">
                    ${rental.isActive ?
                    `<button class="return-btn" onclick="returnMovie(${rental.movieId})">Zwróæ</button>` :
                    `<button class="rate-btn" onclick="openRatingModal(${rental.movieId}, '${rental.movieName}')">Oceñ</button>`
                }
                </div>
            `;
            rentalList.appendChild(rentalItem);
        });
    } catch (error) {
        console.error('B³¹d ³adowania wypo¿yczeñ:', error);
    }
}

//Poka¿ mój profil
async function loadMyProfile() {
    try {
        const response = await fetch(`${API_URL}/Clients/MyProfile`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const profile = await response.json();

        const profileInfo = document.getElementById('profileInfo');
        profileInfo.innerHTML = `
            <div class="profile-card">
                <h3>${profile.name} ${profile.lastname}</h3>
                <p>Email: ${profile.user.email}</p>
                <p>Cz³onkostwo od: ${new Date(profile.user.createdAt).toLocaleDateString()}</p>
                <p>Wszystkie wypo¿yczenia: ${profile.rentals?.length || 0}</p>
                <p>Wszystkie oceny: ${profile.ratings?.length || 0}</p>
            </div>
        `;
    } catch (error) {
        console.error('B³¹d ³adowania profilu:', error);
    }
}

//Po¿ycz film
async function rentMovie(movieId) {
    try {
        const response = await fetch(`${API_URL}/Movies/Rent/${movieId}`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        if (response.ok) {
            alert('Film wypo¿yczony!');
            loadMoviesForClient();
        } else {
            const error = await response.text();
            alert(error || 'Nie uda³o siê wypo¿yczyæ filmu');
        }
    } catch (error) {
        console.error('B³¹d przy próbie wypo¿yczenia:', error);
        alert('B³¹d po³¹czenia');
    }
}

//Mo¿na zwróciæ (np. bo nie chcemy jednak obejrzeæ)
async function returnMovie(movieId) {
    try {
        const response = await fetch(`${API_URL}/Movies/Return/${movieId}`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        if (response.ok) {
            alert('Film zwrócony!');
            loadMyRentals();
        } else {
            const error = await response.text();
            alert(error || 'Nie uda³o siê zwróciæ filmu');
        }
    } catch (error) {
        console.error('B³¹d przy próbie zwrócenia:', error);
        alert('B³¹d po³¹czenia');
    }
}
//Modale
function openRatingModal(movieId, movieName) {
    currentMovieId = movieId;
    document.getElementById('ratingMovieName').textContent = movieName;
    document.getElementById('ratingModal').style.display = 'flex';
}

function closeRatingModal() {
    document.getElementById('ratingModal').style.display = 'none';
    currentMovieId = null;
}

// Add Client
clientForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = document.getElementById("clientName").value;
    const lastname = document.getElementById("clientLastname").value;

    try {
        await fetch(`${API_BASE}/Clients/Add`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, lastname })
        });
        loadClients();
    } catch (error) {
        console.error("Failed to add client", error);
    }
});

// Delete Client
async function deleteClient(id) {
    try {
        await fetch(`${API_BASE}/Clients/Delete?id=${id}`, { method: "DELETE" });
        loadClients();
    } catch (error) {
        console.error("Failed to delete client", error);
    }
}

// MOVIE CRUD
const movieForm = document.getElementById("addMovieForm");
const movieList = document.getElementById("movieList");

// Load Movies
/*
async function loadMovies() {
    movieList.innerHTML = "";
    try {
        const response = await fetch(`${API_BASE}/Movies/List`);
        const movies = await response.json();
        movies.forEach(movie => {
            const li = document.createElement("li");
            li.innerHTML = `
                ${movie.name} (${movie.type}) - Rating: ${movie.rating}
                ${movie.coverImagePath ?
                    `<img src="https://localhost:7267/Uploads/Covers/${movie.coverImagePath}" style="height:50px; margin-right:10px;">
                <button onclick="deleteCover(${movie.id})">Usun okladke</button>`
                    : ''}
                <button onclick="deleteMovie(${movie.id})">Skasuj</button>
                <button onclick="editMovie(${movie.id}, '${movie.name}', '${movie.type}', ${movie.rating}, ${movie.clientID})">Edytuj</button>
                `;
            movieList.appendChild(li);
        });
    } catch (error) {
        console.error("Failed to load movies", error);
    }
}*/
// NOWA FUNKCJA do filmów - skomentowaæ 
async function loadAllMovies() {
    try {
        const response = await fetch(`${API_URL}/Movies/List`);
        const movies = await response.json();

        const movieList = document.getElementById('adminMovieList');
        movieList.innerHTML = '<h3>Wszystkie filmy</h3>';

        movies.forEach(movie => {
            const movieDiv = document.createElement('div');
            movieDiv.className = 'movie-admin-item';
            movieDiv.innerHTML = `
                <div>
                    <strong>${movie.name}</strong> - ${movie.type}
                    <span class="movie-rating">${movie.rating}</span>
                </div>
                <button onclick="deleteMovie(${movie.id})" class="delete-btn">Skasuj</button>
            `;
            movieList.appendChild(movieDiv);
        });
    } catch (error) {
        console.error('B³¹d ³adowania filmów:', error);
    }
}

// Add Movie
/*movieForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = document.getElementById("movieName").value;
    const type = document.getElementById("movieType").value;
    const rating = parseFloat(document.getElementById("movieRating").value);
    const clientID = parseInt(document.getElementById("movieClientID").value);

    try {
        await fetch(`${API_BASE}/Movies/Add`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, type, rating, clientID })
        });
        loadMovies();
    } catch (error) {
        console.error("Failed to add movie", error);
    }
});*/
document.getElementById('addMovieForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();

    const movie = {
        name: document.getElementById('movieName').value,
        type: document.getElementById('movieType').value,
        rating: 0,
        isAvailable: true
    };

    try {
        const response = await fetch(`${API_URL}/Movies/Add`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(movie)
        });

        if (response.ok) {
            alert('Film dodany!');
            document.getElementById('addMovieForm').reset();
            loadAllMovies();
        } else {
            const error = await response.text();
            alert(error || 'Nie uda³o siê dodaæ filmu');
        }
    } catch (error) {
        console.error('B³¹d dodawania filmu:', error);
        alert('B³¹d po³¹czenia');
    }
});

// Delete Movie
async function deleteMovie(movieId) {
    if (!confirm('Na pewno chcesz usun¹æ ten film?')) return;

    try {
        const response = await fetch(`${API_BASE}/Movies/Delete?id=${movieId}`, { method: "DELETE", headers: { 'Authorization': `Bearer ${authToken}` } });
        if (response.ok) {
            alert('Film skasowany!');
            loadAllMovies();
        } else {
            const error = await response.text();
            alert(error || 'Nie uda³o siê usun¹æ filmu');
        }
    } catch (error) {
        console.error("Nie uda³o siê usu¹æ filmu", error);
    }
}

// Edycja klienta
async function editClient(id, currentName, currentLastname) {
    const newName = prompt("Nowe imiê:", currentName);
    const newLastname = prompt("Nowe nazwisko:", currentLastname);

    if (newName && newLastname) {
        try {
            await fetch(`${API_BASE}/Clients/Update`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    id: id,
                    name: newName,
                    lastname: newLastname
                })
            });
            loadClients();
        } catch (error) {
            console.error("B³¹d podczas edycji klienta:", error);
        }
    }
}

// Edycja filmu
async function editMovie(id, currentName, currentType, currentRating, currentClientID) {
    const newName = prompt("Nowa nazwa filmu:", currentName);
    const newType = prompt("Nowy gatunek:", currentType);
    const newRating = prompt("Nowa ocena (0-10):", currentRating);
    const newClientID = prompt("Nowy ID klienta:", currentClientID);

    if (newName && newType && newRating && newClientID) {
        try {
            await fetch(`${API_BASE}/Movies/Update`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    id: id,
                    name: newName,
                    type: newType,
                    rating: parseFloat(newRating),
                    clientID: parseInt(newClientID)
                })
            });
            loadMovies();
        } catch (error) {
            console.error("B³¹d podczas edycji filmu:", error);
        }
    }
}

// Upload cover
document.getElementById('uploadCoverForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const file = document.getElementById('coverFile').files[0];
    const movieId = document.getElementById('movieIdForCover').value;

    const formData = new FormData();
    formData.append('file', file);

    try {
        const response = await fetch(`${API_BASE}/Movies/UploadCover/${movieId}`, {
            method: 'POST',
            body: formData
        });
        if (response.ok) {
            const result = await response.json();
            alert(`Upload successful: ${result.fileName}`);
            loadMovies(); // odœwie¿ listê filmów
        } else {
            const error = await response.text();
            alert("Upload failed: " + error);
        }
    } catch (error) {
        console.error('Upload failed', error);
    }
});

async function deleteCover(movieId) {
    if (!confirm("Czy na pewno chcesz usunac okladke tego filmu?")) return;
    try {
        const response = await fetch(`${API_BASE}/Movies/DeleteCover/${movieId}`, {
            method: "DELETE"
        });
        if (response.ok) {
            alert("Okladka usunieta!");
            loadMovies();
        } else {
            const error = await response.text();
            alert("B³¹d: " + error);
        }
    } catch (error) {
        alert("Blad polaczenia!");
    }
}
//Dodaj ocenê
async function submitRating() {
    const rating = document.getElementById('ratingValue').value;
    const comment = document.getElementById('ratingComment').value;

    try {
        const response = await fetch(`${API_URL}/Movies/Rate/${currentMovieId}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ rating: parseFloat(rating), comment })
        });

        if (response.ok) {
            alert('Ocena przes³ana!');
            closeRatingModal();
            loadMyRentals();
        } else {
            const error = await response.text();
            alert(error || 'Nie uda³o siê oceniæ filmu');
        }
    } catch (error) {
        console.error('B³¹d przy ocenie:', error);
        alert('B³¹d po³¹czenia');
    }
}

// Load initial data
loadClients();
loadMovies();