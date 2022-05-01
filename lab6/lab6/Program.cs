using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Linq;


namespace Masterfind
{
    public delegate bool CheckLogin(string phoneNumber, string password, AppContext db);
    public delegate void GetUserPhoneNumber(string phoneNumber);

    public interface ITryLogin : IRebuildData
    {
        public static bool[] TryLoginUsers(AppContext db, bool[] login, string userType, CheckLogin chk)
        {
            Console.Clear();
            if (userType == "Customer")
            {
                Console.WriteLine("Вы зашли как клиент");
            }
            else if (userType == "Master")
            {
                Console.WriteLine("Вы зашли как мастер");
            }
            string phNumber, pass;
            Console.Write("Введите логин:");
            phNumber = Console.ReadLine();
            Console.Write("Введите пароль:");
            pass = Console.ReadLine();
            login[0] = chk(phNumber, pass, db);
            if (login[0])
            {
                login[1] = false;
            }
            else
            {
                Console.WriteLine("Ошибка логина или пароля. Повторить попытку?(y - yes/n - no)");
                if (Console.ReadKey().Key == ConsoleKey.N)
                {
                    login[1] = false;
                }
            }
            return login;

        }
        public bool[] TryLogin(AppContext db, bool[] login);

        public static bool[] TryRegistryUsers(AppContext db, bool[] login, string userType, GetUserPhoneNumber getPhone)
        {
            string[] specialities = { "обучение", "продажа", "настройка", "починка" };
            string[] instruments = { "гитара", "барабаны", "калимба", "фортепиано", "орган", "бас-гитара", "флейта" };
            int trusted; double tr;
            if (userType == "Customer")
            {
                Console.Clear();
                Console.WriteLine("Вы зашли как клиент");
                Customer cust = new Customer();
                Console.Write("Введите возраст:");
                if (Int32.TryParse(Console.ReadLine(), out trusted))
                    cust.Age = trusted;
                else
                {
                    Console.WriteLine("Ошибка ввода! Повторите попытку");
                    return login;
                }
                Console.Write("Введите имя:");
                cust.Name = Console.ReadLine();
                Console.Write("Введите фамилию:");
                cust.Surname = Console.ReadLine();
                Console.Write("Введите пароль:");
                cust.Password = Console.ReadLine();
                Console.Write("Введите номер телефона(это и будет ваш логин):");
                cust.PhoneNumber = Console.ReadLine();
                foreach (var cc in db.Customers.ToList())
                {
                    if (cc.PhoneNumber == cust.PhoneNumber)
                    {
                        Console.WriteLine("Пользователь с таким номером телефона уже существует. Хотите зайти?(y-yes/n-no)");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            login[1] = false;
                            return login;
                        }
                        else
                            return login;
                    }
                }
                Console.Write("Введите email:");
                cust.Email = Console.ReadLine();
                
                login[0] = Program.CheckDataValidity(cust.Name, cust.Surname, cust.Age, cust.Email, cust.PhoneNumber, specialities, instruments);
                if (login[0])
                {
                    login[1] = false;
                    db.Customers.Add(cust);
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Повторить попытку?(y - yes/n - no)");
                    if (Console.ReadKey().Key == ConsoleKey.N)
                    {
                        login[1] = false;
                    }
                }
                getPhone(cust.PhoneNumber);

            }
            else if (userType == "Master")
            {
                Console.Clear();
                Console.WriteLine("Вы зашли как мастер");
                Master mst = new Master();
                Console.Write("Введите возраст:");
                if (Int32.TryParse(Console.ReadLine(), out trusted))
                    mst.Age = trusted;
                else
                {
                    Console.WriteLine("Ошибка ввода! Повторите попытку");
                    return login;
                }
                Console.Write("Введите имя:");
                mst.Name = Console.ReadLine();
                Console.Write("Введите фамилию:");
                mst.Surname = Console.ReadLine();
                Console.Write("Введите пароль:");
                mst.Password = Console.ReadLine();
                Console.Write("Введите номер телефона(это и будет ваш логин):");
                mst.PhoneNumber = Console.ReadLine();
                foreach (var cc in db.Customers.ToList())
                {
                    if (cc.PhoneNumber == mst.PhoneNumber)
                    {
                        Console.WriteLine("Пользователь с таким номером телефона уже существует. Хотите зайти?(y-yes/n-no)");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            login[1] = false;
                            return login;
                        }
                        else
                            return login;
                    }
                }
                Console.Write("Введите email:");
                mst.Email = Console.ReadLine();
                Console.Write("Укажите ваш опыт работы:");
                if (double.TryParse(Console.ReadLine(), out tr))
                    mst.Experience = tr;
                else
                {
                    Console.WriteLine("Ошибка ввода! Повторите попытку");
                    Console.ReadKey();
                    return login;
                }
                Console.Write("Введите желаемые виды деятельности через запятую без пробелов (на выбор: обучение, продажа, настройка, починка):");
                mst.Speciality = Console.ReadLine();
                Console.Write("Введите инструменты, с которыми вы работаете через запятую без пробелов (на выбор: гитара, барабаны, калимба, фортепиано, орган, бас-гитара, флейта):");
                mst.Instrument = Console.ReadLine();
                login[0] = Program.CheckDataValidity(mst.Name, mst.Surname, mst.Age, mst.Email, mst.PhoneNumber, specialities, instruments, mst.Speciality, mst.Instrument, mst.Experience);
                if (login[0])
                {
                    login[1] = false;
                    string s = mst.Speciality;
                    string ss = mst.Instrument;
                    foreach (var spec in s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        mst.Speciality = spec;
                        foreach (var inst in ss.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            mst.Instrument = inst;
                            db.Masters.Add(mst);
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                    }

                }
                else
                {
                    Console.WriteLine("Повторить попытку?(y - yes/n - no)");
                    if (Console.ReadKey().Key == ConsoleKey.N)
                    {
                        login[1] = false;
                    }
                }
                getPhone(mst.PhoneNumber);
            }
            return login;
        }

        public bool[] TryRegistry(AppContext db, bool[] login);


    }

    public interface IRebuildData
    {
        public static bool ChangeUserParam(AppContext db, string userType, string phone)
        {
            int trusted; double tr;
            string[] specialities = { "обучение", "продажа", "настройка", "починка" };
            string[] instruments = { "гитара", "барабаны", "калимба", "фортепиано", "орган", "бас-гитара", "флейта" };
            while (true)
            {
                Console.Clear();
                if (userType == "Customer")//как создать переменную в зависимости от булевой функции
                {
                    Customer user = new Customer();
                    user.PhoneNumber = phone;
                    var pas = db.Customers.Where(a => a.PhoneNumber == phone).FirstOrDefault();
                    user.Password = pas.Password;
                    Console.Write("Введите возраст:");
                    if (Int32.TryParse(Console.ReadLine(), out trusted))
                        user.Age = trusted;
                    else
                    {
                        Console.WriteLine("Ошибка ввода! Повторите попытку");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Введите имя:");
                    user.Name = Console.ReadLine();
                    Console.Write("Введите фамилию:");
                    user.Surname = Console.ReadLine();
                    Console.Write("Введите email:");
                    user.Email = Console.ReadLine();
                    if (Program.CheckDataValidity(user.Name, user.Surname, user.Age, user.Email, phone, specialities, instruments))
                    {
                        var oldUser = db.Customers.Where(a => a.PhoneNumber == phone).FirstOrDefault();
                        oldUser.Name = user.Name;
                        oldUser.Age = user.Age;
                        oldUser.Email = user.Email;
                        oldUser.Surname = user.Surname;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Повторить попытку?(y - yes/n - no)");
                        if (Console.ReadKey().Key == ConsoleKey.N)
                        {
                            return false;

                        }
                    }
                }
                else if (userType == "Master")
                {
                    Master user = new Master();
                    user.PhoneNumber = phone;
                    var pas = db.Masters.Where(a => a.PhoneNumber == phone).FirstOrDefault();
                    user.Password = pas.Password;
                    Console.Write("Введите возраст:");
                    if (Int32.TryParse(Console.ReadLine(), out trusted))
                        user.Age = trusted;
                    else
                    {
                        Console.WriteLine("Ошибка ввода! Повторите попытку");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Введите имя:");
                    user.Name = Console.ReadLine();
                    Console.Write("Введите фамилию:");
                    user.Surname = Console.ReadLine();
                    Console.Write("Введите email:");
                    user.Email = Console.ReadLine();
                    Console.Write("Укажите ваш опыт работы:");
                    if (double.TryParse(Console.ReadLine(), out tr))
                        user.Experience = tr;
                    else
                    {
                        Console.WriteLine("Ошибка ввода! Повторите попытку");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Укажите новый набор видов деятельности через запятую, без пробелов(на выбор: обучение, продажа, настройка, починка): ");
                    user.Speciality = Console.ReadLine();
                    Console.Write("Укажите новый набор инструментов, с которыми вы работаете, через запятую, без пробелов(на выбор: гитара, барабаны, калимба, фортепиано, орган, бас-гитара, флейта): ");
                    user.Instrument = Console.ReadLine();
                    if (Program.CheckDataValidity(user.Name, user.Surname, user.Age, user.Email, phone, specialities, instruments, user.Speciality, user.Instrument, user.Experience))
                    {
                        string s = user.Speciality;
                        string ss = user.Instrument;
                            foreach (var spec in s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                            {
                                user.Speciality = spec;
                                foreach (var inst in ss.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                {
                                    user.Instrument = inst;
                                    try
                                    {
                                        var oldUser = db.Masters.Where(a => a.PhoneNumber == phone && a.Speciality == user.Speciality && a.Instrument == user.Instrument).FirstOrDefault();
                                        oldUser.Name = user.Name;
                                        oldUser.Age = user.Age;
                                        oldUser.Email = user.Email;
                                        oldUser.Experience = user.Experience;
                                        oldUser.Surname = user.Surname;
                                    }
                                    catch (Exception)
                                    {
                                        db.Masters.Add(user);

                                    }
                                    db.SaveChanges();
                                }
                                db.SaveChanges();
                            }
                        foreach (var item in db.Masters.Where(a=> a.PhoneNumber == phone && a.Name != user.Name))
                        {
                            db.Masters.Remove(item);
                        }
                        db.SaveChanges();
                            return true;
                    }
                    else
                    {
                        Console.WriteLine("Повторить попытку?(y - yes/n - no)");
                        if (Console.ReadKey().Key == ConsoleKey.N)
                        {
                            return false;
                        }
                    }

                }


            
            }
        }
        public bool ChangeParam(AppContext db);
    }

    public class Customer : ITryLogin
    {
        private string _userType = "Customer";
        public string _curPhone, _name;
        
        [Required] [Column("сustomer_name")] [MaxLength(20)]
        public string Name { get; set; }
        [Required] [MaxLength(40)]
        public string Surname { get; set; }
        [Key]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public int Age { get; set; }
        public List<Connect> Connects { get; set; }
        public bool[] TryLogin(AppContext db, bool[] login)
        {
            return ITryLogin.TryLoginUsers(db, login, _userType, CheckLoginCustomers);
        }
        public bool CheckLoginCustomers(string phoneNumber, string password, AppContext db)
        {
            var customers = db.Customers.ToList();
            GetCustomerPhone(phoneNumber);
            foreach (var ph in customers)
            {
                if (ph.PhoneNumber == phoneNumber && ph.Password == password)
                {
                    return true;
                }
            }
            return false;
        }
        public void GetCustomerPhone(string ph)
        {
            _curPhone = ph;
        }
        public void GetCustomerInfo(AppContext db)
        {
            foreach (var m in db.Customers.ToList())
            {
                if (m.PhoneNumber == _curPhone)
                {
                    _name = m.Name;
                }
            }
        }
        public bool[] TryRegistry(AppContext db, bool[] login)
        {
            return ITryLogin.TryRegistryUsers(db, login, _userType, GetCustomerPhone);
        }

        public bool ChangeParam(AppContext db)
        {
            return IRebuildData.ChangeUserParam(db, _userType, _curPhone);
        }

    }

    public class Master : ITryLogin
    {
        private string _userType = "Master";
        public string _curPhone, _name;

        [Required] [Column("master_name")] [MaxLength(20)]
        public string Name { get; set; }
        [Required] [MaxLength(40)]
        public string Surname { get; set; }
        [Required]
        public string Speciality { get; set; }
        [Required]
        public string Instrument { get; set; }
        [Key]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public int Age { get; set; }
        public double Experience { get; set; }//опыт в годах
        public List<Connect> Connects { get; set; }


        public bool[] TryLogin(AppContext db, bool[] login)
        {
            return ITryLogin.TryLoginUsers(db, login, _userType, CheckLoginMasters);
        }
        
        public bool CheckLoginMasters(string phoneNumber, string password, AppContext db)
        {
            var masters = db.Masters.ToList();
            GetCustomerPhone(phoneNumber);
            foreach (var p in masters)
            {
                if (p.PhoneNumber == phoneNumber && p.Password == password)
                {
                    return true;
                }
            }
            return false;
        }
        public void GetMasterInfo(AppContext db)
        {
            foreach (var m in db.Masters.ToList())
            {
                if (m.PhoneNumber == _curPhone)
                {
                    _name = m.Name;
                }
            }
        }

        public void GetCustomerPhone(string ph)
        {
            _curPhone= ph;
        }

        public bool[] TryRegistry(AppContext db, bool[] login)
        {
            return ITryLogin.TryRegistryUsers(db, login, _userType, GetCustomerPhone);
        }
        public bool ChangeParam(AppContext db)
        {
            return IRebuildData.ChangeUserParam(db, _userType, _curPhone);
        }
    }

    public class Connect
    {
        public string CustomerId { get; set; }
        public string MasterId { get; set; }
        public string MasterSpec { get; set; } 
        public string MasterInstr { get; set; } 
        public virtual Customer Customer { get; set; }//навигационные свойства
        public Master Master  { get; set; }
    }



    [NotMapped]
    class Program
    {
        public static bool CheckDataValidity(string name, string surname, int age, string email, string phoneNumber, string[] specialities, string[] instruments, string speciality = "продажа", string instrument = "гитара", double exp = 1.1)
        {
            if (name.Length > 20)
            {
                Console.WriteLine("Ошибка ввода имени. Длина не должна превышать 20 символов");
                return false;
            }
            if (surname.Length > 40)
            {
                Console.WriteLine("Ошибка ввода имени. Длина не должна превышать 40 символов");
                return false;
            }
            if (!(email.Contains('@') && email.IndexOf('@') != 0 && email.Contains('.') && email.IndexOf('.', email.IndexOf('@')+1) != -1))
            {
                Console.WriteLine("Ошибка формата email. Повторите попытку ввода.");
                return false;
            }
            if (age < 14 || age > 120)
            {
                Console.WriteLine("Пользователи с таким возрастом к регистрации не допускаются. Пожалуйста, закройте приложение");
                return false;
            }
            if (!(Int64.TryParse(phoneNumber, out long a) && phoneNumber.Length == 11))
            {
                Console.WriteLine("Ошибка ввода номера телефона. Такого номера не существует");
                return false;
            }
            if (exp <= 0 && exp >= 60)
            {
                Console.WriteLine("Такого опыта работы не бывает. Укажите верную информацию");
                return false;
            }
            if (exp <= 0 && exp >= 60)
            {
                Console.WriteLine("Такого опыта работы не бывает. Укажите верную информацию");
                return false;
            }
            foreach (var spec in speciality.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!specialities.Contains(spec))
                {
                    Console.WriteLine("Указана неверная специальность. Пожалуйста, выберите из списка, а не придумывайте свою.");
                    return false;
                }
            }
            foreach (var inst in instrument.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!instruments.Contains(inst))
                {
                    Console.WriteLine("Указан неверный инструмент. Пожалуйста, выберите из списка, а не придумывайте свой.");
                    return false;
                }
            }
            return true;
        }
        public static void WrongEnter()
        {
            Console.Clear();
            Console.WriteLine("Неверный ввод");
            Thread.Sleep(1000);
        }

        public static bool AddToFavorite(AppContext db, bool param, string baseConf, Customer customer, string anotherConf = "")
        {
            string ph;
            while (true)
            {
                Console.Write("\nУкажите номер телефона мастера: ");
                ph = Console.ReadLine();
                if (!(Int64.TryParse(ph, out long a) && ph.Length == 11))
                {
                    Console.WriteLine("Ошибка ввода номера телефона. Такого номера не существует");
                    continue;
                }
                if (param)
                {
                    Console.Write("Укажите желаемый вид деятельности(специальность): ");
                    anotherConf = Console.ReadLine();
                }
                else if(anotherConf == "")
                {
                    Console.Write("Укажите желаемый инструмент из списка: ");
                    baseConf = Console.ReadLine();
                    string t = baseConf;
                    anotherConf = t;
                }
                Master findbb = (from master in db.Masters
                                       where master.Speciality == anotherConf && master.PhoneNumber == ph && master.Instrument == baseConf
                                       select master).FirstOrDefault();
                try
                {
                    Connect c1 = new Connect();
                    c1.CustomerId = customer._curPhone;
                    c1.MasterId = findbb.PhoneNumber;
                    c1.MasterInstr = findbb.Instrument;
                    c1.MasterSpec = findbb.Speciality;
                    try
                    {
                        db.Connects.Add(c1);
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Вы уже добавили этого мастера с указанным номером, инструментом и специальностью в ваш список. Хотите указать другого?(y-yes)");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            continue;
                        }
                        else return false;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка ввода, номер телефона или другой параметр указан неверно.Повторите попытку.");
                }
            }
        }
       
        public static bool RemoveConnect(AppContext db, string phone)
        {
            while (true)
            {

                Connect c1 = new Connect();
                Console.Write("\nУкажите, номер телефона мастера, которого вы хотите удалить:");
                try
                {
                    c1.MasterId = Console.ReadLine();
                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка ввода номера телефона!");
                    Console.ReadKey();
                    continue;
                }
                Console.Write("Введите желаемые виды деятельности через запятую без пробелов (на выбор: обучение, продажа, настройка, починка):");
                c1.MasterSpec = Console.ReadLine();
                Console.Write("Введите инструменты, с которым работает мастер, через запятую без пробелов (на выбор: гитара, барабаны, калимба, фортепиано, орган, бас-гитара, флейта):");
                c1.MasterInstr = Console.ReadLine();
                c1.CustomerId = phone;
                try
                {
                    db.Connects.Remove(db.Connects.Where(a => a.MasterId == c1.MasterId && a.MasterSpec == c1.MasterSpec && a.MasterInstr == c1.MasterInstr && a.CustomerId == phone).FirstOrDefault());
                    db.SaveChanges();
                    Console.WriteLine("Хотите продолжить удаление?(y-yes)");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        continue;
                    }
                    return true;

                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка ввода...");
                    Console.WriteLine("Хотите продолжить удаление?(y-yes)");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        continue;
                    }
                    return false;
                }

            }
        }

        public static void Main(string[] args)
        {
            string clORms = "Customer";
            using (AppContext db = new AppContext())
            {
                var customer = new Customer();
                var master = new Master();
                
                Customer c1 = new Customer { Age = 20, Name = "Sergey", Surname = "Shchankin", PhoneNumber = "89120119484", Email = "kot_kotov_7878@mail.ru", Password = "123" };
                Customer c2 = new Customer { Age = 35, Name = "Dmitrii", Surname = "vodohliob", PhoneNumber = "88542784390", Email = "bez@vodi.umru", Password = "345" };
                Customer c3 = new Customer { Age = 30, Name = "grecha", Surname = "nozhnih", PhoneNumber = "89134441110", Email = "pishitesuda@gmail.com", Password = "1" };
                Customer c4 = new Customer { Age = 65, Name = "sol", Surname = "piterskaya", PhoneNumber = "89122190000", Email = "solpiter@yandex.yandex", Password = "44" };
                Customer c5 = new Customer { Age = 50, Name = "evgenii", Surname = "petrosian", PhoneNumber = "89120000000", Email = "muka@mail.ru", Password = "9" };
                Customer c6 = new Customer { Age = 25, Name = "pavel", Surname = "obruch", PhoneNumber = "89575389346", Email = "a@a.a", Password = "1111" };


                Master m1 = new Master { Age = 29, Name = "Emil", Surname = "Dokukin", PhoneNumber = "88005553535", Email = "emidok@gudok.com", Speciality = "обучение", Instrument = "гитара", Experience = 3, Password = "000" };
                Master m2 = new Master { Age = 49, Name = "eg", Surname = "sm", PhoneNumber = "88006666635", Email = "smeg@gems.gg", Speciality = "починка", Instrument = "барабаны", Experience = 8, Password = "5" };
                Master m3 = new Master { Age = 29, Name = "naruto", Surname = "saske", PhoneNumber = "88005655353", Email = "poel@popil.normalno", Speciality = "настройка", Instrument = "калимба", Experience = 21, Password = "100" };
                Master m4 = new Master { Age = 49, Name = "gon", Surname = "killua", PhoneNumber = "88005655355", Email = "lavrovii@list.pelmeni", Speciality = "продажа", Instrument = "бас-гитара", Experience = 18, Password = "50" };

                Connect co1 = new Connect { CustomerId = c1.PhoneNumber, MasterId = m2.PhoneNumber, MasterInstr = m2.Instrument, MasterSpec = m2.Speciality };
                Connect co2 = new Connect { CustomerId = c1.PhoneNumber, MasterId = m1.PhoneNumber, MasterInstr = m1.Instrument, MasterSpec = m1.Speciality };
                Connect co3 = new Connect { CustomerId = c1.PhoneNumber, MasterId = m3.PhoneNumber, MasterInstr = m3.Instrument, MasterSpec = m3.Speciality };
                Connect co4 = new Connect { CustomerId = c1.PhoneNumber, MasterId = m4.PhoneNumber, MasterInstr = m4.Instrument, MasterSpec = m4.Speciality };

                db.Customers.AddRange(c1, c2,c3,c4,c5,c6);
                db.Masters.AddRange(m1, m2,m3,m4);
                db.Connects.AddRange(co1, co2, co3, co4);
                db.SaveChanges();

                while (true)
                {
                    bool[] log = new bool[] { false, true };
                    Console.Clear();
                    Console.WriteLine("Добро пожаловать, укажите, в качестве кого вы заходите:");
                    Console.WriteLine("c - Clien/m - Master");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.C:
                            clORms = "Customer";
                            Console.Clear();
                            Console.WriteLine("Вы зашли как клиент");
                            Console.WriteLine("Хотите зарегистрироваться или войти?");
                            Console.WriteLine("r - Registry/l - Login");
                            switch (Console.ReadKey().Key)
                            {
                                case ConsoleKey.R:
                                    while (log[1])
                                    {
                                        log = customer.TryRegistry(db, log);
                                    }
                                    break;
                                case ConsoleKey.L:
                                    while (log[1])
                                    {
                                        log = customer.TryLogin(db, log);
                                    }
                                    break;
                                default:
                                    WrongEnter();
                                    break;
                            }
                                    break;
                        case ConsoleKey.M:
                            clORms = "Master";
                            Console.Clear();
                            Console.WriteLine("Вы зашли как мастер");
                            Console.WriteLine("Хотите зарегистрироваться или войти?");
                            Console.WriteLine("r - Registry/l - Login");
                            switch (Console.ReadKey().Key)
                            {
                                case ConsoleKey.R:
                                    while (log[1])
                                    {
                                        log = master.TryRegistry(db, log);
                                    }
                                    break;
                                case ConsoleKey.L:
                                    while (log[1])
                                    {
                                        log = master.TryLogin(db, log);
                                    }
                                    break;
                                default:
                                    WrongEnter();
                                    break;
                            }
                            break;
                        default:
                            WrongEnter();
                            break;
                    }
                    if (log[0])
                    {
                        log[1] = true;
                        if (clORms == "Customer")
                        {
                            customer.GetCustomerInfo(db);
                            while (log[1])
                            {
                                Console.Clear();
                                customer.GetCustomerInfo(db);
                                Console.WriteLine($"Добро пожаловать, {customer._name} ");
                                Console.WriteLine("Вы можете воспользоваться одной из следующих услуг:");
                                Console.WriteLine("1 - Найти мастера по инструменту\n2 - Найти мастера по роду деятельности\n3 - Найти мастера по инструменту и роду деятельности\n4 - Просмотреть добавленных мастеров\n5 - Изменить профиль\n0 - Выйти");
                                switch (Console.ReadKey().Key)
                                {
                                    case ConsoleKey.D1:
                                        Console.Clear();
                                        Console.Write("Укажите инструмент(на выбор: гитара, барабаны, калимба, фортепиано, орган, бас-гитара, флейта): ");
                                        string instrumen = Console.ReadLine();
                                        var instrument = db.Masters.Where(m => m.Instrument == instrumen);
                                        Console.WriteLine("Имя\t Фамилия\t Специльность\t Возраст\t Опыт(в годах)\t Номер телефона");
                                        foreach (var i in instrument)
                                        {
                                            Console.WriteLine($"{i.Name}\t{i.Surname}\t\t {i.Speciality}\t   {i.Age}\t\t\t{i.Experience}\t   {i.PhoneNumber}");
                                        }
                                        while (true)
                                        {
                                            Thread.Sleep(1000);//пауза перед выводом предложения
                                            Console.WriteLine("Хотите добавить кого-нибудь?(y-yes/любая клавиша-нет)");
                                            if (Console.ReadKey().Key == ConsoleKey.Y)
                                            {
                                                if (AddToFavorite(db, true, instrumen, customer))
                                                {
                                                    Console.WriteLine("Мастер успешно добавлен!");
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        break;
                                    case ConsoleKey.D2:
                                        Console.Clear();
                                        Console.Write("Укажите вид деятельности (на выбор: обучение, продажа, настройка, починка): ");
                                        string spec = Console.ReadLine();
                                        var Special = db.Masters.Where(m => m.Speciality == spec);
                                        Console.WriteLine("Имя\t Фамилия\t Инструмент\t Возраст\t Опыт(в годах)\t Номер телефона");
                                        foreach (var i in Special)
                                        {
                                            Console.WriteLine($"{i.Name}\t{i.Surname}\t\t {i.Instrument}\t\t   {i.Age}\t\t\t{i.Experience}\t   {i.PhoneNumber}");
                                        }
                                        while (true)
                                        {
                                            Thread.Sleep(1000);//пауза перед выводом предложения
                                            Console.WriteLine("Хотите добавить кого-нибудь?(y-yes/любая клавиша-нет)");
                                            if (Console.ReadKey().Key == ConsoleKey.Y)
                                            {
                                                if (AddToFavorite(db, false, spec, customer))
                                                {
                                                    Console.WriteLine("Мастер успешно добавлен!");
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        break;
                                    case ConsoleKey.D3:
                                        Console.Clear();
                                        Console.Write("Укажите вид деятельности (на выбор: обучение, продажа, настройка, починка): ");
                                        string spec1 = Console.ReadLine();
                                        Console.Write("Укажите инструмент(на выбор: гитара, барабаны, калимба, фортепиано, орган, бас-гитара, флейта): ");
                                        string instrumen1 = Console.ReadLine();
                                        var instrument1 = db.Masters.Where(m => m.Instrument == instrumen1 && m.Speciality == spec1);
                                        Console.WriteLine("Имя\t Фамилия\t Возраст\t Опыт(в годах)\t Номер телефона");
                                        foreach (var i in instrument1)
                                        {
                                            Console.WriteLine($"{i.Name}\t{i.Surname}\t\t   {i.Age}\t\t\t{i.Experience}\t   {i.PhoneNumber}");
                                        }
                                        while (true)
                                        {
                                            Thread.Sleep(1000);//пауза перед выводом предложения
                                            Console.WriteLine("Хотите добавить кого-нибудь?(y-yes/любая клавиша-нет)");
                                            if (Console.ReadKey().Key == ConsoleKey.Y)
                                            {
                                                if (AddToFavorite(db, false, instrumen1, customer, spec1))
                                                {
                                                    Console.WriteLine("Мастер успешно добавлен!");
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        break;
                                    case ConsoleKey.D4:
                                        Console.Clear();
                                        Console.WriteLine("Имя\t Фамилия\t Специльность\t Инструмент\t Возраст\t Опыт(в годах)\t Номер телефона");
                                        foreach (var i in db.Connects.Where(a=> a.CustomerId == customer._curPhone))
                                        {
                                            Console.WriteLine($"{i.Master.Name}\t{i.Master.Surname}\t\t {i.Master.Speciality}\t {i.Master.Instrument}\t   {i.Master.Age}\t\t\t{i.Master.Experience}\t   {i.Master.PhoneNumber}");
                                        }
                                        Console.WriteLine("Если хотите удалить кого-нибудь из списка, нажмите D");
                                        if (Console.ReadKey().Key==ConsoleKey.D)
                                        {
                                            if (RemoveConnect(db, customer._curPhone))
                                            {
                                                Console.WriteLine("\nУдаление прошло успешно!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nОшибка удаления.");
                                            }
                                        }
                                        Console.WriteLine("Для продолжения нажмите любую клавишу..."); Console.ReadKey();
                                        break;
                                    case ConsoleKey.D5:
                                        if (customer.ChangeParam(db))
                                        {
                                            Console.WriteLine("Обновление прошло успешно! Возвращаемся в главное меню...");
                                            Thread.Sleep(1500);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Обновление не произошло, данные не изменились. Возвращаемся в главное меню...");
                                            Thread.Sleep(1500);
                                        }
                                        break;
                                        break;
                                    case ConsoleKey.D0:
                                        log[1] = false;
                                        break;
                                    default:
                                        WrongEnter();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            master.GetMasterInfo(db);
                            while (log[1])
                            {
                                Console.Clear();
                                master.GetMasterInfo(db);
                                Console.WriteLine($"Добро пожаловать, {master._name} ");
                                Console.WriteLine("Вы можете воспользоваться одной из следующих услуг:");
                                Console.WriteLine("1 - Просмотреть добавленных клиентов\n2 - Обновить информацию профиля\n0 - Выйти");
                                switch (Console.ReadKey().Key)
                                {
                                    case ConsoleKey.D1:
                                        Console.Clear();
                                        Console.WriteLine("Имя\t Фамилия\t  Возраст\t Номер телефона");
                                        foreach (var i in db.Connects.Where(a=> a.MasterId == master._curPhone))
                                        {
                                            Console.WriteLine($"{i.Customer.Name}\t{i.Customer.Surname}\t {i.Customer.Age}\t   {i.Customer.PhoneNumber}");
                                        }
                                        Console.WriteLine("Для продолжения нажмите любую клавишу..."); Console.ReadKey();
                                        break;
                                    case ConsoleKey.D2:
                                        if (master.ChangeParam(db))
                                        {
                                            Console.WriteLine("Обновление прошло успешно! Возвращаемся в главное меню...");
                                            Thread.Sleep(1500);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Обновление не произошло, данные не изменились. Возвращаемся в главное меню...");
                                            Thread.Sleep(1500);
                                        }
                                        break;
                                    case ConsoleKey.D0:
                                        log[1] = false;
                                        break;
                                    default:
                                        WrongEnter();
                                        break;
                                }

                            }
                        }

                    }
                }
            }
        }
    }

    public class AppContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Connect> Connects { get; set; }

        public AppContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=prof_i;Trusted_Connection=True;");
            //optionsBuilder.UseSqlite($"Data Source={DbPath}");
            //optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Master>().HasKey(u => new {u.PhoneNumber, u.Speciality, u.Instrument});
            modelBuilder.Entity<Connect>().HasKey(u => new { u.CustomerId, u.MasterId, u.MasterInstr, u.MasterSpec });
            modelBuilder.Entity<Connect>()
                .HasOne(c => c.Customer)
                .WithMany(co => co.Connects)
                .HasForeignKey(c => c.CustomerId);
            modelBuilder.Entity<Connect>()
                .HasOne(c => c.Master)
                .WithMany(m => m.Connects)
                .HasForeignKey(c => new { c.MasterId, c.MasterSpec, c.MasterInstr})
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}