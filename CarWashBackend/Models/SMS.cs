public class SmsRequest
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
}

public class BulkSmsRequest
{
    public List<string> PhoneNumbers { get; set; }
    public string Message { get; set; }
}