namespace LibraryManagement.Api.Settings
{
    public class JwtSetting
    {
        public string Key   { get; set; } = null!;
        public string validIssuer { get; set; } = null!;
        public string ValidAudiences { get; set; } = null!;
        public int expires { get; set; } 
    }
}
