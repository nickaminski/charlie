namespace charlie.dto
{
    public class MessagePacket
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
        public string Username { get; set; }
        public string ChannelId { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}", Id, Username, Message, Timestamp);
        }
    }
}
