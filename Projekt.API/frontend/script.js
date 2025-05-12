const API_BASE = "https://localhost:7267/api";

// CLIENT CRUD
const clientForm = document.getElementById("addClientForm");
const clientList = document.getElementById("clientList");

// Load Clients
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
                <button onclick="deleteMovie(${movie.id})">Delete</button>
                <button onclick="editMovie(${movie.id}, '${movie.name}', '${movie.type}', ${movie.rating}, ${movie.clientID})">Edytuj</button>
                `;
            movieList.appendChild(li);
        });
    } catch (error) {
        console.error("Failed to load movies", error);
    }
}

// Add Movie
movieForm.addEventListener("submit", async (e) => {
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
});

// Delete Movie
async function deleteMovie(id) {
    try {
        await fetch(`${API_BASE}/Movies/Delete?id=${id}`, { method: "DELETE" });
        loadMovies();
    } catch (error) {
        console.error("Failed to delete movie", error);
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

// Load initial data
loadClients();
loadMovies();
