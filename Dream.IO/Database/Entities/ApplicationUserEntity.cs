namespace Dream.IO.Database.Entities
{
    public class ApplicationUserEntity
    {
        public int ApplicationUserId { get; set; }
        public string NetworkUserNameIdentifier { get; set; }
        public string ApplicationDisplayableNickName { get; set; }
        public bool IsReadOnlyUser { get; set; }
    }
}
