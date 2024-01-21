namespace dotnet_docker.Model
{
    public class TransferGetDto
    {
        public string Title { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAdress { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string SenderName { get; set; }
        public decimal Amount { get; set; }
        public int Time { get; set; }
    }
}
