using System.Diagnostics.Contracts;

namespace dotnet_docker.Model
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string PasswordFragment { get; set; }
        public int Variant { get; set; }
    }
}
