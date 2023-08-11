using System.Diagnostics;
using System.Xml.Linq;

namespace OrderProcessing
{
    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">An array of strings.</param>
        /// <returns>A Task.<see cref="Task"/></returns>
        static async Task Main(string[] args)
        {
            string xmlFilePath = "order.xml"; // Replace with the actual path to your XML file

            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine("XML file not found.");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                XDocument xmlDocument = XDocument.Load(xmlFilePath);

                Task<string> orderIdTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("ID#"));
                Task<string> orderRequestDateTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("RequestedShipDate"));
                Task<string> createdByTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("CreatedBy"));

                var loadTasks = xmlDocument.Root.Elements("Items").Elements("Item").Select(item => Reader.ReadPropertyValueAsync(item.Element("LOADFACTOR")));
                double totalLoad = (await Task.WhenAll(loadTasks)).Sum(item => double.Parse(item ?? "0"));

                var quantityTasks = xmlDocument.Root.Elements("Items").Elements("Item").Select(item => Reader.ReadPropertyValueAsync(item.Element("Order_Quantity")));
                int totalQuantity = (await Task.WhenAll(quantityTasks)).Sum(item => int.Parse(item ?? "0"));

                var priceTasks = xmlDocument.Root.Elements("Items").Elements("Item").Select(item => Reader.ReadPropertyValueAsync(item.Element("USPrice")));
                decimal totalPrice = (await Task.WhenAll(priceTasks)).Sum(item => decimal.Parse(item ?? "0"));

                Task<string> customerNameTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("CustomerInfo")?.Element("Billing")?.Element("Name"));
                Task<string> customerAddressTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("CustomerInfo")?.Element("Billing")?.Element("Address1"));
                Task<string> customerPhoneTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("CustomerInfo")?.Element("Billing")?.Element("Phone"));
                Task<string> customerEmailTask = Reader.ReadPropertyValueAsync(xmlDocument.Root?.Element("CustomerInfo")?.Element("Billing")?.Element("DeliveryReceiptEmail"));

                await Task.WhenAll(orderIdTask, orderRequestDateTask, createdByTask, customerNameTask, customerAddressTask, customerPhoneTask, customerEmailTask);

                stopwatch.Stop();

                // Generate and print the output
                Console.WriteLine("Order Summary:");
                Console.WriteLine($"Order ID: {orderIdTask.Result}");
                Console.WriteLine($"Order Request Date: {orderRequestDateTask.Result}");
                Console.WriteLine($"Created By: {createdByTask.Result}");
                Console.WriteLine($"Load of the Order: {totalLoad}");
                Console.WriteLine($"Quantities of the Order: {totalQuantity}");
                Console.WriteLine($"Price: {totalPrice}");
                Console.WriteLine($"Customer Name: {customerNameTask.Result}");
                Console.WriteLine($"Customer Address: {customerAddressTask.Result}");
                Console.WriteLine($"Customer Phone: {customerPhoneTask.Result}");
                Console.WriteLine($"Customer Email: {customerEmailTask.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML: {ex.Message}");
            }

            Console.WriteLine($"Total response time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}