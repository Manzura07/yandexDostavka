using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        var foodBot = new FoodBot();
        foodBot.Run();
    }
}


class FoodBot
{
    private static Dictionary<string, (string name, decimal price, string time)> menu = new Dictionary<string, (string, decimal, string)>()
    // --> name: biror elementning nomini bildiruvchi string. 
    //price: elementning narxini bildiruvchi decimal.
    //time: elementga bog'liq vaqt bilan bog'liq ma'lumotni bildiruvchi string.
    {
        {"1", ("Burger", 5.900m, "15 daqiqa")}, // --> m qo'shimchasi qiymatning decimal ekanligini bildiradi.
        {"2", ("Pizza", 25.000m, "20 daqiqa")},
        {"3", ("Salat", 15.000m, "10 daqiqa")},
        {"4", ("Gazli suv", 1.900m, "2 daqiqa")},
        {"5", ("Makaron", 15.800m, "15 daqiqa")},
        {"6", ("Sendvich", 15.000m, "10 daqiqa")},
        {"7", ("Sharbat", 9.000m, "2 daqiqa")},
        {"8", ("Qahva", 20.500m, "2 daqiqa")},
        {"9", ("Kartoshka fri", 15.000m, "5 daqiqa")},
        {"10", ("Muzqaymoq", 10.000m, "5 daqiqa")}
    };

    private static Dictionary<string, decimal> promoCodes = new Dictionary<string, decimal>()
    {
        {"DISCOUNT10", 0.10m},
        {"SAVE20", 0.20m}
    };

    private static List<string> order = new List<string>(); // --> buyurtmalarni o'z ichiga saqlab oladi.
    private static decimal totalCost = 0m; // -> umumiy qiymatni saqlab oladi.
    private static string promoCode = string.Empty; // --> promocodni saqlaydi.

    private static string phoneNumber;
    private static string address;
    private static string firstName;
    private static string lastName;

    public void Run() // --> bu yerda kutib olish va buyurtmalarni olish jarayoni o'tadi.
    {
        WelcomeUser();
        RegisterUser();

        string input;
        do
        {
            Console.Clear();
            ShowMenu();
            Console.WriteLine("Ovqatni buyurtmaga qo'shish uchun raqamni kiriting, 'promo' promo - kodni kiritish uchun yoki 'checkout' buyurtmani yakunlash uchun:");
            input = Console.ReadLine()!.ToLower();

            if (menu.ContainsKey(input))
            {
                AddToOrder(input);
            }
            else if (input == "promo")
            {
                EnterPromoCode();
            }
            else if (input == "checkout")
            {
                Checkout();
            }
            else
            {
                Console.WriteLine("Noto'g'ri kirish. Iltimos, qayta urinib ko'ring.");
                Console.ReadKey();
            }
        } while (input != "checkout");
    }

    private static void WelcomeUser() // --> kutib olish.
    {
        Console.Clear();
        Console.WriteLine("Restoran botimizga xush kelibsiz!");
    }

    private static void RegisterUser() // --> registratsiya jarayoni.
    {
        Console.Clear();
        Console.WriteLine("Xush kelibsiz! Buyurtma berishdan oldin ro'yxatdan o'ting.");

        firstName = GetValidInput("Ismingizni kiriting: ", "Ismni to'g'ri kiriting, unda raqamlar bo'lmasligi kerak.");
        lastName = GetValidInput("Familiyangizni kiriting: ", "Familiyani to'g'ri kiriting, unda raqamlar bo'lmasligi kerak.");

        bool isValidPhoneNumber; // --> telefon nomerni tekshiradi agar harf, yoki raqamlardan tashqari narsalar kiritilsa hatolik chiqaradi.
        do
        {
            Console.Write("Telefon raqamingizni kiriting: ");
            phoneNumber = Console.ReadLine()!;
            isValidPhoneNumber = Regex.IsMatch(phoneNumber, @"^\d+$"); // -- regret.isMatch bu nomer faqat raqamlardan ekanligini tekshiradi.
            if (!isValidPhoneNumber)
            {
                Console.WriteLine("Noto'g'ri telefon raqami. Iltimos, faqat raqamlarni kiriting.");
            }
        } while (!isValidPhoneNumber);

        address = GetValidInput("Manzilingizni kiriting: ", "Manzilni to'ldirish kerak.");
    }

    private static string GetValidInput(string prompt, string errorMessage) // --> foydalanuvchi tog'ri formatda malumot olishi uchun ishlatiladi.
    {
        string input;
        bool isValid;
        do
        {
            Console.Write(prompt); // --> foydalanuvchiga korsatiladigan so'rov xabar ko'rsatiladi.
            input = Console.ReadLine()!;
            isValid = !string.IsNullOrEmpty(input) && !Regex.IsMatch(input, @"\d"); // --> malumot bosh yoki null ekanligini tekshiradi, 
            //regex.Match --> raqamlar borligini tekshiradi.
            if (!isValid)
            {
                Console.WriteLine(errorMessage); // xato narsa kiritilsa korsatadi.
            }
        } while (!isValid);
        return input;
    }

    private static void ShowMenu() // --> menu korsatadi.
    {
        Console.WriteLine("Menyu:");
        foreach (var item in menu)
        {
            Console.WriteLine($"{item.Key}. {item.Value.name} - {item.Value.price} sum - {item.Value.time}");
        }
        Console.WriteLine();
    }

    private static void AddToOrder(string dishNumber) // --> buyurtmani royxatga qo'shadi. umumiy narxini korsatadi.
    {
        order.Add(dishNumber); // --> buyurtma (order) ga qoshadi. 
        totalCost += menu[dishNumber].price; // ovqat narxini olib totalcostga umumiy narxini qoshadi. 
        //menu bu yerda Dictionary<string, (string, decimal, string)> tipida.
        Console.WriteLine($"{menu[dishNumber].name} buyurtmangizga qo'shildi."); // --> bu yerda interpolation bilan string qolanilgan.
        // ovqat nomini o'z ichiga oladi va uni satrga kiritadi.
        Console.ReadKey(); // --> foydalanuvchi biror tugmani bosmagunigacha kutadi.
    }

    private static void EnterPromoCode() // --> promocodni tekshiradi.
    {
        Console.WriteLine("Promo kodni kiriting:");
        string code = Console.ReadLine()!.ToUpper();
        if (promoCodes.ContainsKey(code))
        {
            promoCode = code;
            Console.WriteLine("Promo kod muvaffaqiyatli qo'llanildi.");
        }
        else
        {
            Console.WriteLine("Noto'g'ri promo kod.");
        }
        Console.ReadKey();
    }

    private static void Checkout() // --> buyurtmani korsatadi va umumiy hisobni ko'rsatadi.
    {
        Console.Clear(); // --> consolni tozalaydi.
        Console.WriteLine("Buyurtmangiz:");
        decimal discount = 0m; // --> ozgaruvchi 0ga teng ekanligini elon qiladi.
        if (!string.IsNullOrEmpty(promoCode))
        {
            discount = totalCost * promoCodes[promoCode];
            Console.WriteLine($"Promo kod qo'llanildi: {promoCode} - Chegirma: {discount} sum"); // --> bu kod foydalanuvchida promocod bor bolsa ishlaydi.
        }

        foreach (var item in order) // --> order royxatiga qaraydi va ovqatni nomini qachon tayyorlanish vaqtini korsatadi.
        {
            Console.WriteLine($"{menu[item].name} - {menu[item].price} sum - {menu[item].time}");
        }
        Console.WriteLine($"Umumiy qiymat: sum {totalCost - discount}"); // --> umumiy hisobni chiqaradi.
        Console.WriteLine("Buyurtmangiz uchun rahmat!");

        Console.WriteLine("Chiqish uchun biron bir tugmani bosing."); // --> biror tugma bosak avtomatik ravishda botdan chiqib ketadi.
        Console.ReadKey();
    }
}
