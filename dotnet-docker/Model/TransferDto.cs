namespace dotnet_docker.Model
{
    public class TransferDto
    {
        public string Title { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAdress { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
