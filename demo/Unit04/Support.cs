using System;

namespace demo.Unit04
{
    public class Support
    {
        private static Object _sync;
        private static Support _instance;

        public static Support Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new Support();

                    return _instance;
                }
            }
        }

        static Support()
        {
            _sync = new Object();
        }
    }
}
