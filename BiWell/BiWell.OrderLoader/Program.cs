using BiWell.OrderLoader.Tasks;

namespace BiWell.OrderLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            ITask[] tasks =
            {
                new LoadCreatedOrdersTask(),
                new LoadUpdatedOrdersTask()
            };

            foreach (var task in tasks)
            {
                task.Run();
            }
        }
    }
}
