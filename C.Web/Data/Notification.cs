using C.Web;

public enum NotificationLevel {
    Success, Info, Warning, Error
}

public class Notification {
    public DateTime Created {get; private set;}
    public NotificationLevel Level {get; private set;}
    public string Message {get; private set;}

    public Notification(NotificationLevel level, string mesg) {
        this.Created = DateTime.Now;
        this.Level = level;
        this.Message = mesg;
    }
}