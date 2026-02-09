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
    string line = orderfiles[i];
    string[] parts = line.Split(',');
    // If we have more than 10 parts, join the extra parts back to [9]
    if (parts.Length > 10)
    {
        string itemsfield = parts[9];
        for (int j = 10; j < parts.Length; j++)
        {
            itemsfield += "," + parts[j];
        }
        parts[9] = itemsfield;
        string[] newparts = new string[10];
        for (int j = 0; j < 10; j++)
        {
            newparts[j] = parts[j];
        }
        parts = newparts;
    }

    DateTime deliveryDate = DateTime.ParseExact(parts[3], "dd/MM/yyyy", null);
    TimeSpan deliveryTime = TimeSpan.Parse(parts[4]);
    DateTime deliveryDateTime = deliveryDate.Add(deliveryTime);
    Order o = new Order(Convert.ToInt32(parts[0]), Convert.ToDateTime(parts[6]), Convert.ToDouble(parts[7]), parts[8], deliveryDateTime, parts[5], "", false);

    string itemsdata = parts[9].Trim('"');
    if (!string.IsNullOrEmpty(itemsdata))
    {
        string[] items = itemsdata.Split('|');
        foreach (string item in items)
        {
            string[] itemparts = item.Split(',');
            if (itemparts.Length >= 2)
            {
                string itemname = itemparts[0].Trim();
                int qty = Convert.ToInt32(itemparts[1].Trim());

                foreach (Restaurant r in restaurantlist)
                {
                    if (parts[2] == r.RestaurantId)
                    {
                        Menu menu = r.GetMenus()[0];
                        foreach (FoodItem f in menu.GetFoodItems())
                        {
                            if (f.ItemName == itemname)
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
        if (parts[1] == c.EmailAddress)
        {
            c.AddOrder(o);
        }
    }
    foreach (Restaurant r in restaurantlist)
    {
        if (parts[2] == r.RestaurantId)
        {
            r.AddQueue(o);
            r.AddOrder(o);
        }
    }
}
//Timothy(S10268547H)------------------------ - BASIC FEATURE 3-----------------------------------------------------------------------------------------
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
void ModifyingOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");
    Customer modifycustomer = null;
    while (modifycustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string customeremail = Console.ReadLine();

        foreach (Customer c in customerlist)
        {
            if (customeremail == c.EmailAddress)
            {
                modifycustomer = c;
                break;
            }
        }
        if (modifycustomer == null)
        {
            Console.WriteLine("Customer email does not exist");
        }
    }
    Console.WriteLine("Pending Orders: ");
    List<Order> pendingorders = new List<Order>();
    foreach (Order o in modifycustomer.orders)
    {
        if (o.OrderStatus.Contains("Pending")) 
        {
            Console.WriteLine(o.OrderID);
            pendingorders.Add(o);
        }
    }
    if (pendingorders.Count == 0)
    {
        Console.WriteLine("No pending orders found.");
        return;
    }
    Order selectedorder = null;
    while (selectedorder == null)
    {
        Console.Write("Enter Order ID: ");
        int orderid = Convert.ToInt32(Console.ReadLine());

        foreach (Order o in pendingorders)
        {
            if (orderid == o.OrderID)
            {
                selectedorder = o;
                break;
            }
        }

        if (selectedorder == null)
        {
            Console.WriteLine("Please enter a valid pending order ID");
        }
    }
    Console.WriteLine("Order Items:");
    int count = 1;
    foreach (OrderedFoodItem o in selectedorder.orderedItems)
    {
        Console.WriteLine($"{count}. {o.ItemName} - {o.QtyOrdered}");
        count++;
    }
    Console.WriteLine("Address:");
    Console.WriteLine(selectedorder.DeliveryAddress);
    Console.WriteLine("Delivery Date/Time:");
    Console.WriteLine($"{selectedorder.DeliveryDateTime:d/M/yyyy}, {selectedorder.DeliveryDateTime:HH:mm}");
    Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
    string modifyoption = Console.ReadLine();

    if (modifyoption == "3")  
    {
        Console.Write("Enter new Delivery Time (hh:mm): ");
        string newtimestr = Console.ReadLine();
        try
        {
            TimeSpan newtime = TimeSpan.Parse(newtimestr);
            DateTime newdeliverydatetime = selectedorder.DeliveryDateTime.Date.Add(newtime);
            selectedorder.DeliveryDateTime = newdeliverydatetime;
            Console.WriteLine($"Order {selectedorder.OrderID} updated. New Delivery Time: {newtimestr}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid time format. Update cancelled.");
        }
    }
    else if (modifyoption == "1")
    {
        OrderedFoodItem selectedfooditem = null; 
        while (selectedfooditem == null)
        {
            Console.Write("Enter index of food item from above (e.g 1): ");
            int indexfood = Convert.ToInt32(Console.ReadLine());
            if (indexfood > selectedorder.orderedItems.Count || indexfood < 1)
            {
                Console.WriteLine("Enter a valid food item index");
            }
            else
            {
                selectedfooditem = selectedorder.orderedItems[indexfood - 1];
                break;
            }
        }
        double initialtotal = selectedfooditem.CalculateSubTotal();
        Console.Write("Enter new quantity: ");
        int newquantity = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Item quantity updated successfully!");
        selectedfooditem.QtyOrdered = newquantity;
        double newtotal = selectedorder.CalculateOrderTotal();
        double difftopay = newtotal - initialtotal;
        if (difftopay > 0)
        {
            Console.WriteLine($"You have to pay an extra ${difftopay} ");
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
        }
    }
    else if (modifyoption == "2")
    {
        Console.Write("Enter new delivery address: ");
        string newAddress = Console.ReadLine();
        selectedorder.DeliveryAddress = newAddress;
        Console.WriteLine("Address updated successfully!");
    }
    else
    {
        Console.WriteLine("Invalid option.");
    }
}


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
        ModifyingOrder();
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