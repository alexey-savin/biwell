using BiWell.OrderLoader.ByDesignOrderApi;
using System.Diagnostics;

namespace BiWell.OrderLoader.Tasks
{
    public class UpdateShippingMethodsTask : ITask
    {
        public void Run()
        {
            var orderApiClient = new OrderAPISoapClient();

            var cred = new Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            var responseShippingMethods = orderApiClient.GetShippingMethods(cred);

            using (var dbContext = new BiWellEntities())
            {
                dbContext.Database.Log = s => Debug.WriteLine(s);

                foreach (var sm in responseShippingMethods)
                {
                    var dbShippingMethod = dbContext.shipping_method.Find(sm.ID);
                    if (dbShippingMethod != null)
                    {
                        dbShippingMethod.description = sm.Description;
                    }
                    else
                    {
                        dbShippingMethod = new shipping_method
                        {
                            id = sm.ID,
                            description = sm.Description
                        };

                        dbContext.shipping_method.Add(dbShippingMethod);
                    }
                }

                dbContext.SaveChanges();
            }
        }
    }
}
