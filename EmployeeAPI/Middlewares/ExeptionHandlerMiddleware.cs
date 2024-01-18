namespace EmployeeAPI.Middlewares
{
    public class ExeptionHandlerMiddleware
    {
        public static void LogException(string message)
        {
            string logFilePath = @"C:\ErrorLog.txt";
            using StreamWriter writer = new(logFilePath, true);
            writer.WriteLine("Exception occurred at " + DateTime.Now.ToString());
            writer.WriteLine("Message: " + message);
            writer.WriteLine();
        }
    }
}
