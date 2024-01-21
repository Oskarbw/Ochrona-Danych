using System.Net.WebSockets;

namespace dotnet_docker.Model
{
    public class Attempt
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Ip { get; set; }
        public int Time { get; set; }
        public bool IsLogin {  get; set; }
    }
}
