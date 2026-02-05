using FirasTimothy_PRG2Assignment;
//Timothy (S10268547H)
string[] restaurantfile = File.ReadAllLines("restaurants.csv");
List<Restaurant> restaurantlist = new List<Restaurant>();
for(int i = 1; i<restaurantfile.Length; i++)
{
    string[] lines = restaurantfile[i].Split(",");
    restaurantlist.Add(new Restaurant(lines[0], lines[1], lines[2]));
}

string[] fooditemsfile = File.ReadAllLines("fooditems.csv");
for(int i = 1; i < fooditemsfile.Length; i++)
{
    string[] lines = fooditemsfile[i].Split(",");
    foreach(Restaurant r in restaurantlist)
    {
        if(r.RestaurantId == lines[0])
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
string[] customerfiles = File.ReadAllLines("customers.csv");
List<Customer> customerlist = new List<Customer>();
for (int i = 1;i < customerfiles.Length; i++)
{
    string[] lines = customerfiles[i].Split(",");
    customerlist.Add(new Customer(lines[1], lines[0]));
}
string[] orderfiles = File.ReadAllLines("orders.csv");
for (int i = 1; i < orderfiles.Length; i++)
{
    string[] lines = orderfiles[i].Split(",");
    DateTime deliveryDate = DateTime.ParseExact(lines[3], "dd/MM/yyyy", null);
    TimeSpan deliveryTime = TimeSpan.Parse(lines[4]);
    DateTime deliveryDateTime = deliveryDate.Add(deliveryTime);
    Order o = new Order(Convert.ToInt32(lines[0]), Convert.ToDateTime(lines[6]), Convert.ToDouble(lines[7]), lines[8], deliveryDateTime, lines[5], "", false);
    foreach(Customer c in customerlist)
    {
        if (lines[1] == c.EmailAddress)
        {
            c.AddOrder(o);
        }
    }
    foreach(Restaurant r in restaurantlist)
    {
        if (lines[2] == r.RestaurantId)
        {
            r.AddQueue(o);
        }
    }
}

//Timothy (S10268547H)
void ShowAllRestaurants()
{
    Console.WriteLine("\nAll Restaurants and Menus Items\n================================");
    foreach (Restaurant r in restaurantlist)
    {
        Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");
        Menu m = r.GetMenus()[0];
        foreach (FoodItem f in m.GetFoodItems())
        {
            Console.WriteLine($"-{f.ItemName}: {f.ItemDesc} - ${f.ItemPrice}");
        }
        Console.WriteLine();
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
void NewOrder()
{
    Console.WriteLine("Create New Order \r\n================ \r\n");
    Customer enteredcustomer = null;  
    while (enteredcustomer == null)  
    {
        Console.Write("Enter Customer Email: ");
        string nemail = Console.ReadLine();
        bool foundCustomer = false; 
        foreach (Customer c in customerlist)
        {
            if (c.EmailAddress == nemail)
            {
                enteredcustomer = c;  
                foundCustomer = true;
                break;  
            }
        }
        if (!foundCustomer) 
        {
            Console.WriteLine("Enter a valid email address");
        }
    }

    Console.Write("Enter Restaurant ID: ");
    string restaurantid = Console.ReadLine();
    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
    DateTime deliverydate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);
    Console.Write("Enter Delivery Time (HH:mm): ");
    string timeInput = Console.ReadLine();
    TimeSpan deliveryTime; // Sets deliverytime to use in while loop
    while (true)
    {
        try
        {
            deliveryTime = TimeSpan.Parse(timeInput);
            break;
        }
        catch (FormatException)
        {
            Console.Write("Invalid time format. Please enter in HH:mm format (e.g" +
                " 14:30): ");
            timeInput = Console.ReadLine();
        }
    }

    DateTime deliverydatetime = deliverydate.Add(deliveryTime);
    Console.Write("Enter Delivery Address: ");
    string address = Console.ReadLine();
    Console.WriteLine("\nAvailable Food Items:");
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
        Console.WriteLine($"{count}. {f.ItemName} - ${f.ItemPrice:F2} ");  
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
            Console.WriteLine($"\nOrder Total: ${ordertotal:F2} + $5.00 (delivery) = ${(ordertotal + 5):F2}");
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
                        Console.Write("Payment method: [CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
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
                    norder.OrderPaymentMethod= paymentMethod;
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





Console.WriteLine("Welcome to the Gruberoo Food Delivery System 15 restaurants loaded! \r\n51 food items loaded! \r\n20 customers loaded! \r\n35 orders loaded! \r\n");
while (true)
{
    Console.WriteLine("===== Gruberoo Food Delivery System ===== \r\n1.\tList all restaurants and menu items \r\n2.\tList all orders \r\n3.\tCreate a new order \r\n4.\tProcess an order \r\n5.\tModify an existing order \r\n6.\tDelete an existing order \r\n0.\tExit");
    Console.Write("Enter your choice: ");
    string option = Console.ReadLine();
    if (option == "1")
    {
        ShowAllRestaurants();
    }
    else if (option == "3")
    {
        NewOrder();
    }
    else if (option == "0")
    {
        break;
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
