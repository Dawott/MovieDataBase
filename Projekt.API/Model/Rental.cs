namespace Projekt.API.Model
{
    //Model do reprezentacji Wypożyczeń - tutaj zmieniam relację - z Klient > Film na Klient > Wypożyczenie > Film
    public class Rental
    {
        public int ID { get; set; }

        public int ClientID { get; set; }
        public int MovieID { get; set; }

        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Relacje/nawigacja od klienta i filmu
        public virtual Client Client { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
