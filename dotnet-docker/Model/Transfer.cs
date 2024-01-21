namespace dotnet_docker.Model
{
    public class Transfer
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAdress { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public decimal Amount { get; set; }
        public int Time { get; set; }

    }
}
