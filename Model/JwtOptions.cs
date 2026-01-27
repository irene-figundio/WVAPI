namespace AI_Integration.Model
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiryHours { get; set; } = 4;
    }

}
