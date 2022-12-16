namespace PWCExamService.Data
{
    public class TokenResponseEntity
    {
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public int TokenExpiresIn { get; set; }
    }
}
