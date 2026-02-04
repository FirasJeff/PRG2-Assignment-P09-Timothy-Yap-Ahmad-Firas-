using FirasTimothy_PRG2Assignment;
string[] restaurantfile = File.ReadAllLines("restaurants.csv");

for(int i = 1; i<restaurantfile.Length; i++)
{
    string lines[] = restaurantfile[i].Split(",");

}

string[] fooditemsfile = File.ReadAllLines("fooditems.csv");

