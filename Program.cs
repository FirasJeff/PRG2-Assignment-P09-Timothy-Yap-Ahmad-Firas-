//==========================================================
// Student Number : S10268547H (Timothy), S10273408F (Firas)
// Student Name : Timothy Yap, Firas
// Partner Name : Timothy & Firas
//==========================================================

using FirasTimothy_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//Timothy (S10268547H)-------------------------BASIC FEATURE 1-----------------------------------------------------------------------------------------
string[] restaurantfile = File.ReadAllLines("restaurants.csv");
List<Restaurant> restaurantlist = new List<Restaurant>();
for (int i = 1; i < restaurantfile.Length; i++)
{
    string[] lines = restaurantfile[i].Split(",");
    restaurantlist.Add(new Restaurant(lines[0], lines[1], lines[2]));
}

string[] fooditemsfile = File.ReadAllLines("fooditems.csv");
for (int i = 1; i < fooditemsfile.Length; i++)
{
    string[] lines = fooditemsfile[i].Split(",");
    foreach (Restaurant r in restaurantlist)
    {
        if (r.RestaurantId == lines[0])
        {
            FoodItem newfoodItem = new FoodItem(lines[1], lines[2], Convert.ToDouble(lines[3]), "");
            if (r.GetMenus().Count == 0)
            {
                r.AddMenu(new Menu(lines[0], r.RestaurantName));
            }
            Menu m = r.GetMenus()[0];
            m.AddFoodItem(newfoodItem);
        }
    }
}

//Timothy (S10268547H)-------------------------BASIC FEATURE 2-----------------------------------------------------------------------------------------
string[] customerfiles = File.ReadAllLines("customers.csv");
List<Customer> customerlist = new List<Customer>();
for (int i = 1; i < customerfiles.Length; i++)
{
    string[] lines = customerfiles[i].Split(",");
    customerlist.Add(new Customer(lines[1], lines[0]));
}

string[] orderfiles = File.ReadAllLines("orders.csv");
Stack<Order> refundStack = new Stack<Order>();
for (int i = 1; i < orderfiles.Length; i++)
{
    string[] lines = orderfiles[i].Split(",");
    DateTime deliveryDate = DateTime.ParseExact(lines[3], "dd/MM/yyyy", null);
    TimeSpan deliveryTime = TimeSpan.Parse(lines[4]);
    DateTime deliveryDateTime = deliveryDate.Add(deliveryTime);
    Order o = new Order(Convert.ToInt32(lines[0]), Convert.ToDateTime(lines[6]), Convert.ToDouble(lines[7]), lines[8], deliveryDateTime, lines[5], "", false);

    // Load ordered items if present in CSV (column 9+)
    if (lines.Length > 9 && !string.IsNullOrEmpty(lines[9]))
    {
        string itemsData = lines[9].Trim('"');
        string[] items = itemsData.Split('|');
        foreach (string item in items)
        {
            string[] itemParts = item.Split(',');
            if (itemParts.Length >= 2)
            {
                string itemName = itemParts[0].Trim();
                int qty = Convert.ToInt32(itemParts[1].Trim());

                // Find the food item details from restaurant
                foreach (Restaurant r in restaurantlist)
                {
                    if (lines[2] == r.RestaurantId)
                    {
                        Menu menu = r.GetMenus()[0];
                        foreach (FoodItem f in menu.GetFoodItems())
                        {
                            if (f.ItemName == itemName)
                            {
                                o.AddOrderedFoodItem(new OrderedFoodItem(f.ItemName, f.ItemDesc, f.ItemPrice, "", qty));
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    foreach (Customer c in customerlist)
    {
        if (lines[1] == c.EmailAddress)
        {
            c.AddOrder(o);
        }
    }
    foreach (Restaurant r in restaurantlist)
    {
        if (lines[2] == r.RestaurantId)
        {
            r.AddQueue(o);
            r.AddOrder(o);
        }
    }
}

//Timothy (S10268547H)-------------------------BASIC FEATURE 3-----------------------------------------------------------------------------------------
void ShowAllRestaurants()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant r in restaurantlist)
    {
        Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");
        Menu m = r.GetMenus()[0];
        foreach (FoodItem f in m.GetFoodItems())
        {
            Console.WriteLine($" - {f.ItemName}: {f.ItemDesc} - ${f.ItemPrice:F2}");
        }
        Console.WriteLine();
    }
}

//Firas (S10273408F)-------------------------BASIC FEATURE 4-----------------------------------------------------------------------------------------
void ListAllOrders()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine($"{"Order ID",-8} {"Customer",-10} {"Restaurant",-13} {"Delivery Date/Time",-18} {"Amount",-6} {"Status",-9}");
    Console.WriteLine($"{"--------",-8} {"----------",-10} {"-------------",-13} {"------------------",-18} {"------",-6} {"---------",-9}");

    foreach (Customer c in customerlist)
    {
        foreach (Order o in c.orders)
        {
            // Find restaurant name
            string restaurantName = "";
            foreach (Restaurant r in restaurantlist)
            {
                foreach (Order ro in r.GetOrders())
                {
                    if (ro.OrderID == o.OrderID)
                    {
                        restaurantName = r.RestaurantName;
                        break;
                    }
                }
                if (restaurantName != "") 
                    break;
            }

            Console.WriteLine($"{o.OrderID,-8} {c.CustomerName,-10} {restaurantName,-13} {o.DeliveryDateTime:dd/MM/yyyy HH:mm,-18} ${o.OrderTotal,-6:F2} {o.OrderStatus}");
        }
    }
}

//Timothy (S10268547H)
int GetNextOrderId(List<Customer> customerlist)
{
    int maxid = 0;
    foreach (Customer customer in customerlist)
    {
        foreach (Order order in customer.orders)
        {
            if (order.OrderID > maxid)
            {
                maxid = order.OrderID;
            }
        }
    }
    return maxid + 1;
}

//Timothy (S10268547H)-------------------------BASIC FEATURE 5-----------------------------------------------------------------------------------------
void NewOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    Customer enteredcustomer = null;
    while (enteredcustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string newmail = Console.ReadLine();
        bool foundCustomer = false;
        foreach (Customer c in customerlist)
        {
            if (c.EmailAddress == newmail)
            {
                enteredcustomer = c;
                foundCustomer = true;
                break;
            }
        }
        if (foundCustomer != true)
        {
            Console.WriteLine("Enter a valid email address");
        }
    }

    Console.Write("Enter Restaurant ID: ");
    string restaurantid = Console.ReadLine();
    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
    DateTime deliverydate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);
    Console.Write("Enter Delivery Time (hh:mm): ");
    string timeInput = Console.ReadLine();
    TimeSpan deliveryTime;
    while (true)
    {
        try
        {
            deliveryTime = TimeSpan.Parse(timeInput);
            break;
        }
        catch (FormatException)
        {
            Console.Write("Invalid time format. Please enter in HH:mm format (e.g 14:30): ");
            timeInput = Console.ReadLine();
        }
    }

    DateTime deliverydatetime = deliverydate.Add(deliveryTime);
    Console.Write("Enter Delivery Address: ");
    string address = Console.ReadLine();
    Console.WriteLine("Available Food Items:");
    Restaurant selectedrestaurant = null;
    List<FoodItem> fooditemlistinmenu = new List<FoodItem>();
    foreach (Restaurant r in restaurantlist)
    {
        if (r.RestaurantId == restaurantid)
        {
            selectedrestaurant = r;
            Menu restaurantmenu = r.GetMenus()[0];
            fooditemlistinmenu = restaurantmenu.GetFoodItems();
            break;
        }
    }

    if (selectedrestaurant == null)
    {
        Console.WriteLine("Restaurant not found!");
        return;
    }

    int count = 1;
    foreach (FoodItem f in fooditemlistinmenu)
    {
        Console.WriteLine($"{count}. {f.ItemName} - ${f.ItemPrice:F2}");
        count++;
    }

    int norderid = GetNextOrderId(customerlist);
    Order norder = new Order(norderid, DateTime.Now, 0, "Pending", deliverydatetime, address, "", false);
    string specialrequest = "";
    while (true)
    {
        Console.Write("Enter item number (0 to finish): ");
        int itemoption = Convert.ToInt32(Console.ReadLine());
        if (itemoption > fooditemlistinmenu.Count || itemoption < 0)
        {
            Console.WriteLine("Enter a valid food option");
        }
        else if (itemoption == 0)
        {
            while (true)
            {
                Console.Write("Add special request? [Y/N]: ");
                string specialrequestornot = Console.ReadLine().ToUpper();
                if (specialrequestornot == "Y")
                {
                    Console.Write("Enter your special request: ");
                    specialrequest = Console.ReadLine();
                    foreach (OrderedFoodItem item in norder.orderedItems)
                    {
                        item.Customise = specialrequest;
                    }
                    break;
                }
                else if (specialrequestornot == "N")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Enter a valid option (Y or N)");
                }
            }

            double ordertotal = 0;
            ordertotal = norder.CalculateOrderTotal();
            Console.WriteLine($"Order Total: ${ordertotal:F2} + $5.00 (delivery) = ${(ordertotal + 5):F2}");
            norder.OrderTotal = ordertotal + 5;
            while (true)
            {
                Console.Write("Proceed to payment? [Y/N]: ");
                string proceed = Console.ReadLine().ToUpper();
                if (proceed == "Y")
                {
                    string paymentMethod = "";
                    while (true)
                    {
                        Console.Write("Payment method:\n[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                        paymentMethod = Console.ReadLine().ToUpper();
                        if (paymentMethod == "CC" || paymentMethod == "PP" || paymentMethod == "CD")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid option (CC, PP, or CD)");
                        }
                    }
                    norder.OrderPaymentMethod = paymentMethod;
                    norder.OrderPaid = true;
                    break;
                }
                else if (proceed == "N")
                {
                    Console.WriteLine("Order cancelled.");
                    return;
                }
                else
                {
                    Console.WriteLine("Enter a valid option (Y or N)");
                }
            }
            enteredcustomer.AddOrder(norder);
            selectedrestaurant.AddOrder(norder);
            selectedrestaurant.AddQueue(norder);
            string allordereditems = "";
            foreach (OrderedFoodItem o in norder.orderedItems)
            {
                if (allordereditems != "")
                {
                    allordereditems += "|";
                }
                allordereditems += $"{o.ItemName}, {o.QtyOrdered}";
            }

            string addintoordercsv = $"{norderid},{enteredcustomer.EmailAddress},{restaurantid},{deliverydate:dd/MM/yyyy},{deliveryTime:hh\\:mm},{address},{DateTime.Now:dd/MM/yyyy HH:mm},{norder.OrderTotal:F1},{norder.OrderStatus},\"{allordereditems}\"";
            File.AppendAllText("orders.csv", "\n" + addintoordercsv);
            Console.WriteLine($"Order {norderid} created successfully! Status: {norder.OrderStatus}");
            break;
        }
        else
        {
            Console.Write("Enter quantity: ");
            int quantity = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < fooditemlistinmenu.Count; i++)
            {
                if (itemoption == i + 1)
                {
                    FoodItem addeditem = fooditemlistinmenu[i];
                    norder.AddOrderedFoodItem(new OrderedFoodItem(addeditem.ItemName, addeditem.ItemDesc, addeditem.ItemPrice, "", quantity));
                    break;
                }
            }
        }
    }
}

//Firas (S10273408F)-------------------------BASIC FEATURE 6-----------------------------------------------------------------------------------------
void ProcessOrder()
{
    Console.WriteLine("Process Order");
    Console.WriteLine("=============");

    Console.Write("Enter Restaurant ID: ");
    string restaurantId = Console.ReadLine();
    Restaurant restaurant = null;

    foreach (Restaurant r in restaurantlist)
    {
        if (r.RestaurantId == restaurantId)
        {
            restaurant = r;
            break;
        }
    }

    if (restaurant == null)
    {
        Console.WriteLine("Restaurant not found!");
        return;
    }

    List<Order> ordersList = restaurant.GetOrders();
    if (ordersList.Count == 0)
    {
        Console.WriteLine("No orders for this restaurant.");
        return;
    }

    // Process each order
    List<Order> processedOrders = new List<Order>();
    foreach (Order order in ordersList)
    {
        if (processedOrders.Contains(order)) 
           continue;

        Console.WriteLine($"Order {order.OrderID}:");

        // Find customer name
        string customerName = "";
        foreach (Customer c in customerlist)
        {
            foreach (Order o in c.orders)
            {
                if (o.OrderID == order.OrderID)
                {
                    customerName = c.CustomerName;
                    break;
                }
            }
            if (customerName != "") 
                break;
        }

        Console.WriteLine($"Customer: {customerName}");
        Console.WriteLine("Ordered Items:");

        for (int i = 0; i < order.orderedItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {order.orderedItems[i].ItemName} - {order.orderedItems[i].QtyOrdered}");
        }

        Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${order.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {order.OrderStatus}");

        Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string action = Console.ReadLine().ToUpper();

        if (action == "C")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Preparing";
                Console.WriteLine($"Order {order.OrderID} confirmed. Status: Preparing");
            }
            else
            {
                Console.WriteLine($"Cannot confirm. Order status is {order.OrderStatus}");
            }
        }
        else if (action == "R")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Rejected";
                refundStack.Push(order);
                Console.WriteLine($"Order {order.OrderID} rejected. Refund of ${order.OrderTotal:F2} processed.");
            }
            else
            {
                Console.WriteLine($"Cannot reject. Order status is {order.OrderStatus}");
            }
        }
        else if (action == "S")
        {
            if (order.OrderStatus == "Cancelled")
            {
                Console.WriteLine($"Order {order.OrderID} skipped.");
            }
            else
            {
                Console.WriteLine($"Can only skip cancelled orders. Current status: {order.OrderStatus}");
            }
        }
        else if (action == "D")
        {
            if (order.OrderStatus == "Preparing")
            {
                order.OrderStatus = "Delivered";
                Console.WriteLine($"Order {order.OrderID} delivered. Status: Delivered");
            }
            else
            {
                Console.WriteLine($"Cannot deliver. Order status is {order.OrderStatus}");
            }
        }
        else
        {
            Console.WriteLine("Invalid action.");
        }

        processedOrders.Add(order);
    }
}

//Timothy (S10273408F)-------------------------BASIC FEATURE 7-----------------------------------------------------------------------------------------


//Firas (S10273408F)-------------------------BASIC FEATURE 8-----------------------------------------------------------------------------------------
void DeleteExistingOrder()
{
    Console.WriteLine("Delete Order");
    Console.WriteLine("============");

    Console.Write("Enter Customer Email: ");
    string customerEmail = Console.ReadLine();
    Customer customer = null;

    foreach (Customer c in customerlist)
    {
        if (c.EmailAddress == customerEmail)
        {
            customer = c;
            break;
        }
    }

    if (customer == null)
    {
        Console.WriteLine("Customer not found!");
        return;
    }

    // Find pending orders
    List<Order> pendingOrders = new List<Order>();
    foreach (Order o in customer.orders)
    {
        if (o.OrderStatus == "Pending")
        {
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (Order order in pendingOrders)
    {
        Console.WriteLine(order.OrderID);
    }

    Console.Write("Enter Order ID: ");
    int orderId = Convert.ToInt32(Console.ReadLine());

    Order targetOrder = null;
    foreach (Order o in pendingOrders)
    {
        if (o.OrderID == orderId)
        {
            targetOrder = o;
            break;
        }
    }

    if (targetOrder == null)
    {
        Console.WriteLine("Order not found or not pending.");
        return;
    }

    // Display order details
    Console.WriteLine($"Customer: {customer.CustomerName}");
    Console.WriteLine("Ordered Items:");
    for (int i = 0; i < targetOrder.orderedItems.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {targetOrder.orderedItems[i].ItemName} - {targetOrder.orderedItems[i].QtyOrdered}");
    }
    Console.WriteLine($"Delivery date/time: {targetOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Total Amount: ${targetOrder.OrderTotal:F2}");
    Console.WriteLine($"Order Status: {targetOrder.OrderStatus}");

    Console.Write("Confirm deletion? [Y/N]: ");
    string confirm = Console.ReadLine().ToUpper();

    if (confirm == "Y")
    {
        targetOrder.OrderStatus = "Cancelled";
        refundStack.Push(targetOrder);
        Console.WriteLine($"Order {targetOrder.OrderID} cancelled. Refund of ${targetOrder.OrderTotal:F2} processed.");
    }
    else
    {
        Console.WriteLine("Deletion cancelled.");
    }
}

void SaveQueueAndStack()
{
    // Save queue data
    using (StreamWriter writer = new StreamWriter("queue.csv"))
    {
        writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,TotalAmount,Status");

        foreach (Restaurant restaurant in restaurantlist)
        {
            foreach (Order order in restaurant.GetOrders())
            {
                string customerEmail = "";
                foreach (Customer c in customerlist)
                {
                    foreach (Order o in c.orders)
                    {
                        if (o.OrderID == order.OrderID)
                        {
                            customerEmail = c.EmailAddress;
                            break;
                        }
                    }
                    if (customerEmail != "") break;
                }

                writer.WriteLine($"{order.OrderID},{customerEmail},{restaurant.RestaurantId}," +
                               $"{order.DeliveryDateTime:dd/MM/yyyy},{order.DeliveryDateTime:HH:mm}," +
                               $"{order.DeliveryAddress},{order.OrderTotal:F1},{order.OrderStatus}");
            }
        }
    }

    // Save stack data
    using (StreamWriter writer = new StreamWriter("stack.csv"))
    {
        writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,TotalAmount,Status");

        foreach (Order order in refundStack)
        {
            string customerEmail = "";
            string restaurantId = "";

            foreach (Customer c in customerlist)
            {
                foreach (Order o in c.orders)
                {
                    if (o.OrderID == order.OrderID)
                    {
                        customerEmail = c.EmailAddress;
                        break;
                    }
                }
                if (customerEmail != "") break;
            }

            foreach (Restaurant r in restaurantlist)
            {
                foreach (Order ro in r.GetOrders())
                {
                    if (ro.OrderID == order.OrderID)
                    {
                        restaurantId = r.RestaurantId;
                        break;
                    }
                }
                if (restaurantId != "") break;
            }

            writer.WriteLine($"{order.OrderID},{customerEmail},{restaurantId}," +
                           $"{order.DeliveryDateTime:dd/MM/yyyy},{order.DeliveryDateTime:HH:mm}," +
                           $"{order.DeliveryAddress},{order.OrderTotal:F1},{order.OrderStatus}");
        }
    }

    Console.WriteLine("Queue and stack saved successfully!");
}

Console.WriteLine($"Welcome to the Gruberoo Food Delivery System\n{restaurantlist.Count} restaurants loaded!\n{restaurantlist.Sum(r => r.GetMenus().Sum(m => m.GetFoodItems().Count))} food items loaded!\n{customerlist.Count} customers loaded!\n{customerlist.Sum(c => c.orders.Count)} orders loaded!\n");

while (true)
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1.\tList all restaurants and menu items");
    Console.WriteLine("2.\tList all orders");
    Console.WriteLine("3.\tCreate a new order");
    Console.WriteLine("4.\tProcess an order");
    Console.WriteLine("5.\tModify an existing order");
    Console.WriteLine("6.\tDelete an existing order");
    Console.WriteLine("0.\tExit");
    Console.Write("Enter your choice: ");
    string option = Console.ReadLine();
    Console.WriteLine();

    if (option == "1")
    {
        ShowAllRestaurants();
    }
    else if (option == "2")
    {
        ListAllOrders();
    }
    else if (option == "3")
    {
        NewOrder();
    }
    else if (option == "4")
    {
        ProcessOrder();
    }
    else if (option == "5")
    {
        Console.WriteLine("Timothy waiting for you my man");
    }
    else if (option == "6")
    {
        DeleteExistingOrder();
    }
    else if (option == "0")
    {
        SaveQueueAndStack();
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice. Please try again.");
    }
    Console.WriteLine();
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();