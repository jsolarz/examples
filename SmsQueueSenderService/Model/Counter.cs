namespace SmsQueueSenderService.Model
{
    public static class Counter
    {
        //private readonly object SyncRoot = new object();
        private static int value = 0;

        public static int Value
        {
            get
            {
                //lock (this.SyncRoot)
                //{
                    return value;
                //}
            }
        }

        public static int Decrement()
        {
            //lock (this.SyncRoot)
            //{
                return --value;
            //}
        }

        public static int Increment()
        {
            //lock (this.SyncRoot)
            //{
                return ++value;
            //}
        }
    }
}
