﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.CodeDom;

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
        public override string ToString()
        {
            return $"LogIn: {LogIn}, password: {Password}";
        }
    }
    static class Global
    {
        public const int DepretiationTime = 60;

    }
    [Serializable]
    public class Container
    {
        public string ContNumber { get; set; }
        public int ContWeight { get; set; }
        public int ContSize { get; set; }
        public string ContType { get; set; }
        public double MaxWeight { get; set; }
        public string Condition { get; set; }
        public Container() { }
        public override string ToString()
        {
            return $"{ContNumber}/{ContSize}{ContType}, cont weight: {ContWeight}, " +
                $"max payload: {MaxWeight}, condition: {Condition}\n";
        }
    }
    [Serializable]
    public class Company
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyStreet { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyPhone { get; set; }
        public Company()
        {

        }
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
    class Expense { }
    public abstract class Order
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime Validity { get; set; }
        public List<Container> Containers { get; set; }
        public override string ToString()
        {
            StringBuilder stringBuilder= new StringBuilder();
            stringBuilder.Append($"Order: {OrderNumber}, from: {OrderDate.ToShortDateString()}, " +
                $"valid till: {Validity.ToShortDateString()}\nContainers:\n");

            foreach (var cont in Containers)
            {
                stringBuilder.Append( cont );
            }
            return stringBuilder.ToString();
        }
    }
    [Serializable]
    public class PurchaseRentOrder: Order
    {
        public Company Supplier { get; set; }
        public decimal PricePerDay { get; set; }
        public PurchaseRentOrder(Company supplier, string orderNumber, DateTime orderDate,
            decimal price, DateTime validity, List<Container> containers)
        {
            Supplier = supplier;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            PricePerDay = price;
            Validity = validity;
            Containers = containers;
        }
        public PurchaseRentOrder()
        {

        }
        public override string ToString()
        {
            return base.ToString() + $"\nSupplier: {Supplier}, price per day: {PricePerDay}";
        }
    }
    [Serializable]
    public class SalesOrder : Order
    {
        public Company Seller { get; set; } 
        public decimal SellerPrice { get; set; }

        public SalesOrder(Company seller, decimal sellerPrice, string orderNumber, DateTime orderDate,
            DateTime validity, List<Container> containers)
        {
            Seller = seller;
            SellerPrice = sellerPrice;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            Validity = validity;
            Containers = containers;
        }
        public SalesOrder()
        {

        }
        public override string ToString()
        {
            return base.ToString() + $"\nSeller: {Seller}, seller price: {SellerPrice}";
        }
    }
    [Serializable]
    public class CustomerRentOrder: Order
    {
        public Company Customer { get; set; }
        public decimal PricePerDay { get; set; }
        public CustomerRentOrder(Company customer, decimal pricePerDay, string orderNumber, 
            DateTime orderDate, DateTime validity, List<Container> containers)
        {
            Customer = customer;
            PricePerDay = pricePerDay;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            Validity= validity;
            Containers = containers;
        }
        public CustomerRentOrder()
        {

        }
        public override string ToString()
        {
            return base.ToString() + $"\nCustomer: {Customer}, price per day: {PricePerDay}";
        }
    }
    internal class ContLine
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
            Console.Clear();
            Console.WriteLine("\nPlease make your choice: ");
            Console.WriteLine("\n1. Create new Rent Order in database");
            Console.WriteLine("2. View all existing Rent Orders");
            Console.WriteLine("3. Create new Sale Order in database");
            Console.WriteLine("4. View all existing Sale Orders");
            Console.WriteLine("5. Close existing contract");
            Console.WriteLine("6. View all active containers");
            Console.WriteLine("7. Find contract/container");
            Console.WriteLine("8. Create financial report");

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
                if (item.LogIn == user.LogIn && item.Password == user.Password)
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

            FileStream streamLoad = new FileStream("../../Users.bin", FileMode.Open);
            BinaryFormatter formatterLoad = new BinaryFormatter();

            users = (List<User>)formatterLoad.Deserialize(streamLoad);

            streamLoad.Close();
            Console.WriteLine("Загрузка из файла успешно выполнена!");
            return users;
        }
        static List<PurchaseRentOrder> LoadPurchaseRentOrderFromFile()
        {
            List<PurchaseRentOrder> temp = new List<PurchaseRentOrder>();
            FileStream stream = new FileStream("../../PurchaseRentOrder.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(List<PurchaseRentOrder>));
            temp = (List<PurchaseRentOrder>)serializer.Deserialize(stream);
            stream.Close();
            return temp;
        }
        static List<SalesOrder> LoadSalesOrdersFromFile()
        {
            List<SalesOrder> temp = new List<SalesOrder>();
            FileStream stream2 = new FileStream("../../SalesOrders.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(List<SalesOrder>));
            temp = (List<SalesOrder>)serializer.Deserialize(stream2);
            stream2.Close();
            return temp;
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
        static List<Container> NewContLot() 
        {
            List<Container> containers = new List<Container>();

            Console.WriteLine("Enter information about containers:");
            int count = 1; int temp;
            while (true)
            {

                Console.WriteLine($"To enter {count++} container press 1");
                Console.WriteLine("To continue press 0");
                temp = Convert.ToInt32(Console.ReadLine());

                if (temp == 0)
                    break;

                Container container = new Container();

                Console.Write("Enter container number: ");
                container.ContNumber = Console.ReadLine();

                Console.Write("Enter container weight: ");
                container.ContWeight = Convert.ToInt32(Console.ReadLine());

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
        static PurchaseRentOrder NewPurchaseRentOrder(Company company, List<Container> contList)
        {
            PurchaseRentOrder RentOrder = new PurchaseRentOrder();

            RentOrder.Supplier = company;
            RentOrder.Containers = contList;

            Console.Write("Please enter number of order: ");
            RentOrder.OrderNumber = Console.ReadLine();

            Console.Write("Please enter order date: ");
            RentOrder.OrderDate = Convert.ToDateTime(Console.ReadLine());

            Console.Write("Please enter validity date: ");
            RentOrder.Validity = Convert.ToDateTime(Console.ReadLine());

            Console.Write("Please enter price per date: ");
            RentOrder.PricePerDay = Convert.ToDecimal(Console.ReadLine());

            return RentOrder;
        }
        static SalesOrder NewSalesOrder(Company company, List<Container> contList)
        {
            SalesOrder saleOrder = new SalesOrder();

            saleOrder.Seller = company;
            saleOrder.Containers = contList;

            Console.Write("Please enter number of order: ");
            saleOrder.OrderNumber = Console.ReadLine();

            Console.Write("Please enter order date: ");
            saleOrder.OrderDate = Convert.ToDateTime(Console.ReadLine());

            Console.Write("Please enter validity date: ");
            saleOrder.Validity = Convert.ToDateTime(Console.ReadLine());

            Console.Write("Please enter price of container: ");
            saleOrder.SellerPrice = Convert.ToDecimal(Console.ReadLine());

            return saleOrder;
        }
        private static void Main(string[] args)
        {
            try
            {            
                FileStream stream = null;
                BinaryFormatter formatter = null;

                List<User> users = new List<User>();
                users = LoadUsersFromFile();

                List<Container> ListOfContainers = new List<Container>();
                List<Company> companies = new List<Company>();

                List<PurchaseRentOrder> purchaseRentOrders = new List<PurchaseRentOrder>();
                purchaseRentOrders = LoadPurchaseRentOrderFromFile();

                List<SalesOrder> salesOrders = new List<SalesOrder>();
                salesOrders = LoadSalesOrdersFromFile();

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
                                    {
                                        stream = new FileStream("../../PurchaseRentOrder.xml", FileMode.Create);
                                        XmlSerializer serPurchaseOrders = new XmlSerializer(typeof(List<PurchaseRentOrder>));
                                        serPurchaseOrders.Serialize(stream, purchaseRentOrders);
                                        stream.Close();
                                        Console.WriteLine("RentOrders saved!");

                                        stream = new FileStream("../../SalesOrders.xml", FileMode.Create);
                                        XmlSerializer serSalesOrders = new XmlSerializer(typeof(List<SalesOrder>));
                                        serSalesOrders.Serialize(stream, salesOrders);
                                        stream.Close();
                                        Console.WriteLine("SalesOrders saved!");


                                        Console.ReadKey();
                                        break;
                                    }

                                    switch(temp)
                                    {
                                        case 1: 
                                            purchaseRentOrders.Add(NewPurchaseRentOrder(NewCompany(companies), NewContLot()));
                                            Console.WriteLine("\nNew Rent Order added successfully!");
                                            break;
                                        case 2:
                                            foreach (var item in purchaseRentOrders)
                                            {
                                                Console.WriteLine(item);
                                            }
                                            Console.ReadKey();
                                            break;
                                        case 3:
                                            salesOrders.Add(NewSalesOrder(NewCompany(companies), NewContLot()));
                                            Console.WriteLine("\nNew Sale Order added successfully!");
                                            break;
                                        case 4:
                                            foreach (var item in salesOrders)
                                            {
                                                Console.WriteLine(item);
                                            }
                                            Console.ReadKey();
                                            break;
                                        case 6:
                                            foreach (var item in purchaseRentOrders)
                                            {
                                                foreach (var cont in item.Containers)
                                                {
                                                    Console.Write(cont);
                                                }
                                            }
                                            foreach (var item in salesOrders)
                                            {
                                                foreach (var cont in item.Containers)
                                                {
                                                    Console.Write(cont);
                                                }
                                            }
                                            Console.ReadKey();
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
                            stream = new FileStream("../../Users.bin", FileMode.Create);
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