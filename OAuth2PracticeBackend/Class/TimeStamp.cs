namespace OAuth2PracticeBackend.Class
{
    public class TimeStamp<T>
    {

        public TimeStamp(T originData, double addMinutes = 5) {
            data = originData;
            time_stamp = DateTime.Now.AddMinutes(addMinutes);
        }

        public DateTime time_stamp { get; }

        public T data { get; set; }

    }
}
