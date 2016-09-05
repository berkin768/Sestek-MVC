namespace PhoneDex.Models
{
    public class ContactDto
    {
        public int Id { get; set; }
        public string sn { get; set; }
        public string title { get; set; }
        public string givenName { get; set; }
        public string physicalDeliveryOfficeName { get; set; }
        public string mail { get; set; }
        public string mobile { get; set; }
        public long? telephoneNumber { get; set; }

        public bool IsNewUser()
        {
            return Id <= 0;
        }
    }
}