namespace Home_test_1_3
{
    internal class Program
    {
        static void Main(string[] args) // הכל עובד טוב!
        {
            Customer customer1 = new Customer("Yonatan", 3);
            Customer customer2 = new Customer("Gideon", 6);
            Customer customer3 = new Customer("Shimi", 2);

            Call call1 = new Call(customer1, 50);
            Call call2 = new Call(customer2, 30);
            Call call3 = new Call(customer3, 100);

            Node<Call> calls = new Node<Call>(call1, new Node<Call>(call2, new Node<Call>(call3, null)));

            Console.WriteLine("GenerateUrgent Test:");
            Node<Call> urgentCalls = null;
            Node<Call> temp = calls;
            while (temp != null)
            {
                if (temp.GetValue().Seconds > 40 || temp.GetValue().Customer.IsVIP())
                {
                    urgentCalls = new Node<Call>(new Call(temp.GetValue()), urgentCalls);
                }
                temp = temp.GetNext();
            }

            Node<Call> temp222 = urgentCalls;
            while (temp222 != null)
            {
                Console.WriteLine($"Customer: {temp222.GetValue().Customer.Name}, VIP: {temp222.GetValue().Customer.IsVIP()}, Seconds: {temp222.GetValue().Seconds}");
                temp222 = temp222.GetNext();
            }

            Console.WriteLine("MaxVIPWait Test:");
            Node<Call> maxTemp = calls;
            Customer maxVIPCustomer = null;
            int maxWT = -1;

            while (maxTemp != null)
            {
                Call call = maxTemp.GetValue();
                if (call.Customer.IsVIP() && call.Seconds > maxWT)
                {
                    maxWT = call.Seconds;
                    maxVIPCustomer = call.Customer;
                }
                maxTemp = maxTemp.GetNext();
            }

            if (maxVIPCustomer != null)
            {
                Console.WriteLine($"Max VIP Customer: {maxVIPCustomer.Name}");
            }
            else
            {
                Console.WriteLine("No VIP customer found.");
            }


        }
        public static Node<Call> GenerateUrgent(Node<Call> calls, int threshold)
        {
            Node<Call> urgentList = null;
            Node<Call> copy = calls;

            while (copy != null)
            {
                if (copy.GetValue().Seconds > threshold || copy.GetValue().Customer.IsVIP()) //בדקתי לפי ההוראות שנתנו
                {
                    Call urgentCall = new Call(copy.GetValue()); // = לוקח את השיחה אחרי שהיא עברה את הבדיקה ומכניס למשתנה - וגם יוצר העתק 

                    urgentList = new Node<Call>(urgentCall, urgentList);// מכניס לרשימה החדשה ואחריו מה שהיה לפני בתהחלה זה יצביע לכלום
                }

                copy = copy.GetNext();
            }

            return urgentList;
        }

    }
    public class CallCenter
    {
        public Node<Call> callList;

        public CallCenter(Node<Call> callList)
        {
            this.callList = callList;
        }

        public Customer MaxVIPWait() // לא היה לי הרבה מה להסביר 
        {
            Node<Call> copy = callList; // יצרתי העתק לשמור על המקורי
            Customer maxVIPCustomer = null; // המשתנה של הלקוח עם הכי הרבה זמן
            int maxWT = -1; // הכי הרבה זמן

            while (copy != null)
            {
                Call call = copy.GetValue();

                if (call.Customer.IsVIP() && call.Seconds > maxWT) // בדיקת מקסימום
                {
                    maxWT = call.Seconds;
                    maxVIPCustomer = call.Customer;
                }

                copy = copy.GetNext();
            }

            return maxVIPCustomer;
        }
    }


    public class Customer
    {
        public string Name { get; set; }
        public int YearsAsMember { get; set; }

        public Customer(string name, int yearsAsMember)
        {
            Name = name;
            YearsAsMember = yearsAsMember;
        }

        public Customer(Customer copy)
        {
            Name = copy.Name;
            YearsAsMember = copy.YearsAsMember;
        }

        public bool IsVIP()
        {
            return YearsAsMember > 5;
        }
    }

    public class Call
    {
        public Customer Customer { get; set; }
        public int Seconds { get; set; }

        public Call(Customer customer, int seconds)
        {
            Customer = customer;
            Seconds = seconds;
        }

        public Call(Call copy)
        {
            Customer = new Customer(copy.Customer);
            Seconds = copy.Seconds;
        }
    }

}