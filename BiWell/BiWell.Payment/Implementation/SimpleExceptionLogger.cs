using BiWell.Payment.Interfaces;

namespace BiWell.Payment.Implementation
{
    public class SimpleExceptionLogger : IExceptionLogger
    {
        public void Log(string exceptionMessage)
        {
            // NOTHING
        }
    }
}