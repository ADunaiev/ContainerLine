using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;

//Задание GOL Container Line
//Создать приложение GOL Container Line
//Основная задача проекта создать приложение позволяющее компании следить за собственным и 
//арендованным контейнерным оборудованием. Оценивать прибыльность проекта в разрезе 
//контейнера.
//Интерфейс приложения должен позволять следующие возможности:
//■ При старте приложения пользователь вводит логин и пароль для входа. Если пользователь не
//зарегистрирован, он должен пройти процесс регистрации. 
//■ При регистрации нужно указать: • логин(нельзя зарегистрировать уже существующий логин); • 
//пароль;

//■ После входа в систему пользователь может:
//• Взятие/снятие с учета контейнеров
//o Добавить новый лот приобретенных/арендованных контейнеров
//▪ Возможность внесения номеров, типов, грузоподъемности, собственника
//▪ Состояние контейнера
//▪ Условия приобретения или аренды
//▪ Расчет амортизации.
//o Посмотреть все контейнеры в оперировании
//▪ Возможность внесения номеров, типов, грузоподъемности, собственника
//▪ Состояние контейнера
//▪ Условия приобретения или аренды
//o Вывести контейнеры(лот контейнеров) из учета
//• изменить настройки: можно менять пароль и дату рождения
//• Регистрация сделок по передачу контейнеров в перевозку
//o Котировка ставок
//▪ Возможность выборки по нужному типу и грузоподъемности контейнеров
//▪ Отображение себестоимости (аммортизации контейнера в день/месяц)
//▪ Расчет продажной ставки с учетом заданной ставки рентабельности.
//o Регистрация букинга
//▪ Номер букинга
//▪ Маршрут перевозки
//▪ Количество контейнеров
//▪ Стоимость использования контейнеров
//▪ Условия сверхнормативного использования контейнеров
//o Контроль за ходом выполнения активных букингов
//▪ Дата начала перевозки
//▪ Возможность регистрации промежуточных дат и локаций контейнера
//▪ Дата завершения перевозки
//o Завершение перевозки
//▪ Начисления дохода по контейнеру
//▪ Проверка и довыставление дохода за сверхнормативное использование 
//контейнера
//• Косвенные расходы
//o Внесение расходов и привязка их к контейнерам:
//▪ по переадресации порожних контейнеров
//▪ По хранению порожних контейнеров
//▪ ПРР порожних контейнеров
//▪ Аренда контейнеров
//▪ Приобретение контейнеров
//• Вывод финансового результата за все контейнеры за период
//o За месяц
//o За квартал
//o За год
//• Вывод финансового результата в разрезе контейнера (лота контейнеров)

namespace ContainerLine
{
    [Serializable]
    public class User
    {
        public string LogIn { get; set; }
        public string Password { get; set; }
        public User(string login, string password)
        {
            LogIn = login;
            Password = password;
        }
        public static bool operator ==(User left, User right)
        {
            return left.LogIn == right.LogIn && left.Password == right.Password;
        }
        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            return $"LogIn: {LogIn}, password: {Password}";
        }
    }
    //public enum ContainerType { ST, HQ, FR, OT }
    static class Global
    {
        public const int DepretiationTime = 60;

    }
    abstract class Container
    {
        public string ContNumber { get; set; }
        public int ContSize { get; set; }
        public string ContType { get; set; }
        public double MaxWeight { get; set; }
        public string Condition { get; set; }
        public override string ToString()
        {
            return $"{ContNumber}/{ContSize}{ContType} {MaxWeight} - {Condition}";
        }
    }
    class OurContainer : Container
    {
        public Company Owner { get; set; }
        public double BuyingValue { get; set; } = 0;
        public double MonthlyCost { get; set; }
        public DateTime ContractDateStart { get; set; }
        public DateTime ContractDateEnd { get; set; }

        public void FinReport()
        { }
        public override string ToString()
        {
            return base.ToString() + $"\n" +
                $"Owner: {Owner}, buying value: {BuyingValue}, " +
                $"contract date: {ContractDateStart}, " +
                $"contract ends: {ContractDateEnd}";
        }
    }
    class Company
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyStreet { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyPhone { get; set; }

        public override string ToString()
        {
            return $"Company name: {CompanyName}\n" +
                $"code: {CompanyCode}\n" +
                $"address: {CompanyStreet}, {CompanyCity}," +
                $" {CompanyPostalCode}, " +
                $"{CompanyCountry}, {CompanyPhone}";
        }
    }
    abstract class FinDocument
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public double DocumentAmount { get; set; }
        public override string ToString()
        {
            return $"Document number: {DocumentNumber}, " +
                $"date: {DocumentDate}, " +
                $"amount: {DocumentAmount}";
        }
    }
    class Income : FinDocument
    {
        public Company customer { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    class Expense
    {
        public Company supplier { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    abstract class Contract
    {
        public Company Counterparty { get; set; }
        public string ContractNumber { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime Validity { get; set; }
    }

    interface IContract
    {
        List<OurContainer> containers { get; set; }
        bool IsActive();
        void SetContainers();
    }
    class PurchaseContract : Contract, IContract
    {
        public List<OurContainer> containers { get; set; }
        public PurchaseContract() { }
        public double Amount { get; set; }
        public bool IsActive()
        {
            if (DateTime.Today <= Validity && DateTime.Today >= ContractDate )
                return true;
            else 
                return false;
        }
        public void SetContainers()
        {
            for (int i = 0; i < containers.Count; i++)
            {
                containers[i].Owner = Counterparty;
                if (containers.Count> 0)
                    containers[i].BuyingValue = Amount / containers.Count;
                containers[i].MonthlyCost =  
                    containers[i].BuyingValue/ containers.Count;
                containers[i].ContractDateStart = ContractDate;
                containers[i].ContractDateEnd = Validity;
            }
        }

    }
    //class RentContractSupplier : Contract, IContract
    //{

    //}
    //class SalesContract : Contract, IContract
    //{

    //}


    internal class Program
    {


        static int StartMenu()
        {
            Console.Clear();
            Console.WriteLine("\tGOL Container Line application\n");
            Console.WriteLine("Please make your choice: ");
            Console.WriteLine("1. Log in (existing user)");
            Console.WriteLine("2. Register as a new user");
            Console.WriteLine("0. Exit application\n");
            Console.Write("Your choice: ");

            return Convert.ToInt32(Console.ReadLine());
        }
        static User LogInMenu()
        {
            Console.Write("\nPlease enter your LogIn: ");
            string tempLogIn = Console.ReadLine();

            Console.Write("Please enter password: ");
            string tempPass = Console.ReadLine();

            return new User(tempLogIn, tempPass);
        }
        static int MainMenu()
        {
            Console.WriteLine("\nPlease make your choice: ");
            Console.WriteLine("\n1. Create new contract in database");
            Console.WriteLine("2. View all existing contracts");
            Console.WriteLine("3. Close existing contract");
            Console.WriteLine("4. View all active containers");
            Console.WriteLine("5. Find contract/container");
            Console.WriteLine("6. Create financial report");

            Console.WriteLine("0. Exit application\n");
            Console.Write("Your choice: ");
            return Convert.ToInt32(Console.ReadLine());
        }
        static void UserRegistration(List<User> users)
        {
            string tempLogIn = string.Empty;
            while (true)
            {
                Console.Write("\nPlease enter your LogIn: ");
                tempLogIn = Console.ReadLine();

                if (CheckLogIn(users, tempLogIn))
                {
                    Console.WriteLine("There is alraady User with this LogIn.");
                }
                else
                    break;
            }

            Console.Write("Please enter password: ");
            string tempPass = Console.ReadLine();

            users.Add(new User(tempLogIn, tempPass));
            Console.WriteLine("You are registered successfully!");

        }
        static bool CheckUser(List<User> users, User user)
        {
            foreach (var item in users)
            {
                if (item == user)
                {
                    return true;                    
                }
                
            }
            return false;
        }
        static bool CheckLogIn(List<User> users, string login)
        {
            foreach (var item in users)
            {
                if (item.LogIn == login)
                {
                    return true;
                }

            }
            return false;
        }
        static List<User> LoadUsersFromFile()
        {
            List<User> users = new List<User>();

            FileStream streamLoad = new FileStream("Users.bin", FileMode.Open);
            BinaryFormatter formatterLoad = new BinaryFormatter();

            users = (List<User>)formatterLoad.Deserialize(streamLoad);

            streamLoad.Close();
            Console.WriteLine("Загрузка из файла успешно выполнена!");
            return users;
        }
        static Company NewCompany(List<Company> companies) 
        { 
            Company company = new Company();

            Console.Write("Enter company name: ");
            company.CompanyName = Console.ReadLine().Trim();

            Console.Write("Enter company code: ");
            company.CompanyCode = Console.ReadLine().Trim();

            Console.Write("Enter company street: ");
            company.CompanyStreet = Console.ReadLine().Trim();

            Console.Write("Enter company city: ");
            company.CompanyCity = Console.ReadLine().Trim();

            Console.Write("Enter company postal code: ");
            company.CompanyPostalCode = Console.ReadLine().Trim();

            Console.Write("Enter company country: ");
            company.CompanyCountry = Console.ReadLine().Trim();

            Console.Write("Enter company phone: ");
            company.CompanyPhone = Console.ReadLine().Trim();

            companies.Add(company);

            return company;
        }
        static List<OurContainer> NewContLot() 
        {
            List<OurContainer> containers = new List<OurContainer>();

            Console.WriteLine("Enter information about containers:");
            int count = 1; int temp;
            while (true)
            {

                Console.WriteLine($"To enter {count++} container press 1");
                Console.WriteLine("To continue press 0");
                temp = Convert.ToInt32(Console.ReadLine());

                if (temp == 0)
                    break;

                OurContainer container = new OurContainer();

                Console.Write("Enter container number: ");
                container.ContNumber = Console.ReadLine();

                Console.Write("Enter container size: ");
                container.ContSize = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter container type: ");
                container.ContType = Console.ReadLine();

                Console.Write("Enter container max GW: ");
                container.MaxWeight = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter container condition: ");
                container.Condition = Console.ReadLine();

                containers.Add(container);

            }

            return containers; 
        }
        static PurchaseContract NewPurchaseContract(Company company, List<OurContainer> contList)
        {
            PurchaseContract contract = new PurchaseContract();

            contract.Counterparty = company;
            contract.containers= contList;

            Console.Write("Please enter number of contract: ");
            contract.ContractNumber = Console.ReadLine();

            Console.Write("Please enter contract date: ");
            contract.ContractDate = Convert.ToDateTime(Console.ReadLine());

            Console.Write("Please enter validity date: ");
            contract.Validity = Convert.ToDateTime(Console.ReadLine());

            contract.SetContainers();

            return contract;
        }
        private static void Main(string[] args)
        {
            try
            {            
                FileStream stream = null;
                BinaryFormatter formatter = null;
                List<User> users = new List<User>();

                List<OurContainer> ListOfContainers = new List<OurContainer>();
                List<PurchaseContract> purchaseContracts = new List<PurchaseContract>();
                List<Company> companies = new List<Company>();

                users = LoadUsersFromFile();
                
                while (true)
                {
                    switch (StartMenu())
                    {
                        case 1:
                            if (CheckUser(users, LogInMenu()))
                            {
                                Console.Clear();
                                Console.WriteLine("Verification successful!");
                                int temp;
                                while (true)
                                {
                                    temp = MainMenu();
                                    if (temp == 0)
                                        break;

                                    switch(temp)
                                    {
                                        case 1:                                           
                                            NewPurchaseContract(NewCompany(companies), NewContLot());
                                            Console.WriteLine("New contract added successfully!");
                                            break;

                                    }
                                    
                                }
                            }
                            else
                                Console.WriteLine("Invalid Login or Password!");
                            Console.ReadKey();
                            break;
                        case 2:
                            UserRegistration(users);
                            Console.ReadKey();
                            break;
                        case 0:
                            stream = new FileStream("Users.bin", FileMode.Create);
                            formatter = new BinaryFormatter();

                            formatter.Serialize(stream, users);
                            
                            stream.Close();
                            Console.WriteLine("Данные о пользователях сохранены!");
                            Console.ReadKey();
                            return;
                    }
                }
            }

            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message); 
            }

            //Company GOLLT = new Company("Global Ocean Link Lithuania",
            //        304584502, "Ateites 31b", "Vilnius", "LT", "06326",
            //        "Lithuania", "+370-521-43-241");
        }
    }
}
