using FirasTimothy_PRG2Assignment;
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

Console.WriteLine("All Restaurants and Menus Items\n================================");
foreach(Restaurant r in restaurantlist)
{
    Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");
    Menu m = r.GetMenus()[0];
    foreach(FoodItem f in m.GetFoodItems())
    {
        Console.WriteLine($"-{f.ItemName}: {f.ItemDesc} - {f.ItemPrice}");
    }
    Console.WriteLine();
}
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
