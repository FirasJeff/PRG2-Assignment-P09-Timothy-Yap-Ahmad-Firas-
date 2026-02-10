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
string[] restaurantFile = File.ReadAllLines("restaurants.csv");
List<Restaurant> restaurantList = new List<Restaurant>();
for (int i = 1; i < restaurantFile.Length; i++)
{
    string[] lines = restaurantFile[i].Split(",");
    restaurantList.Add(new Restaurant(lines[0], lines[1], lines[2]));
}

string[] foodItemsFile = File.ReadAllLines("fooditems.csv");
for (int i = 1; i < foodItemsFile.Length; i++)
{
    string[] lines = foodItemsFile[i].Split(",");
    foreach (Restaurant r in restaurantList)
    {
        if (r.RestaurantId == lines[0])
        {
            FoodItem newFoodItem = new FoodItem(lines[1], lines[2], Convert.ToDouble(lines[3]), "");
            if (r.GetMenus().Count == 0)
            {
                r.AddMenu(new Menu(lines[0], r.RestaurantName));
            }
            Menu m = r.GetMenus()[0];
            m.AddFoodItem(newFoodItem);
        }
    }
}

string[] specialOffersFile = File.ReadAllLines("specialoffers.csv");
List<SpecialOffer> specialOffers = new List<SpecialOffer>();
for (int i = 1; i < specialOffersFile.Length; i++)
{
    string[] lines = specialOffersFile[i].Split(",");
    double discount = 0;
    if (lines[3] != "-")
    {
        discount = Convert.ToDouble(lines[3]);
    }

    specialOffers.Add(new SpecialOffer
    {
        Restaurant = lines[0],
        OfferCode = lines[1],
        OfferDesc = lines[2],
        Discount = discount
    });
}
//Timothy (S10268547H)-------------------------BASIC FEATURE 2-----------------------------------------------------------------------------------------
string[] customerFiles = File.ReadAllLines("customers.csv");
List<Customer> customerList = new List<Customer>();
for (int i = 1; i < customerFiles.Length; i++)
{
    string[] lines = customerFiles[i].Split(",");
    customerList.Add(new Customer(lines[1], lines[0]));
}

string[] orderFiles = File.ReadAllLines("orders.csv");
Stack<Order> refundStack = new Stack<Order>();
for (int i = 1; i < orderFiles.Length; i++)
{
    string line = orderFiles[i];
    string[] parts = line.Split(',');
    // If we have more than 10 parts, join the extra parts back to [9]
    if (parts.Length > 10)
    {
        string itemsField = parts[9];
        for (int j = 10; j < parts.Length; j++)
        {
            itemsField += "," + parts[j];
        }
        parts[9] = itemsField;
        string[] newParts = new string[10];
        for (int j = 0; j < 10; j++)
        {
            newParts[j] = parts[j];
        }
        parts = newParts;
    }

    DateTime deliveryDate = DateTime.ParseExact(parts[3], "dd/MM/yyyy", null);
    TimeSpan deliveryTime = TimeSpan.Parse(parts[4]);
    DateTime deliveryDateTime = deliveryDate.Add(deliveryTime);
    Order o = new Order(Convert.ToInt32(parts[0]), Convert.ToDateTime(parts[6]), Convert.ToDouble(parts[7]), parts[8], deliveryDateTime, parts[5], "", false);

    string itemsData = parts[9].Trim('"');
    if (!string.IsNullOrEmpty(itemsData))
    {
        string[] items = itemsData.Split('|');
        foreach (string item in items)
        {
            string[] itemParts = item.Split(',');
            if (itemParts.Length >= 2)
            {
                string itemName = itemParts[0].Trim();
                int qty = Convert.ToInt32(itemParts[1].Trim());

                foreach (Restaurant r in restaurantList)
                {
                    if (parts[2] == r.RestaurantId)
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

    foreach (Customer c in customerList)
    {
        if (parts[1] == c.EmailAddress)
        {
            c.AddOrder(o);
        }
    }
    foreach (Restaurant r in restaurantList)
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
    foreach (Restaurant r in restaurantList)
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

    foreach (Customer c in customerList)
    {
        foreach (Order o in c.orders)
        {
            // Find restaurant name
            string restaurantName = "";
            foreach (Restaurant r in restaurantList)
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
int GetNextOrderId(List<Customer> customerList)
{
    int maxId = 0;
    foreach (Customer customer in customerList)
    {
        foreach (Order order in customer.orders)
        {
            if (order.OrderID > maxId)
            {
                maxId = order.OrderID;
            }
        }
    }
    return maxId + 1;
}

//Timothy (S10268547H)-------------------------BASIC FEATURE 5 WITH ADVANCED FEATURE-----------------------------------------------------------------------------------------
bool CheckIfPublicHoliday(DateTime date)
{
    List<DateTime> publicHolidays = new List<DateTime>
    {
        new DateTime(date.Year, 1, 1),
        new DateTime(date.Year, 2, 17),
        new DateTime(date.Year, 2, 18),
        new DateTime(date.Year, 4, 3),
        new DateTime(date.Year, 4, 21),
        new DateTime(date.Year, 5, 1),
        new DateTime(date.Year, 5, 10),
        new DateTime(date.Year, 6, 8),
        new DateTime(date.Year, 8, 9),
        new DateTime(date.Year, 11, 6),
        new DateTime(date.Year, 12, 25)
    };

    return publicHolidays.Contains(date.Date);
}
void NewOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    Customer enteredCustomer = null;
    while (enteredCustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string newEmail = Console.ReadLine();
        bool foundCustomer = false;
        foreach (Customer c in customerList)
        {
            if (c.EmailAddress == newEmail)
            {
                enteredCustomer = c;
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
    string restaurantId = Console.ReadLine();
    DateTime deliveryDate = DateTime.MinValue; 
    TimeSpan deliveryTime = TimeSpan.Zero;
    DateTime deliveryDateTime = DateTime.MinValue;
    while (true)
    {
        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        deliveryDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);
        Console.Write("Enter Delivery Time (hh:mm): ");
        string timeInput = Console.ReadLine();
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

        deliveryDateTime = deliveryDate.Add(deliveryTime);
        if (deliveryDateTime < DateTime.Now)
        {
            Console.WriteLine("Delivery date/time cannot be in the past!");
        }
        else
        {
            break;
        }
    }
    Console.Write("Enter Delivery Address: ");
    string address = Console.ReadLine();
    Console.WriteLine("Available Food Items:");
    Restaurant selectedRestaurant = null;
    List<FoodItem> foodItemListInMenu = new List<FoodItem>();
    foreach (Restaurant r in restaurantList)
    {
        if (r.RestaurantId == restaurantId)
        {
            selectedRestaurant = r;
            Menu restaurantMenu = r.GetMenus()[0];
            foodItemListInMenu = restaurantMenu.GetFoodItems();
            break;
        }
    }

    if (selectedRestaurant == null)
    {
        Console.WriteLine("Restaurant not found!");
        return;
    }

    int count = 1;
    foreach (FoodItem f in foodItemListInMenu)
    {
        Console.WriteLine($"{count}. {f.ItemName} - ${f.ItemPrice:F2}");
        count++;
    }

    int nOrderId = GetNextOrderId(customerList);
    Order nOrder = new Order(nOrderId, DateTime.Now, 0, "Pending", deliveryDateTime, address, "", false);
    string specialRequest = "";
    while (true)
    {
        Console.Write("Enter item number (0 to finish): ");
        int itemOption = Convert.ToInt32(Console.ReadLine());
        if (itemOption > foodItemListInMenu.Count || itemOption < 0)
        {
            Console.WriteLine("Enter a valid food option");
        }
        else if (itemOption == 0)
        {
            while (true)
            {
                Console.Write("Add special request? [Y/N]: ");
                string specialRequestOrNot = Console.ReadLine().ToUpper();
                if (specialRequestOrNot == "Y")
                {
                    Console.Write("Enter your special request: ");
                    specialRequest = Console.ReadLine();
                    foreach (OrderedFoodItem item in nOrder.orderedItems)
                    {
                        item.Customise = specialRequest;
                    }
                    break;
                }
                else if (specialRequestOrNot == "N")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Enter a valid option (Y or N)");
                }
            }
            // Additional Feature 
            DateTime timeNow = DateTime.Now;
            string discountOccasion = null;
            double discountPercentage = 0;
            double discountMultiplier = 1.0;
            string specialOfferCode = null;
            string specialOfferDesc = null;

            // Newcomer discount 
            if (enteredCustomer.orders.Count == 0)
            {
                discountOccasion = "Newcomer discount";
                discountPercentage = 5;
                discountMultiplier = 0.95;
            }
            // Special offers from CSV
            else if (selectedRestaurant != null)
            {
                List<SpecialOffer> restaurantOffers = new List<SpecialOffer>();
                foreach (SpecialOffer offer in specialOffers)
                {
                    if (offer.Restaurant == selectedRestaurant.RestaurantName)
                    {
                        restaurantOffers.Add(offer);
                    }
                }

                SpecialOffer applicableOffer = null;
                // Early Bird 
                if (timeNow.Hour < 11)
                {
                    foreach (SpecialOffer offer in restaurantOffers)
                    {
                        if (offer.OfferCode == "EARL")
                        {
                            applicableOffer = offer;
                            break;
                        }
                    }
                }

                // Weekday 
                if (applicableOffer == null && timeNow.DayOfWeek >= DayOfWeek.Monday && timeNow.DayOfWeek <= DayOfWeek.Friday)
                {
                    foreach (SpecialOffer offer in restaurantOffers)
                    {
                        if (offer.OfferCode == "WEEK")
                        {
                            applicableOffer = offer;
                            break;
                        }
                    }
                }

                // Public Holiday 
                if (applicableOffer == null && CheckIfPublicHoliday(timeNow))
                {
                    foreach (SpecialOffer offer in restaurantOffers)
                    {
                        if (offer.OfferCode == "PHOL")
                        {
                            applicableOffer = offer;
                            break;
                        }
                    }
                }

                // Festive Season 
                if (applicableOffer == null && timeNow.Month == 12 && timeNow.Day >= 15 && timeNow.Day <= 25)
                {
                    foreach (SpecialOffer offer in restaurantOffers)
                    {
                        if (offer.OfferCode == "FEST")
                        {
                            applicableOffer = offer;
                            break;
                        }
                    }
                }

                // Apply offer
                if (applicableOffer != null)
                {
                    discountOccasion = applicableOffer.OfferDesc;
                    discountPercentage = applicableOffer.Discount;
                    discountMultiplier = 1 - (applicableOffer.Discount / 100.0);
                    specialOfferCode = applicableOffer.OfferCode;
                    specialOfferDesc = applicableOffer.OfferDesc;
                }
            }
            // Time based discounts 
            if (discountOccasion == null && timeNow.Hour >= 12 && timeNow.Hour <= 14)
            {
                discountOccasion = "Lunch discount";
                discountPercentage = 20;
                discountMultiplier = 0.8;
            }
            else if (discountOccasion == null && timeNow.Hour >= 18 && timeNow.Hour <= 20)
            {
                discountOccasion = "Dinner discount";
                discountPercentage = 10;
                discountMultiplier = 0.9;
            }

            double orderTotal = nOrder.CalculateOrderTotal();
            double deliveryFee = 5.00;

            // Check for DELI 
            SpecialOffer deliOffer = null;
            foreach (SpecialOffer offer in specialOffers)
            {
                if (offer.Restaurant == selectedRestaurant.RestaurantName && offer.OfferCode == "DELI")
                {
                    deliOffer = offer;
                    break;
                }
            }

            if (deliOffer != null && orderTotal >= 30)
            {
                deliveryFee = 0;
                Console.WriteLine($"\n {deliOffer.OfferDesc} applied! Delivery fee waived.");
            }

            double subTotal = orderTotal + deliveryFee;
            double discountAmount = 0;
            double finalTotal = subTotal;

            if (discountOccasion != null)
            {
                discountAmount = subTotal * (discountPercentage / 100.0);
                finalTotal = subTotal - discountAmount;
                nOrder.OrderTotal = finalTotal;
                Console.WriteLine($"\n{discountOccasion} ({discountPercentage}% off) has been applied!");
                Console.WriteLine($"Order Total:\t${orderTotal:F2}");
                Console.WriteLine($"Delivery Fee:\t+${deliveryFee:F2}");
                Console.WriteLine($"Subtotal:\t${subTotal:F2}");
                Console.WriteLine($"Discount ({discountPercentage}%):\t-${discountAmount:F2}");
                Console.WriteLine($"Final Total:\t${finalTotal:F2}");
            }
            else
            {
                nOrder.OrderTotal = subTotal;
                Console.WriteLine($"\nOrder Total: ${orderTotal:F2} + ${deliveryFee:F2} (delivery) = ${nOrder.OrderTotal:F2}");
            }
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
                    nOrder.OrderPaymentMethod = paymentMethod;
                    nOrder.OrderPaid = true;
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
            enteredCustomer.AddOrder(nOrder);
            selectedRestaurant.AddOrder(nOrder);
            selectedRestaurant.AddQueue(nOrder);
            string allOrderedItems = "";
            foreach (OrderedFoodItem o in nOrder.orderedItems)
            {
                if (allOrderedItems != "")
                {
                    allOrderedItems += "|";
                }
                allOrderedItems += $"{o.ItemName}, {o.QtyOrdered}";
            }

            string addIntoOrderCsv = $"{nOrderId},{enteredCustomer.EmailAddress},{restaurantId},{deliveryDate:dd/MM/yyyy},{deliveryTime:hh\\:mm},{address},{DateTime.Now:dd/MM/yyyy HH:mm},{nOrder.OrderTotal:F1},{nOrder.OrderStatus},\"{allOrderedItems}\"";
            File.AppendAllText("orders.csv", "\n" + addIntoOrderCsv);
            Console.WriteLine($"Order {nOrderId} created successfully! Status: {nOrder.OrderStatus}");
            break;
        }
        else
        {
            Console.Write("Enter quantity: ");
            int quantity = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < foodItemListInMenu.Count; i++)
            {
                if (itemOption == i + 1)
                {
                    FoodItem addedItem = foodItemListInMenu[i];
                    nOrder.AddOrderedFoodItem(new OrderedFoodItem(addedItem.ItemName, addedItem.ItemDesc, addedItem.ItemPrice, "", quantity));
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

    foreach (Restaurant r in restaurantList)
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
        foreach (Customer c in customerList)
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
    Customer modifyCustomer = null;
    while (modifyCustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        foreach (Customer c in customerList)
        {
            if (customerEmail == c.EmailAddress)
            {
                modifyCustomer = c;
                break;
            }
        }
        if (modifyCustomer == null)
        {
            Console.WriteLine("Customer email does not exist");
        }
    }
    Console.WriteLine("Pending Orders: ");
    List<Order> pendingOrders = new List<Order>();
    foreach (Order o in modifyCustomer.orders)
    {
        if (o.OrderStatus.Contains("Pending"))
        {
            Console.WriteLine(o.OrderID);
            pendingOrders.Add(o);
        }
    }
    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found.");
        return;
    }
    Order selectedOrder = null;
    while (selectedOrder == null)
    {
        Console.Write("Enter Order ID: ");
        int orderId = Convert.ToInt32(Console.ReadLine());

        foreach (Order o in pendingOrders)
        {
            if (orderId == o.OrderID)
            {
                selectedOrder = o;
                break;
            }
        }

        if (selectedOrder == null)
        {
            Console.WriteLine("Please enter a valid pending order ID");
        }
    }
    Console.WriteLine("Order Items:");
    int count = 1;
    foreach (OrderedFoodItem o in selectedOrder.orderedItems)
    {
        Console.WriteLine($"{count}. {o.ItemName} - {o.QtyOrdered}");
        count++;
    }
    Console.WriteLine("Address:");
    Console.WriteLine(selectedOrder.DeliveryAddress);
    Console.WriteLine("Delivery Date/Time:");
    Console.WriteLine($"{selectedOrder.DeliveryDateTime:d/M/yyyy}, {selectedOrder.DeliveryDateTime:HH:mm}");
    Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
    string modifyOption = Console.ReadLine();

    if (modifyOption == "3")
    {
        Console.Write("Enter new Delivery Time (hh:mm): ");
        string newTimeStr = Console.ReadLine();
        try
        {
            TimeSpan newTime = TimeSpan.Parse(newTimeStr);
            DateTime newDeliveryDateTime = selectedOrder.DeliveryDateTime.Date.Add(newTime);
            selectedOrder.DeliveryDateTime = newDeliveryDateTime;
            Console.WriteLine($"Order {selectedOrder.OrderID} updated. New Delivery Time: {newTimeStr}");
            
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid time format. Update cancelled.");
        }
    }
    else if (modifyOption == "1")
    {
        OrderedFoodItem selectedFoodItem = null;
        while (selectedFoodItem == null)
        {
            Console.Write("Enter index of food item from above (e.g 1): ");
            int indexFood = Convert.ToInt32(Console.ReadLine());
            if (indexFood > selectedOrder.orderedItems.Count || indexFood < 1)
            {
                Console.WriteLine("Enter a valid food item index");
            }
            else
            {
                selectedFoodItem = selectedOrder.orderedItems[indexFood - 1];
                break;
            }
        }
        double initialTotal = selectedFoodItem.CalculateSubTotal();
        Console.Write("Enter new quantity: ");
        int newQuantity = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Item quantity updated successfully!");
        selectedFoodItem.QtyOrdered = newQuantity;
        double newTotal = selectedOrder.CalculateOrderTotal();
        double diffToPay = newTotal - initialTotal;
        if (diffToPay > 0)
        {
            Console.WriteLine($"You have to pay an extra ${diffToPay} ");
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
    else if (modifyOption == "2")
    {
        Console.Write("Enter new delivery address: ");
        string newAddress = Console.ReadLine();
        selectedOrder.DeliveryAddress = newAddress;
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

    foreach (Customer c in customerList)
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

//Timothy (S10268547H)-------------------------ADVANCED FEATURE A---------------------------------------------------------------------------------------
void BulkProcessPendingOrders()
{
    DateTime today = DateTime.Today;
    //DateTime today = new DateTime(2026, 2, 5); //(To check for testing)
    DateTime currentTime = DateTime.Now;
    //DateTime currentTime = new DateTime(2026, 2, 5, 15, 0, 0); //(To check for testing)
    int totalPending = 0;
    List<Order> processedOrders = new List<Order>();
    List<Order> preparingOrders = new List<Order>();
    List<Order> rejectedOrders = new List<Order>();
    foreach (Restaurant r in restaurantList)
    {
        foreach (Order o in r.GetOrders())
        {
            if (o.OrderStatus == "Pending" && o.DeliveryDateTime.Date == today)
            {
                totalPending++;
                processedOrders.Add(o);
            }
        }
    }
    Console.WriteLine($"Total number of orders with 'Pending' status for today ({today:dd/MM/yyyy}): {totalPending}");
    foreach (Order o in processedOrders)
    {
        TimeSpan timeToDelivery = o.DeliveryDateTime - currentTime;
        // If delivery time is less than 1 hour from now then reject
        if (timeToDelivery.TotalHours < 1 && timeToDelivery.TotalHours >= 0)
        {
            o.OrderStatus = "Rejected";
            rejectedOrders.Add(o);
        }
        else
        {
            o.OrderStatus = "Preparing";
            preparingOrders.Add(o);
        }
    }

    Console.WriteLine("\nSummary Statistics:\n===================");
    Console.WriteLine($"Number of orders processed: {processedOrders.Count}");
    Console.WriteLine($"Number of 'Preparing' orders: {preparingOrders.Count}");
    Console.WriteLine($"Number of 'Rejected' orders: {rejectedOrders.Count}");
    if (processedOrders.Count > 0)
    {
        double percentageProcessed = (preparingOrders.Count * 100.0) / processedOrders.Count;
        Console.WriteLine($"Percentage of automatically processed orders: {percentageProcessed:F1}%");
    }
    else
    {
        Console.WriteLine("Percentage of automatically processed orders: 0.0%");
    }
   
}

//Firas (S10273408F)-------------------------ADVANCED FEATURE B-----------------------------------------------------------------------------------------
void DisplayTotalOrderAmount()
{
    Console.WriteLine("Total Order Amount Summary");
    Console.WriteLine("==========================");
    Console.WriteLine();

    double grandTotalOrderAmount = 0;
    double grandTotalRefunds = 0;
    int grandDeliveredCount = 0;

    const double DELIVERY_FEE = 5.00;
    const double GRUBEROO_COMMISSION = 0.30;

    // Process each restaurant
    foreach (Restaurant restaurant in restaurantList)
    {
        Console.WriteLine($"Restaurant: {restaurant.RestaurantName} ({restaurant.RestaurantId})");
        Console.WriteLine(new string('-', 60));

        double restaurantTotalDelivered = 0;
        double restaurantTotalRefunds = 0;
        int deliveredCount = 0;
        int refundedCount = 0;

        List<Order> restaurantOrders = restaurant.GetOrders();

        foreach (Order order in restaurantOrders)
        {
            if (order.OrderStatus == "Delivered")
            {
                restaurantTotalDelivered += order.OrderTotal;
                deliveredCount++;
            }
            else if (order.OrderStatus == "Cancelled" || order.OrderStatus == "Rejected")
            {
                restaurantTotalRefunds += order.OrderTotal;
                refundedCount++;
            }
        }

        
        Console.WriteLine($"Delivered Orders: {deliveredCount}");
        Console.WriteLine($"Total Order Amount (less delivery fee): ${restaurantTotalDelivered:F2}");
        Console.WriteLine($"Refunded Orders: {refundedCount}");
        Console.WriteLine($"Total Refunds: ${restaurantTotalRefunds:F2}");
        Console.WriteLine();

       
        grandTotalOrderAmount += restaurantTotalDelivered;
        grandTotalRefunds += restaurantTotalRefunds;
        grandDeliveredCount += deliveredCount;
    }

  
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("OVERALL SUMMARY");
    Console.WriteLine(new string('=', 60));
    Console.WriteLine($"Total Order Amount (all restaurants): ${grandTotalOrderAmount:F2}");
    Console.WriteLine($"Total Refunds (all restaurants): ${grandTotalRefunds:F2}");
    Console.WriteLine();

    // Gruberoo earnings = commission + delivery fees
    double commissionEarnings = grandTotalOrderAmount * GRUBEROO_COMMISSION;
    double deliveryEarnings = grandDeliveredCount * DELIVERY_FEE;
    double gruberooEarnings = commissionEarnings + deliveryEarnings;

    Console.WriteLine($"Final Amount Gruberoo Earns: ${gruberooEarnings:F2}");
    Console.WriteLine(new string('=', 60));
}



//Firas (S10273408F)-------------------------BONUS FEATURE - Auto Assign Drivers-----------------------------------------------------------------------------------------
//This bonus feature automatically assigns delivery drivers from a list of available drivers to orders that are in "Preparing" status.//
void AutoAssignDrivers()
{
    Console.WriteLine("Auto Assign Drivers to Orders");
    Console.WriteLine("==============================");
    Console.WriteLine();

    // Simple driver pool
    List<string> availableDrivers = new List<string>
    {
        "John Tan", "Sarah Lee", "Ahmad Rahman", "Wei Ming", "Priya Kumar",
        "David Wong", "Lisa Chen", "Kumar Singh", "Michelle Ng", "Ali Hassan"
    };

    int assignedCount = 0;
    int noPreparingOrders = 0;
    Random random = new Random();

    // Process each restaurant
    foreach (Restaurant restaurant in restaurantList)
    {
        List<Order> restaurantOrders = restaurant.GetOrders();
        bool hasPreparingOrders = false;

        foreach (Order order in restaurantOrders)
        {
            // Only assign drivers to "Preparing" orders
            if (order.OrderStatus == "Preparing")
            {
                if (!hasPreparingOrders)
                {
                    Console.WriteLine($"Restaurant: {restaurant.RestaurantName} ({restaurant.RestaurantId})");
                    Console.WriteLine(new string('-', 60));
                    hasPreparingOrders = true;
                }

                // Assign a random driver from the pool
                int driverIndex = random.Next(availableDrivers.Count);
                string assignedDriver = availableDrivers[driverIndex];

                // Find customer for this order
                string customerName = "";
                foreach (Customer c in customerList)
                {
                    foreach (Order o in c.orders)
                    {
                        if (o.OrderID == order.OrderID)
                        {
                            customerName = c.CustomerName;
                            break;
                        }
                    }
                    if (customerName != "") break;
                }

                Console.WriteLine($"  Order {order.OrderID}:");
                Console.WriteLine($"    Customer: {customerName}");
                Console.WriteLine($"    Delivery Time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"    Address: {order.DeliveryAddress}");
                Console.WriteLine($"    Amount: ${order.OrderTotal:F2}");
                Console.WriteLine($"    ✓ Assigned Driver: {assignedDriver}");
                Console.WriteLine();
                assignedCount++;
            }
        }

        if (hasPreparingOrders)
        {
            Console.WriteLine();
        }
        else
        {
            noPreparingOrders++;
        }
    }

    // Display summary
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("DRIVER ASSIGNMENT SUMMARY");
    Console.WriteLine(new string('=', 60));
    Console.WriteLine($"Total orders assigned to drivers: {assignedCount}");
    Console.WriteLine($"Available drivers in pool: {availableDrivers.Count}");
    Console.WriteLine(new string('=', 60));

    if (assignedCount > 0)
    {
        Console.WriteLine("\n All drivers have been notified.");
        Console.WriteLine(" Orders will be picked up and delivered on schedule.");
    }
    else
    {
        Console.WriteLine("No orders currently in 'Preparing' status.");
        Console.WriteLine("Use option 4 to process pending orders first.");
    }
}

void SaveQueueAndStack()
{
    // Save queue data
    using (StreamWriter writer = new StreamWriter("queue.csv"))
    {
        writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,TotalAmount,Status");

        foreach (Restaurant restaurant in restaurantList)
        {
            foreach (Order order in restaurant.GetOrders())
            {
                string customerEmail = "";
                foreach (Customer c in customerList)
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

            foreach (Customer c in customerList)
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

            foreach (Restaurant r in restaurantList)
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

Console.WriteLine($"Welcome to the Gruberoo Food Delivery System\n{restaurantList.Count} restaurants loaded!\n{restaurantList.Sum(r => r.GetMenus().Sum(m => m.GetFoodItems().Count))} food items loaded!\n{customerList.Count} customers loaded!\n{customerList.Sum(c => c.orders.Count)} orders loaded!\n{specialOffers.Count} special offers loaded!\n");

while (true)
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1.\tList all restaurants and menu items");
    Console.WriteLine("2.\tList all orders");
    Console.WriteLine("3.\tCreate a new order (With Bonus Features)");
    Console.WriteLine("4.\tProcess an order");
    Console.WriteLine("5.\tModify an existing order");
    Console.WriteLine("6.\tDelete an existing order");
    Console.WriteLine("7.\tDisplay total order amount (Advanced Feature)");
    Console.WriteLine("8.\tAuto assign drivers to orders (Bonus Feature)");
    Console.WriteLine("9.\tBulk processing orders for today (Advanced Feature)");
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
    else if (option == "7")
    {
        DisplayTotalOrderAmount();
    }
    else if (option == "8")
    {
        AutoAssignDrivers();
    }
    else if (option == "9")
    {
        BulkProcessPendingOrders();
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