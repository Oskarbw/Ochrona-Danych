namespace dotnet_docker.Model
{
    public class Password
    {
        public Guid Id { get; set; }
        public int Variant { get; set; }
        public string Username { get; set; }
        public string Pattern { get; set; }
        public string Hash { get; set; }
    }
}
