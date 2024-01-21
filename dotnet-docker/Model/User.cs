namespace dotnet_docker.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string DocumentNumber { get; set; }
    }
}
